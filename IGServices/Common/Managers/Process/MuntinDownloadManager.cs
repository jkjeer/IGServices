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

namespace IGServices.Common.Managers.Process
{
    public class MuntinDownloadManager
    {
        public MuntinDownloadManager(BusinessSystemSession session)
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
            var response = new ProcessResponse("MTO Spec Unit Download Manager");
            var table1 = new MuntinBarCodeTable();
            var table2 = new MuntinBarItemTable();

            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "Item") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan,
                    E1TimeManager.GetE1TimeStamp(session));
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
                var step2 = new ProcessStep("Get Items");
                try
                {
                    var itemManager = new MuntinBarItemManager(session);
                    var items = itemManager.GetBarItems(session, plant, processRunSpan, itemNum);
                    step2.RowsAffected = items.Count;
                    table1.Add(items);
                    table2.Add(items);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }
            }

            if (table1.Rows.Count > 0)
            { 
                /********************************************************/
                var step3 = new ProcessStep("Bulk Copy Tables");
                try
                {
                    var bulkreturnResponse = BulkCopyManager.BulkTable(table1.ExtendedProperties["Target"].ToString(), table1,
                        sqlCns["PLANT.INTEGRATION"]);
                    step3.SqlBulkCopyResponses.Add(bulkreturnResponse);
                    var bulkResponse2 = BulkCopyManager.BulkTable(table2.ExtendedProperties["Target"].ToString(), table2,
                        sqlCns["PLANT.INTEGRATION"]);
                    step3.SqlBulkCopyResponses.Add(bulkResponse2);
                    step3.RowsAffected = (int)bulkResponse2.RowsCopied;
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                finally
                {
                    step3.Done();
                    response.RowsAffected = step3.RowsAffected;
                    response.AddStep(step3);
                }

                /********************************************************/
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table1.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table1.MaxDate;
                            highWaterMark.EnterpriseDateTime = table1.MaxDate.AddHours(-plant.TimeOffset);
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
    }
}
