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
    public class SalesAttributeDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public SalesAttributeDownloadManager(BusinessSystemSession session)
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
            var response = new ProcessResponse("Sales Attribute Download");
            var orderTable = new SalesAttributeTable();
            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580032") : null;
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
	                    ATUKID,
	                    ATDOCO,
	                    ATDCTO,
	                    ATLNID,
	                    ATZ58PT,
	                    ATZ58CP2,
	                    ATZ58CP3,
	                    ATZ58CP4,
	                    ATZ58CSQNO,
	                    ATZ58GSQNO,
	                    ATZ5840460,
	                    ATZ5899009,
	                    ATZ5899010,
	                    ATZ5899011,
	                    ATZ5899012,
	                    ATZ5899074,
	                    ATZ5899075,
	                    ATZ5899076,
	                    ATUPMJ,
	                    ATTDAY
                    FROM 
	                    F580032
                        INNER JOIN F4211
                        ON  ATKCOO = SDKCOO AND
                            ATDOCO = SDDOCO AND
                            ATDCTO = SDDCTO AND
                            ATLNID = SDLNID
	                WHERE 
                        {filterString}
                        SDADDJ = 0 AND
                        SDCNDJ = 0 AND
	                    ATDOCO > 0"
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
                filters.Add("SDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("ATDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }
                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("ATUPMJ", "ATTDAY", span.E1TimeIdSpan);
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
