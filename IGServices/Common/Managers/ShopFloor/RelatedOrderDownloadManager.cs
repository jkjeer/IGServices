using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public class RelatedOrderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RelatedOrderDownloadManager(BusinessSystemSession session)
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
            var response = new ProcessResponse("Related Order Download");
            var orderTable = new RelatedOrderTable();
            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580400") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));
            //var rt1 = new ResponseTime("Get From E1");
            var reader = GetReader(session, cn, plant, processRunSpan, 0);
            //rt1.Done();
            //response.ResponseTimes.Add(rt1);
            orderTable.Add(reader);
            if (orderTable.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse = BulkCopyManager.BulkTable(orderTable.ExtendedProperties["Target"].ToString(), orderTable, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                // Set High Water Mark
                if (mark != null)
                {
                    mark.EnterpriseDateTime = orderTable.MaxDate;
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
	                    ROTDAY
                    FROM 
	                    F580400
	                WHERE 
                        {filterString}
	                    RODOCO > 0 AND
	                    ROMCU2 != ''"
                    };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal orderNum)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!string.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("ROMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }
                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("RODOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }
                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("ROUPMJ", "ROTDAY", span.E1TimeIdSpan);
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
