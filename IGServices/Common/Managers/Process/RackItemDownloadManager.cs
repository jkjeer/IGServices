using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class RackItemDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackItemDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"RackItemDownloadManager.GetReceivedQty", GetReceivedQty()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal rackId, List<string> statusCodes)
        {
            var response = new ProcessResponse("Rack Detail Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Spans");

            ProcessRunSpan processRunSpanRackItem = new ProcessRunSpan();
            HighWaterMark highWaterMarkRackItem = new HighWaterMark();

            try
            {
                highWaterMarkRackItem = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "RackDetail") : null;
                processRunSpanRackItem = ProcessRunSpanManager.GetRunSpan(highWaterMarkRackItem, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpanRackItem", processRunSpanRackItem.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {
                var table = new RackItemTable();
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    if (useHighWaterMark)
                    {
                        var reader = GetRackItemLedger(session, processRunSpanRackItem, plant.PlantNum, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    else
                    {
                        var reader = GetRackItems(session, processRunSpanRackItem, plant.PlantNum, rackId, statusCodes);
                        table.Add(reader, plant);
                    }
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse =
                                BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                    table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Update High Water Marks");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMarkRackItem.LocalDateTime = table.MaxDate.AddHours(plant.TimeOffset);
                            highWaterMarkRackItem.EnterpriseDateTime = table.MaxDate;
                            if (table.MaxId > 0)
                            {
                                highWaterMarkRackItem.UniqueID = (long)table.MaxId;
                            }
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMarkRackItem));
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /********************************************************************************
        * Rack Item
        ********************************************************************************/
        public static IDataReader GetRackItems(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Cannot run wide Open
                if (!statusCodes.Any() && processRunSpan.IsEmpty && rackId == 0)
                {
                    filters.Add("0 = 1 AND \r\n");
                }

                // Plant - Required
                filters.Add("RDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("RDZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("RDZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetRackItems(filters);
                for (int i = 0; i < 1; i++)
                {
                    foreach (iDB2Parameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackItems(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $@"
SELECT 
	*
FROM (
	SELECT
		RDLOTN AS LOTN,
		RDZ58SN AS Z58SN,
		RDITM AS ITM,
		RDMCU AS MCU,
		RDZ58DST AS Z58DST,
		RDZ58RID AS Z58RID,
		RDZ58MDT AS Z58MDT,
		RDZ58PDT AS Z58PDT,
		RDPQOH AS PQOH,
        RDZ58CQTY AS RQOH,
		RDZ58PS AS Z58PS,
		RDZ58SL AS Z58SL,
		RDZ58STK AS Z58STK,
		RDZ58ROW AS Z58ROW,
		RDZ58SD AS Z58SD,
		RDZ58BXRO AS Z58BXRO,
		RDDOC1 AS DOC1,
		RDPKCO AS PKCO,
		RDDOC2 AS DOC2,
		RDXCTO AS XCTO,
		RDLNIC AS LNIC,
		RDKCOO AS KCOO,
		RDDOCO AS DOCO,
		RDDCTO AS DCTO,
		RDLNID AS LNID,
		RDUPMJ AS UPMJ,
		RDTDAY AS TDAY,
		CAST(0 AS DECIMAL(15,0)) AS UKID,
		CAST('Init' AS VARCHAR(10)) AS IOPR,
        COALESCE(DLMUPM, 0) AS MUPM,
        COALESCE(DLUPMT, 0) AS UPMT,
        RDUSER AS USERID,
        RDPID AS PID,
		IBSTKT AS STKT,
		IBGLPT AS GLPT,
		COALESCE(PDMCU, '') AS PDMCU,
        COALESCE(PDAN8, 0) AS PDAN8,
        COALESCE(PCUST.ABALPH, '') AS PDALPH,
		COALESCE(SDMCU, '') AS SDMCU,
        COALESCE(SDAN8, 0) AS SDAN8,
        COALESCE(SCUST.ABALPH, '') AS SDALPH,
		COALESCE(WAMCU, '') AS WAMCU,
        COALESCE(WAAN8, 0) AS WAAN8,
        COALESCE(WCUST.ABALPH, '') AS WAALPH,
        COALESCE(CMCFGID, 0) AS CFGID,
		ROW_NUMBER() OVER (PARTITION BY RDLOTN, RDZ58SN ORDER BY CMCFGID DESC) AS RN
	FROM
		F580056
		INNER JOIN F4102
		ON	IBMCU = RDMCU AND
			IBITM = RDITM
		LEFT OUTER JOIN F4311 LEFT OUTER JOIN F0101 PCUST ON PCUST.ABAN8 = PDAN8
		ON  PDKCOO = RDPKCO AND
			PDDOCO = RDDOC2 AND
			PDDCTO = RDXCTO AND
			PDLNID = RDLNIC AND
			PDMCU = RDMCU
		LEFT OUTER JOIN F4211 LEFT OUTER JOIN F0101 SCUST ON SCUST.ABAN8 = SDAN8
		ON  SDKCOO = RDKCOO AND
			SDDOCO = RDDOCO AND
			SDDCTO = RDDCTO AND
			SDLNID = RDLNID AND
			SDMCU = RDMCU
		LEFT OUTER JOIN F4801 LEFT OUTER JOIN F0101 WCUST ON WCUST.ABAN8 = WAAN8
		ON	WADOCO = RDDOC1 AND
			WAMCU = RDMCU
		LEFT OUTER JOIN F3201
		ON	CMKCOO = COALESCE(PDKCOO, SDKCOO) AND
			CMDOCO = COALESCE(PDDOCO, SDDOCO) AND
			CMDCTO = COALESCE(PDDCTO, SDDCTO) AND
			CMLNID = COALESCE(PDLNID, SDLNID) 
        LEFT OUTER JOIN F580569
        ON  DLMCU = RDMCU AND
            DLLOTN = RDLOTN AND
            DLITM = RDITM AND
            DLZ58SN = RDZ58SN AND
            DLIOPR = 'Insert'
	WHERE
        {filterString} 1=1
	) A
WHERE 
	A.RN = 1
FOR READ ONLY"
            };
            return cmd;
        }

        /********************************************************************************
        * Rack Item Ledger
        ********************************************************************************/
        public static IDataReader GetRackItemLedger(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal rackId, List<string> statusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Cannot run wide Open
                if (processRunSpan.HighWaterMark.Exists && processRunSpan.HighWaterMark.UniqueID == 0)
                {
                    filters.Add("0 = 1 AND \r\n");
                }

                // Plant - Required
                filters.Add("DLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("DLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Status Codes
                if (statusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in statusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("DLZ58DST IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // High Water Mark
                if (processRunSpan.HighWaterMark.Exists)
                {
                    filters.Add("DLUKID > ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;
                }
                else
                {
                    // Process Run Span
                    if (!processRunSpan.IsEmpty)
                    {
                        filters.Add(E1TimeManager.GetE1DateTimeQueryString("DLMUPM", "DLUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                    }
                }

                // Command
                var cmd = GetRackItemLedger(filters);
                for (int i = 0; i < 2; i++)
                {
                    foreach (iDB2Parameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static iDB2Command GetRackItemLedger(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
SELECT 
	*
FROM (
	SELECT
		DLLOTN AS LOTN,
		DLZ58SN AS Z58SN,
		DLITM AS ITM,
		DLMCU AS MCU,
		DLZ58DST AS Z58DST,
		DLZ58RID AS Z58RID,
		DLZ58MDT AS Z58MDT,
		DLZ58PDT AS Z58PDT,
		DLPQOH AS PQOH,
        DLZ58CQTY AS RQOH,
		DLZ58PS AS Z58PS,
		DLZ58SL AS Z58SL,
		DLZ58STK AS Z58STK,
		DLZ58ROW AS Z58ROW,
		DLZ58SD AS Z58SD,
		DLZ58BXRO AS Z58BXRO,
		DLDOC1 AS DOC1,
		DLPKCO AS PKCO,
		DLDOC2 AS DOC2,
		DLXCTO AS XCTO,
		DLLNIC AS LNIC,
		DLKCOO AS KCOO,
		DLDOCO AS DOCO,
		DLDCTO AS DCTO,
		DLLNID AS LNID,
		DLMUPM AS UPMJ,
		DLUPMT AS TDAY,
		DLUKID AS UKID,
		DLIOPR AS IOPR,
		DLMUPM AS MUPM,
		DLUPMT AS UPMT,
        DLUSER AS USERID,
        DLPID AS PID,
		IBSTKT AS STKT,
		IBGLPT AS GLPT,
		COALESCE(PDMCU, '') AS PDMCU,
        COALESCE(PDAN8, 0) AS PDAN8,
        COALESCE(PCUST.ABALPH, '') AS PDALPH,
		COALESCE(SDMCU, '') AS SDMCU,
        COALESCE(SDAN8, 0) AS SDAN8,
        COALESCE(SCUST.ABALPH, '') AS SDALPH,
		COALESCE(WAMCU, '') AS WAMCU,
        COALESCE(WAAN8, 0) AS WAAN8,
        COALESCE(WCUST.ABALPH, '') AS WAALPH,
        COALESCE(CMCFGID, 0) AS CFGID,
		ROW_NUMBER() OVER (PARTITION BY DLLOTN, DLZ58SN ORDER BY DLUKID DESC) AS RN
	FROM
		F580569
		INNER JOIN F4102
		ON	IBMCU = DLMCU AND
			IBITM = DLITM
		LEFT OUTER JOIN F4311 LEFT OUTER JOIN F0101 PCUST ON PCUST.ABAN8 = PDAN8
		ON  PDKCOO = DLPKCO AND
			PDDOCO = DLDOC2 AND
			PDDCTO = DLXCTO AND
			PDLNID = DLLNIC AND
			PDMCU = DLMCU
		LEFT OUTER JOIN F4211 LEFT OUTER JOIN F0101 SCUST ON SCUST.ABAN8 = SDAN8
		ON  SDKCOO = DLKCOO AND
			SDDOCO = DLDOCO AND
			SDDCTO = DLDCTO AND
			SDLNID = DLLNID AND
			SDMCU = DLMCU
		LEFT OUTER JOIN F4801 LEFT OUTER JOIN F0101 WCUST ON WCUST.ABAN8 = WAAN8
		ON	WADOCO = DLDOC1 AND
			WAMCU = DLMCU
		LEFT OUTER JOIN F3201
		ON	CMKCOO = COALESCE(PDKCOO, SDKCOO) AND
			CMDOCO = COALESCE(PDDOCO, SDDOCO) AND
			CMDCTO = COALESCE(PDDCTO, SDDCTO) AND
			CMLNID = COALESCE(PDLNID, SDLNID) 
	WHERE
        {filterString} 
		DLIOPR = 'Insert'
	) A
WHERE
	A.RN = 1

UNION ALL

SELECT 
	*
FROM (
	SELECT
		DLLOTN AS LOTN,
		DLZ58SN AS Z58SN,
		DLITM AS ITM,
		DLMCU AS MCU,
		DLZ58DST AS Z58DST,
		DLZ58RID AS Z58RID,
		DLZ58MDT AS Z58MDT,
		DLZ58PDT AS Z58PDT,
		DLPQOH AS PQOH,
        DLZ58CQTY AS RQOH,
		DLZ58PS AS Z58PS,
		DLZ58SL AS Z58SL,
		DLZ58STK AS Z58STK,
		DLZ58ROW AS Z58ROW,
		DLZ58SD AS Z58SD,
		DLZ58BXRO AS Z58BXRO,
		DLDOC1 AS DOC1,
		DLPKCO AS PKCO,
		DLDOC2 AS DOC2,
		DLXCTO AS XCTO,
		DLLNIC AS LNIC,
		DLKCOO AS KCOO,
		DLDOCO AS DOCO,
		DLDCTO AS DCTO,
		DLLNID AS LNID,
		DLMUPM AS UPMJ,
		DLUPMT AS TDAY,
		DLUKID AS UKID,
		DLIOPR AS IOPR,
		DLMUPM AS MUPM,
		DLUPMT AS UPMT,
        DLUSER AS USERID,
        DLPID AS PID,
		IBSTKT AS STKT,
		IBGLPT AS GLPT,
		CAST(NULL AS VARCHAR(12))  AS PDMCU,
        CAST(NULL AS DECIMAL(8,0)) AS PDAN8,
        CAST(NULL AS VARCHAR(40)) AS PDALPH,
		CAST(NULL AS VARCHAR(12))  AS SDMCU,
        CAST(NULL AS DECIMAL(8,0)) AS SDAN8,
        CAST(NULL AS VARCHAR(40)) AS SDALPH,
		CAST(NULL AS VARCHAR(12))  AS WAMCU,
        CAST(NULL AS DECIMAL(8,0)) AS WAAN8,
        CAST(NULL AS VARCHAR(40)) AS WAALPH,
        CAST(NULL AS DECIMAL(8,0)) AS CFGID,
		ROW_NUMBER() OVER (PARTITION BY DLLOTN, DLZ58SN ORDER BY DLUKID DESC) AS RN
	FROM
		F580569
		INNER JOIN F4102
		ON	IBMCU = DLMCU AND
			IBITM = DLITM
	WHERE
        {filterString} 
		DLIOPR <> 'Insert'
	) A
WHERE
	A.RN = 1
ORDER BY 
	UKID
FOR READ ONLY"
            };
            return cmd;
        }

    }
}
