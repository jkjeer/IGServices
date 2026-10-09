using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
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
    public class PurchasedLotDownloadManager
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
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns,
            Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Purchased Lot Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            DateTime businessSystemTime = new DateTime();

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            ProcessRunSpan processRunSpanReceipt = new ProcessRunSpan();
            HighWaterMark highWaterMarkReceipt = new HighWaterMark();

            ProcessRunSpan processRunSpanLoad = new ProcessRunSpan();
            HighWaterMark highWaterMarkLoad = new HighWaterMark();


            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                businessSystemTime = session.GetCurrentTimestamp().AddMinutes(-1);
                step1.Messages.Add(new Message("Business System Timestamp", businessSystemTime.ToString()));

                highWaterMark = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "PurchasedLots")
                    : null;
                processRunSpan =
                    ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, businessSystemTime);
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));

                highWaterMarkReceipt = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "PurchasedLotReceipt")
                    : null;
                processRunSpanReceipt =
                    ProcessRunSpanManager.GetRunSpan(highWaterMarkReceipt, timeSpan, businessSystemTime);
                step1.Messages.Add(new Message("RunSpanReceipt", processRunSpanReceipt.Serialize()));

                highWaterMarkLoad = (useHighWaterMark)
                    ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "PurchasedLotLoad")
                    : null;
                processRunSpanLoad =
                    ProcessRunSpanManager.GetRunSpan(highWaterMarkLoad, timeSpan, businessSystemTime);
                step1.Messages.Add(new Message("RunSpanLoad", processRunSpanLoad.Serialize()));


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
                var step2 = new ProcessStep("Get Purchased Lots Data");
                var lotMgr = new PurchasedLotsManager(session);
                var lots = new PurchasedLots();
                try
                {
                    lots = lotMgr.GetPurchasedLots(session, plant, processRunSpan);
                    step2.RowsAffected = lots.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                var dt1 = new PurchasedLotTable();
                var dt2 = new PurchasedLotWorkOrderTable();
                var dt3 = new PurchasedLotSalesOrderTable();
                if (lots.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponses = new SqlBulkCopyResponses();
                    try
                    {
                        dt1.Add(lots, plant);
                        if (dt1.Rows.Count > 0)
                        {
                            response.RowsAffected += dt1.Rows.Count;
                            bulkResponses.Add(BulkCopyManager.BulkTable(dt1.ExtendedProperties["Target"].ToString(),
                                dt1, sqlCns["PLANT.INTEGRATION"]));
                        }
                        dt2.Add(lots, plant);
                        if (dt2.Rows.Count > 0)
                        {
                            response.RowsAffected += dt2.Rows.Count;
                            bulkResponses.Add(BulkCopyManager.BulkTable(dt2.ExtendedProperties["Target"].ToString(),
                                dt2, sqlCns["PLANT.INTEGRATION"]));
                        }
                        dt3.Add(lots, plant);
                        if (dt3.Rows.Count > 0)
                        {
                            response.RowsAffected += dt3.Rows.Count;
                            bulkResponses.Add(BulkCopyManager.BulkTable(dt3.ExtendedProperties["Target"].ToString(),
                                dt3, sqlCns["PLANT.INTEGRATION"]));
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.SqlBulkCopyResponses.AddRange(bulkResponses);
                        step3.Done();
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                var lotLoads = new PurchasedLots();
                var step4 = new ProcessStep("Get Purchased Lot Load Data");
                try
                {
                    lotLoads = lotMgr.GetPurchasedLotLoads(session, plant, processRunSpanLoad);
                    step4.RowsAffected = lotLoads.Count;
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                step4.Done();
                response.AddStep(step4);

                /********************************************************/
                var dt5 = new PurchasedLotLoadTable();
                if (lotLoads.Count > 0)
                {
                    var step4a = new ProcessStep("Bulk Copy");
                    var bulkResponses = new SqlBulkCopyResponses();
                    try
                    {
                        dt5.Add(lotLoads, plant);
                        if (dt5.Rows.Count > 0)
                        {
                            response.RowsAffected += dt5.Rows.Count;
                            bulkResponses.Add(BulkCopyManager.BulkTable(dt5.ExtendedProperties["Target"].ToString(),
                                dt5, sqlCns["PLANT.INTEGRATION"]));
                        }
                    }
                    catch (Exception e)
                    {
                        step4a.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4a.SqlBulkCopyResponses.AddRange(bulkResponses);
                        step4a.Done();
                        response.AddStep(step4a);
                    }
                }

                /********************************************************/
                var lotReceipts = new PurchasedLots();
                var step5 = new ProcessStep("Get Purchased Lot Receipts Data");
                try
                {
                    lotReceipts = lotMgr.GetPurchasedLotReceipts(session, plant, processRunSpanReceipt);
                    step5.RowsAffected = lotReceipts.Count;
                }
                catch (Exception e)
                {
                    step5.Errors.Add(new Error(e));
                }
                step5.Done();
                response.AddStep(step5);

                /********************************************************/
                var dt6 = new PurchasedLotReceiptTable();
                if (lotReceipts.Count > 0)
                {
                    var step5a = new ProcessStep("Bulk Copy");
                    var bulkResponses = new SqlBulkCopyResponses();
                    try
                    {
                        dt6.Add(lotReceipts, plant);
                        if (dt6.Rows.Count > 0)
                        {
                            response.RowsAffected += dt6.Rows.Count;
                            bulkResponses.Add(BulkCopyManager.BulkTable(dt6.ExtendedProperties["Target"].ToString(),
                                dt6, sqlCns["PLANT.INTEGRATION"]));
                            step5a.RowsAffected = dt6.Rows.Count;
                        }
                    }
                    catch (Exception e)
                    {
                        step5a.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step5a.SqlBulkCopyResponses.AddRange(bulkResponses);
                        step5a.Done();
                        response.AddStep(step5a);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors())
                {
                    var step6 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (dt1.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = dt1.MaxDate;
                            highWaterMark.EnterpriseDateTime = dt1.MaxDate.AddHours(-plant.TimeOffset);
                            highWaterMark.UniqueID = (dt1.MaxId > highWaterMark.UniqueID)
                                ? (long) dt1.MaxId
                                : highWaterMark.UniqueID;
                            step6.SqlDmlResponses.Add(
                                HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                        }
                        if (dt5.Rows.Count > 0)
                        {
                            highWaterMarkLoad.LocalDateTime = dt5.MaxDate;
                            highWaterMarkLoad.EnterpriseDateTime = dt5.MaxDate.AddHours(-plant.TimeOffset);
                            step6.SqlDmlResponses.Add(
                                HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMarkLoad));
                        }
                        if (dt6.Rows.Count > 0)
                        {
                            highWaterMarkReceipt.LocalDateTime = dt6.MaxDate;
                            highWaterMarkReceipt.EnterpriseDateTime = dt6.MaxDate.AddHours(-plant.TimeOffset);
                            step6.SqlDmlResponses.Add(
                                HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMarkReceipt));
                        }

                    }
                    catch (Exception e)
                    {
                        step6.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step6.Done();
                        response.AddStep(step6);
                    }
                }
            }
            response.Done();
            Logger.Trace(response.Serialize);
            Logging.InsertProcessLog(response, false);
            return response;
        }

    }
}
