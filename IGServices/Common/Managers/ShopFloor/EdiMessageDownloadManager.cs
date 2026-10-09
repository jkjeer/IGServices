using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
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

namespace IGServices.Common.Managers.ShopFloor
{
    public class EdiMessageDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public EdiMessageDownloadManager(BusinessSystemSession session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
        //    {
        //        session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark, decimal orderNum)
        {
            var response = new ProcessResponse("Edi Message Download");
            var table = new EdiMessageTable();
            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F4715") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));
            //var rt1 = new ResponseTime("Get From E1");
            var reader = GetReader(session, cn, plant, processRunSpan, 0);
            //rt1.Done();
            //response.ResponseTimes.Add(rt1);
            table.Add(reader);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                // Set High Water Mark
                if (mark != null)
                {
                    mark.EnterpriseDateTime = table.MaxDate;
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, mark));
                }
            }
            return response;
        }

        public iDB2Command GetReader(List<string> filters)
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
                        ZDKCOO,
                        ZDDOCO,
                        ZDDCTO,
                        ZDLNID,
                        ZDLINS,
                        ZDEDTY,
                        ZDEDSQ,
                        ZDTXLN,
                        ZDUPMJ,
                        ZDTDAY
                    FROM 
	                    F4715
                        INNER JOIN F4201
                        ON  SHKCOO = ZDKCOO AND
                            SHDOCO = ZDDOCO AND
                            SHDCTO = ZDDCTO
	                WHERE 
                        {filterString}
	                    ZDDOCO > 0"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("SHMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("ZDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }
                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("ZDUPMJ", "ZDTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }
                var cmd = GetReader(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
