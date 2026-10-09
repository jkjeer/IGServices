using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class ShippableRackDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ShippableRackDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var response = new ProcessResponse("Shippable Rack Download");
            var table = new ShippableRackTable();
            try
            {
                response.Plant = plant;
                var step1 = new ProcessStep("E1 Extract");
                try
                {
                    var reader = this.GetReader(session, cn, plant);
                    table.Add(reader);
                    step1.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                if (table.Rows.Count > 0)
                {
                    var step2a = new ProcessStep("Truncate EBS.ShippableRacks Table");
                    try
                    {
                        step2a.SqlDmlResponses.Add(TruncateTable(cn));
                    }
                    catch (Exception e)
                    {
                        step2a.Errors.Add(new Error(e));
                    }
                    step2a.Done();
                    response.AddStep(step2a);

                    var step2 = new ProcessStep("SF Download");
                    try
                    {
                        var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                        if (bulkResponse.HasErrors())
                        {
                            step2.Errors.AddRange(bulkResponse.Errors);
                        }
                        step2.SqlBulkCopyResponses.Add(bulkResponse);
                        step2.RowsAffected = (int)step2.SqlBulkCopyResponses.Last().RowsCopied;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    response.AddStep(step2);

                    var step3 = new ProcessStep("EXEC dbo.usp_UpdateShippingRacks");
                    try
                    {
                        step3.SqlDmlResponses.Add(UpdateShippingInfo(cn));
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    step3.Done();
                    response.AddStep(step3);
                }
                response.Done();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            return response;
        }

        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT
	            RHMCU,
	            RHZ58RID,
	            RHZ58RT,
	            RHZ58SCN,
	            RHZ58BOX,
	            RHZ58HST,
				RHZ58HST_SCHED,
	            RDDOC1,
                RDLOTN,
	            RDZ58SN,
                RDZ58DST,
	            DLZ58RID,
	            WASRST,
	            UNITS	
            FROM (
	            SELECT
		            RH1.RHMCU,
		            RH1.RHZ58RID,
		            RH1.RHZ58RT,
		            RH1.RHZ58SCN,
		            RH1.RHZ58BOX,
		            RH1.RHZ58HST,
					RH2.RHZ58HST AS RHZ58HST_SCHED ,
		            RDDOC1,
                    RDLOTN,
		            RDZ58SN,
                    RDZ58DST,
		            DLZ58RID,
		            WASRST,
		            1 AS UNITS,
		            ROW_NUMBER() OVER (PARTITION BY DLMCU, DLLOTN, DLZ58SN ORDER BY DLMUPM, DLUPMT) AS ROWNUM
	            FROM 
		            F580056
		            INNER JOIN F580055 RH1
		            ON	RH1.RHZ58RID = RDZ58RID
		            INNER JOIN F4801
		            ON	WADOCO = RDDOC1
		            INNER JOIN F580569
		            ON	DLMCU = RDMCU AND
			            DLLOTN = RDLOTN AND
			            DLZ58SN = RDZ58SN AND
			            DLIOPR = 'Insert'
					INNER JOIN F580055 RH2
					ON RH2.RHZ58RID = DLZ58RID
	            WHERE
		            {0}
		            RH1.RHZ58HST NOT IN ('7','8','9') AND
		            WASRST < '96' AND
		            RH1.RHZ58SCN < 1000) A
            WHERE 
	            A.ROWNUM = 1
            ORDER BY 4
            FOR READ ONLY", filterString.ToString());
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Plant - Required
                filters.Add("RH1.RHMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;
                _logger.Debug(Logging.DbCmdString(cmd));

                _logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public SqlDmlResponse TruncateTable(SqlConnection cn)
        {
            var dmlResponse = new SqlDmlResponse();
            try
            {
                var cmd = new SqlCommand();
                cmd.CommandText = "TRUNCATE TABLE ShopFloor.EBS.ShippableRacks";
                cmd.Connection = cn;
                dmlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                dmlResponse.RowsAffected = rows;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dmlResponse;
        }

        public SqlDmlResponse UpdateShippingInfo(SqlConnection cn)
        {
            var dmlResponse = new SqlDmlResponse();
            try
            {
                var cmd = new SqlCommand();
                cmd.CommandText = "EXEC ShopFloor.dbo.usp_UpdateShippingRacks";
                cmd.Connection = cn;
                cmd.CommandTimeout = 0;
                dmlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                dmlResponse.RowsAffected = rows;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dmlResponse;
        }
    }
}
