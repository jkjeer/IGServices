using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class RelatedOrderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public RelatedOrderDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Related Order Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new RelatedOrderTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "RelatedOrder") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
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
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetReader(session, plant.PlantNum, processRunSpan);
                    table.Add(reader, plant);
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
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
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
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate;
                            highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                            //highWaterMark.LocalDateTime = table.MaxDate.AddHours(plant.TimeOffset);
                            //highWaterMark.EnterpriseDateTime = table.MaxDate;
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
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
            response.Close();
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetReader(IReadOnlyCollection<string> filters)
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
	                ROMCU,
	                RODOCO,
	                RODCTO,
	                ROLNID,
	                ROZ58L1CLN,
	                ROMCU2,
	                RODOC1,
	                RORCTO,
	                RORLLN,
	                ROEDTR,
	                ROUORG,
	                ROZ58JBSTS,
	                ROUPMJ,
	                ROTDAY,
                    WA1.WADOCO,
                    WA1.WADCTO,
                    COALESCE(PDAN8, 0) AS PDAN8
                FROM 
	                F580400
	                LEFT OUTER JOIN F4211 SD1
	                ON	SD1.SDKCOO = ROKCOO AND
		                SD1.SDDOCO = RODOCO AND
		                SD1.SDDCTO = RODCTO AND
		                SD1.SDLNID = ROLNID	                
                    LEFT OUTER JOIN F42119 SD2
	                ON	SD2.SDKCOO = ROKCOO AND
		                SD2.SDDOCO = RODOCO AND
		                SD2.SDDCTO = RODCTO AND
		                SD2.SDLNID = ROLNID
					LEFT OUTER JOIN F580032
					ON  ATKCOO = COALESCE(SD1.SDKCOO, SD2.SDKCOO) AND
						ATDOCO = COALESCE(SD1.SDDOCO, SD2.SDDOCO) AND
						ATDCTO = COALESCE(SD1.SDDCTO, SD2.SDDCTO) AND
						ATLNID = COALESCE(SD1.SDLNID, SD2.SDLNID) 
                    LEFT OUTER JOIN F4801 WA1
                    ON  WA1.WADOCO = ATZ58WODC AND
						WA1.WADOCO > 0
                    LEFT OUTER JOIN F4311
                    ON  PDKCOO = RORKCO AND
                        PDDOCO = RODOC1 AND
                        PDDCTO = RORCTO AND
                        PDLNID = RORLLN
	            WHERE 
                    {filterString}
                    ROZ58JBSTS = '30' AND
	                RODOCO > 0 AND
					ROEDTR IN ('S','V','R') 
                FOR READ ONLY"
            };
            return cmd;
        }

        private static iDB2Command GetReaderOLD(IReadOnlyCollection<string> filters)
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
	                ROMCU,
	                RODOCO,
	                RODCTO,
	                ROLNID,
	                ROZ58L1CLN,
	                ROMCU2,
	                RODOC1,
	                RORCTO,
	                RORLLN,
	                ROEDTR,
	                ROUORG,
	                ROZ58JBSTS,
	                ROUPMJ,
	                ROTDAY,
                    COALESCE(WA1.WADOCO, WA2.WADOCO) AS WADOCO,
                    COALESCE(WA1.WADCTO, WA2.WADCTO) AS WADCTO,
                    COALESCE(PDAN8, 0) AS PDAN8
                FROM 
	                F580400
	                LEFT OUTER JOIN F4211 SD1
	                ON	SD1.SDKCOO = ROKCOO AND
		                SD1.SDDOCO = RODOCO AND
		                SD1.SDDCTO = RODCTO AND
		                SD1.SDLNID = ROLNID	                
                    LEFT OUTER JOIN F42119 SD2
	                ON	SD2.SDKCOO = ROKCOO AND
		                SD2.SDDOCO = RODOCO AND
		                SD2.SDDCTO = RODCTO AND
		                SD2.SDLNID = ROLNID
                    -- SO
                    LEFT OUTER JOIN F4801 WA1
                    ON  COALESCE(SD1.SDRORN, SD2.SDRORN) = WA1.WADOCO AND
                        COALESCE(SD1.SDRCTO, SD2.SDRCTO) = WA1.WADCTO AND
                        COALESCE(SD1.SDRORN, SD2.SDRORN) != '' AND
                        COALESCE(SD1.SDRCTO, SD2.SDRCTO) = 'SO' 
                    -- ST
                    LEFT OUTER JOIN F4801 WA2
                    ON  COALESCE(SD1.SDDOCO, SD2.SDDOCO) = WA2.WARORN AND
                        COALESCE(SD1.SDDCTO, SD2.SDDCTO) = WA2.WARCTO AND                        
                        COALESCE(SD1.SDLNID, SD2.SDLNID) = WA2.WALNID AND                        
                        WA2.WARORN != '' AND
                        COALESCE(SD1.SDDCTO, SD2.SDDCTO) = 'ST' 
                    LEFT OUTER JOIN F4311
                    ON  PDKCOO = RORKCO AND
                        PDDOCO = RODOC1 AND
                        PDDCTO = RORCTO AND
                        PDLNID = RORLLN
	            WHERE 
                    {filterString}
	                RODOCO > 0
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, string plantNum, ProcessRunSpan processRunSpan)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("ROMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("ROUPMJ", "ROTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                // Command
                var cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
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
    }
}
