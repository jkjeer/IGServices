using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
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
    public class SpacerItemDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SpacerItemDownloadManager(BusinessSystemSession session)
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

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            var response = new ProcessResponse("Spacer Item Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "SpacerItem") : null;
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

            /********************************************************/
            var table = new SpacerItemTable();
            var step2 = new ProcessStep("Get From E1");
            try
            {
                var spacerItemManager = new Managers.Enterprise.SpacerItemManager(session);
                var items = spacerItemManager.GetSpacerItemKeys(session, plant, processRunSpan, itemNum);
                table.Add(items);
                step2.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step2.Errors.Add(new Error(e));
            }
            step2.Done();
            response.ProcessSteps.Add(step2);

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
                    step3.RowsAffected = (int) bulkResponse.RowsCopied;
                    step3.SqlBulkCopyResponses.Add(bulkResponse);
                    step3.Done();
                    response.RowsAffected = step3.RowsAffected;
                    response.AddStep(step3);
                }
            }

            /********************************************************/
            if (useHighWaterMark && !response.HasErrors())
            {
                var step4 = new ProcessStep("Update High Water Mark");
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        highWaterMark.LocalDateTime = table.MaxDate;
                        highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
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
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }
    }
}
