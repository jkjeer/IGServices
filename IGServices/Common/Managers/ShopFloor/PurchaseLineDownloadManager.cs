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
    public class PurchaseLineDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public PurchaseLineDownloadManager(BusinessSystemSession session)
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

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark, decimal orderNum, string orderType)
        {
            var response = new ProcessResponse("Purchase Order Line Download");
            var orderTable = new PurchaseLineTable();

            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F43199") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));

            //var rt1 = new ResponseTime("Get From E1");
            var reader = GetReader(session, cn, plant, processRunSpan, orderNum, orderType);
            //rt1.Done();
            //response.ResponseTimes.Add(rt1);
            orderTable.Add(reader);
            if (orderTable.Rows.Count > 0)
            {
                // Bulk Copy
                var woBulkResponse = BulkCopyManager.BulkTable(orderTable.ExtendedProperties["Target"].ToString(), orderTable, cn);
                response.SqlBulkCopyResponses.Add(woBulkResponse);
                // Set High Water Mark
                if (mark != null)
                {
                    mark.UniqueID = (long)orderTable.MaxId;
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
	                    OLUKID,
	                    OLMCU,
	                    OLDOCO,
	                    OLDCTO,
	                    OLLNID,
	                    OLKCOO,
	                    OLAN8,
	                    OLSHAN,
	                    OLTRDJ,
	                    OLDRQJ,
	                    OLCNDJ,
                        OLADDJ,
	                    OLITM,
                        OLLOTN,
	                    OLLNTY,
	                    OLNXTR,
	                    OLLTTR,
	                    OLUORG,
	                    OLUREC,
	                    OLPRRC,
	                    OLAEXP,
	                    OLGLC,
	                    OLUPMJ,
	                    OLTDAY,
                        IBSTKT,
                        CMCFGID
                    FROM 
	                    F43199
                        INNER JOIN F4102 
                        ON IBMCU = OLMCU AND IBITM = OLITM
                        LEFT OUTER JOIN F3201
                        ON  CMDOCO = OLDOCO AND
                            CMDCTO = OLDCTO AND
                            CMLNID = OLLNID AND
                            CMKCOO = OLKCOO AND
                            CMPID != 'P580150'
                    WHERE
                        {filterString} 
	                    OLGLC IN ('GLAS','GLAF','GLAX','GLAZ')"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal orderNum, string orderType)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!string.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("OLMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }
                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("OLDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }
                // Order Type
                if (!string.IsNullOrEmpty(orderType))
                {
                    filters.Add("OLDCTO = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                    parameters.Last().Value = orderType;
                }
                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("OLUPMJ", "OLTDAY", span.E1TimeIdSpan);
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
