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
    public class RackLedgerDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RackLedgerDownloadManager(BusinessSystemSession session)
        //{
        //    //var hash = new Dictionary<string, iDB2Command>
        //    //{
        //    //    //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
        //    //};
        //    //foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
        //    //{
        //    //    session.DbCommands.Add(keyName, hash[keyName]);
        //    //}
        //}

        public ProcessResponse ProcessAttributes(BusinessSystemSession session, SqlConnection cn, Plant plant, decimal rackId)
        {
            var response = new ProcessResponse("Rack Ledger Download");
            var table = new RackLedgerTable();
            //var rt1 = new ResponseTime("Get From E1");
            var mgr = new Enterprise.RackManager(session);
            var items = mgr.GetRacks(session, rackId, null, null, true);
            //rt1.Done();
            //response.ResponseTimes.Add(rt1);
            table.Add(items);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
            }
            return response;
        }

        public ProcessResponse ProcessStatus(BusinessSystemSession session, SqlConnection cn, Plant plant, decimal rackId)
        {
            var response = new ProcessResponse("Rack Status Ledger Download");
            var table = new RackStatusLedgerTable();
            //var rt1 = new ResponseTime("Get From E1");
            var mgr = new Enterprise.RackManager(session);
            var items = mgr.GetRacks(session, rackId, null);
            //rt1.Done();
            //response.ResponseTimes.Add(rt1);
            table.Add(items);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
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
                        HLUKID,
                        HLZ58RID,
                        HLZ58RT,
                        HLZ58HST,
                        HLLOCN,
                        HLURCD,
                        HLUPMJ,
                        HLTDAY
                    FROM 
	                    F580559
	                WHERE 
                        {filterString} 1=1
                    ORDER BY
                        HLUKID"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan? span, HighWaterMark mark, decimal rackId)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Plant - Required
                filters.Add("HLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                // Order Num
                if (rackId > 0)
                {
                    filters.Add("HLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }
                // TimeSpan
                if (span != null)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("HLUPMJ", "HLTDAY", DateTime.Now.Add(-(TimeSpan)span));
                    filters.Add(queryString + " AND\r\n");
                }
                // Mark
                if (mark != null)
                {
                    filters.Add("HLUKID >= ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = mark.UniqueID;
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
