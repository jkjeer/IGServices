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
    public class RackEventDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackEventDownloadManager(BusinessSystemSession session)
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

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackId)
        {
            var response = new ProcessResponse("Rack Event Download");
            var table = new RackEventTable();
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

                var step1 = new ProcessStep("E1 Extract F580559");
                try
                {
                    step1.Messages.Add(new Message("Span", span.Serialize()));
                    var reader = GetReader(session, cn, plant, span, rackId);
                    table.Add(reader);
                    step1.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);


                if (table.Rows.Count > 0)
                {

                    var step2 = new ProcessStep("SF Download");
                    try
                    {
                        var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                        if (bulkResponse.HasErrors())
                        {
                            step2.Errors.AddRange(bulkResponse.Errors);
                        }
                        response.SqlBulkCopyResponses.Add(bulkResponse);
                        step2.RowsAffected = (int)response.SqlBulkCopyResponses.Last().RowsCopied;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    response.AddStep(step2);

                    if (span.HighWaterMark.Exists)
                    {
                        var step3 = new ProcessStep("SF High Water Mark");
                        try
                        {
                            span.HighWaterMark.EnterpriseDateTime = table.MaxDate;
                            span.HighWaterMark.UniqueID = (long)table.MaxId;
                            response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                            step3.RowsAffected = response.SqlDmlResponses.Last().RowsAffected;
                        }
                        catch (Exception e)
                        {
                            step3.Errors.Add(new Error(e));
                        }
                        step3.Done();
                        response.AddStep(step3);
                    }
                }
            }
            catch (ApplicationException e)
            {
                Logger.Error($"ERROR: {cn.ConnectionString}  {e.Message}");
                response.Errors.Add(new Error($"ERROR: {cn.ConnectionString}  {e.Message}"));
                throw new ApplicationException(e.Message);
            }
            response.Close();
            return response;
        }

        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
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
                        {filterString} HLVERS != 'DELETED'
                    ORDER BY
                        HLUKID
                    FOR READ ONLY"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackId)
        {
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {

                // Plant - Required
                filters.Add("HLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Rack Id
                if (rackId > 0)
                {
                    filters.Add("HLZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackId;
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("HLUPMJ", "HLTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND\r\n");
                }

                var cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
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
