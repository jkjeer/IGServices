using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class ConsumedStoceUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ConsumedStoceUploadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"GetF580200Single", GetF580200Single()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, SqlConnection cnInt, Plant plant, string status)
        {
            var response = new ProcessResponse("Consumed Stoce");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

            var sfManager = new ConsumedStoceItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("INT", cnInt.ConnectionString));

                var step1 = new ProcessStep("Set SF Consumed Stoce Records");
                try
                {
                    var dmlResponse = sfManager.SetTransferStatus(cn, "N", "Q");
                    step1.SqlDmlResponses.Add(dmlResponse);
                    step1.RowsAffected = dmlResponse.RowsAffected;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                var items = new ConsumedStoceItems();
                var step2 = new ProcessStep("Get SF Consumed Stoce Records");
                try
                {
                    items = sfManager.Extract(session, cn, plant, status);
                    step2.RowsAffected = items.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                if (items.Count > 0)
                {
                    var step3 = new ProcessStep("Validate");
                    try
                    {
                        Validate(session, plant, items, f580030Mgr);
                        step3.RowsAffected = items.Count(x => x.TransferStatus == "E");
                        var errorItems = items.Where(x => x.TransferStatus == "E").ToList();
                        if (errorItems.Any())
                        {
                            response.SerializedObjects.Add(new SerializedObject("ValidationErrors",
                                XmlSerializationUtility.SerializeObject(Encoding.UTF8, errorItems)));
                            //foreach (var e in errorItems)
                            //{
                            //    step3.SerializedObjects.Add(new SerializedObject("Validation Error", $"StoceID: {e.GLStockID} {e.TransferMessage}"));
                            //}
                            items.RemoveAll(x => x.TransferStatus == "E");
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    step3.Done();
                    response.AddStep(step3);

                    if (items.Count > 0)
                    {
                        var step4 = new ProcessStep("Get Sequences");
                        long sfedbt = 0;
                        decimal qaukid = 0;
                        try
                        {
                            var sfukid = SequenceManager.GetRange(cnInt, "E1.SFUKID", items.Count);
                            sfedbt = SequenceManager.GetNextVal(cnInt, "E1.SFEDBT");
                            qaukid = SequenceManager.GetNextVal(cnInt, "E1.QAUKID");
                            foreach (var item in items)
                            {
                                item.TransferId = sfukid;
                                item.TransferBatchId = sfedbt;
                                sfukid++;
                            }
                            step4.RowsAffected = items.Count;
                            response.SerializedObjects.Add(new SerializedObject("ConsumedGlassItems",
                                XmlSerializationUtility.SerializeObject(Encoding.UTF8, items)));
                        }
                        catch (Exception e)
                        {
                            step4.Errors.Add(new Error(e));
                        }
                        step4.Done();
                        response.AddStep(step4);

                        var step5 = new ProcessStep("Build F580030 Records");
                        var f580030s = new F580030s();
                        try
                        {
                            f580030s = sfManager.Transform(items);
                            step5.RowsAffected = f580030s.Count;
                            response.SerializedObjects.Add(new SerializedObject("F580030s",
                                XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030s)));
                        }
                        catch (Exception e)
                        {
                            step5.Errors.Add(new Error(e));
                        }
                        step5.Done();
                        response.AddStep(step5);

                        var step6 = new ProcessStep("Insert F580030 to E1");
                        var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                        try
                        {
                            response.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                            e1Trans.Commit();
                            response.RowsAffected = f580030s.Count;
                            step6.RowsAffected = f580030s.Count;
                        }
                        catch (Exception e)
                        {
                            e1Trans.Rollback();
                            step6.Errors.Add(new Error(e));
                        }
                        step6.Done();
                        response.AddStep(step6);


                        if (!step6.HasErrors())
                        {
                            var step7 = new ProcessStep("Insert F580030A to E1");
                            var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                            var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                            try
                            {
                                response.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
                                e1Trans1.Commit();
                                step7.RowsAffected = 1;
                            }
                            catch (Exception e)
                            {
                                e1Trans1.Rollback();
                                step7.Errors.Add(new Error(e));
                            }
                            step7.Done();
                            response.AddStep(step7);

                            var step8 = new ProcessStep("Save SF Transfer Properties");
                            try
                            {
                                response.SqlDmlResponses.AddRange(sfManager.SaveTransferProperties(items, 100, cn));
                                step8.RowsAffected = items.Count;
                            }
                            catch (Exception e)
                            {
                                step8.Errors.Add(new Error(e));
                            }
                            step8.Done();
                            response.AddStep(step8);
                        }
                    }

                    var step9 = new ProcessStep("Send Mail Receipt");
                    try
                    {
                        XElement root = new XElement("root", XElement.Load(new StringReader(XmlSerializationUtility.SerializeObject(Encoding.UTF8, response))));
                        foreach (var serializedObject in response.SerializedObjects)
                        {
                            root.Add(XElement.Load(new StringReader(serializedObject.Value)));
                        }
                        var serializedXml = root.ToString();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.ConsumedStoceUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var subject = plant.PlantCode + " [" + System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " + "Consumed Stoce Upload ";

                        Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            System.Configuration.ConfigurationManager.AppSettings["MailBizToAddress"],
                            null,
                            subject,
                            xslOutput);
                        //plant.PlantCode + " [" + System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " + "Consumed Stoce Upload ",
                    }
                    catch (Exception e)
                    {
                        step9.Errors.Add(new Error(e));
                    }
                    step9.Done();
                    response.AddStep(step9);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            response.Close();
            return response;
        }

        public void Validate(BusinessSystemSession session, Plant plant, ConsumedStoceItems items, F580030Manager mgr)
        {
            foreach (var item in items)
            {
                if (mgr.Exists(session, plant.PlantNum, item.GLStockID.ToString(), "GS"))
                {
                    item.Transferred = true;
                    item.TransferStatus = "E";
                    item.TransferMessage = "Previously Uploaded SFZ58UID to F580030";
                }
                //if (mgr.UKIDExists(session, item.TransferId))
                //{
                //    item.Transferred = true;
                //    item.TransferStatus = "E";
                //    item.TransferMessage = "Previously Uploaded SFUKID to F580030";
                //}
            }
        }
    }
}
