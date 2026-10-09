using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
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
    public class RemakeUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RemakeUploadManager(BusinessSystemSession e1Session)
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

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {

            var response = new ProcessResponse("Remake Upload Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var sfManager = new RemakeItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            var items = new RemakeItems();
            try
            {

                var step1 = new ProcessStep("Get Shop Floor records");
                ProcessRunSpan processRunSpan = new ProcessRunSpan();
                HighWaterMark highWaterMark = new HighWaterMark();
                try
                {
                    items = sfManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "N");
                    response.RowsAffected = items.Count;
                    step1.RowsAffected = items.Count;
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

                if (items.Count > 0)
                {
                    var step2 = new ProcessStep("Validate");
                    try
                    {
                        Validate(session, plant, items, f580030Mgr);
                        var invalidItems = items.Where(x => x.TransferStatus == "E").ToList();
                        //invalidItemsList = invalidItems as IList<RemakeItem> ?? invalidItems.ToList();
                        step2.RowsAffected = invalidItems.Count();
                        if (invalidItems.Any())
                        {
                            items.RemoveAll(x => x.TransferStatus == "E");
                            step2.Messages.Add(new Message("Validations", string.Join(",", invalidItems.Select(x=>x.E1RemakeId).ToList())));
                        }
                        response.SerializedObjects.Add(new SerializedObject("ValidationItems",
                            XmlSerializationUtility.SerializeObject(Encoding.UTF8, invalidItems)));

                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2.Done();
                        response.AddStep(step2);
                    }
                }

                if (items.Count > 0)
                {
                    var step3 = new ProcessStep("Sequences");
                    var sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", items.Count);
                    var f580030As = new F580030As();
                    var batchGroups = items.GroupBy(x => new {x.CustName, x.OriginalScheduleNum});
                    try
                    {
                        foreach (var batchGroup in batchGroups)
                        {
                            var sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                            var qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                            f580030As.Add(f580030AMgr.Create("RM", qaukid, sfedbt, plant.PlantNum));
                            foreach (var item in batchGroup)
                            {
                                item.TransferId = sfukid;
                                item.TransferBatchId = sfedbt;
                                sfukid++;
                            }
                        }

                        response.SerializedObjects.Add(new SerializedObject("RemakeItems",
                            XmlSerializationUtility.SerializeObject(Encoding.UTF8, items)));
                        response.SerializedObjects.Add(new SerializedObject("f580030As",
                            XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030As)));
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.Done();
                        response.AddStep(step3);
                    }

                    var step4 = new ProcessStep("Build F580030");
                    var f580030s = new F580030s();
                    try
                    {
                        f580030s = sfManager.Transform(items);
                        step4.RowsAffected = f580030s.Count;
                        response.SerializedObjects.Add(new SerializedObject("F580030s",
                            XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030s)));
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

                    var step5 = new ProcessStep("Save F580030");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        step5.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        e1Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        step5.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step5.Done();
                        response.AddStep(step5);
                    }

                    var step7 = new ProcessStep("Save F580030A");
                    var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        foreach (var f580030A in f580030As)
                        {
                            step7.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
                        }

                        e1Trans1.Commit();
                    }
                    catch (Exception e)
                    {
                        e1Trans1.Rollback();
                        step7.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step7.Done();
                        response.AddStep(step7);
                    }

                    var step8 = new ProcessStep("ShopFloor Status update");
                    try
                    {
                        step8.SqlDmlResponses.AddRange(
                            sfManager.SaveTransferProperties(items, 100, sqlCns["PLANT.SHOPFLOOR"]));
                    }
                    catch (Exception e)
                    {
                        step8.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step8.Done();
                        response.AddStep(step8);
                    }


                    var step9 = new ProcessStep("Serialized Collections");
                    try
                    {
                        var headerSummary = new
                        {
                            PlantCode = plant.PlantCode,
                            StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                            ElapsedSeconds = (DateTime.Now - response.StartDate).TotalSeconds,
                            Lines = items.Count,
                            Batches = items.GroupBy(x => x.TransferBatchId).Count()
                        };
                        response.SerializedObjects.Add("RemakeSummary", headerSummary, null);
                    }
                    catch (Exception e)
                    {
                        step9.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step9.Done();
                        response.AddStep(step9);
                    }

                    var step10 = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "IGServices.Common.Xslt.RemakeUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "REMAKE_UPLOAD", "ADMIN",
                            null, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "REMAKE_UPLOAD", "PUBLIC",
                            plant.PlantCode, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "REMAKE_UPLOAD", "USER",
                            plant.PlantCode, null));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            $"{(response.HasErrors() ? "ERROR - " : String.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Remake Upload",
                            xslOutput);
                    }
                    catch (Exception e)
                    {
                        step10.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step10.Done();
                        response.AddStep(step10);
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        public static void Validate(BusinessSystemSession session, Plant plant, RemakeItems items, F580030Manager mgr)
        {
            foreach (var item in items)
            {
                //if (mgr.Exists(session, plant.PlantNum, item.E1RemakeId.ToString(), "RM"))
                //{
                //    item.Transferred = true;
                //    item.TransferStatus = "E";
                //    item.TransferMessage = "RM Previously Uploaded to F580030";
                //}
            }
        }
    }
}
