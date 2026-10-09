using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public class MachinePartUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger ProcessLogger = LogManager.GetLogger("ProcessLog");

        public MachinePartUploadManager(BusinessSystemSession e1Session)
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
            var response = new ProcessResponse("Machine Part Consumption");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var sfManager = new MachinePartManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                // Set Eligible Extract Records
                var step1 = new ProcessStep("SF Set Transfer Status");
                try
                {
                    step1.SqlDmlResponses.Add(sfManager.SetTransferStatus(sqlCns["PLANT.INTEGRATION"], "N", "Q"));
                    step1.RowsAffected = step1.SqlDmlResponses.Last().RowsAffected;
                    response.RowsAffected = step1.RowsAffected;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                // Extract Records
                MachinePartConsumedItems items = new MachinePartConsumedItems();
                if (response.RowsAffected > 0 && !response.HasErrors())
                {
                    var step2 = new ProcessStep("SF Extract");
                    try
                    {
                        items = sfManager.Extract(session, sqlCns["PLANT.INTEGRATION"], plant, "Q");
                        step2.RowsAffected = items.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    response.AddStep(step2);
                }

                // Validate
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step3 = new ProcessStep("Validate");
                    try
                    {
                        var validationErrors = Validate(session, plant, items, f580030Mgr);
                        step3.Errors.AddRange(validationErrors);
                        step3.RowsAffected = items.Count(x => x.TransferStatus == "E");
                        if (validationErrors.Any())
                        {
                            var errorItems = items.Where(x => x.TransferStatus == "E").ToList();
                            response.SerializedObjects.Add("ValidationErrors", errorItems);
                            items.RemoveAll(x => x.TransferStatus == "E");
                            step3.SqlDmlResponses.AddRange(sfManager.SaveTransferProperties(errorItems, 100, sqlCns["PLANT.INTEGRATION"]));
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    step3.Done();
                    response.AddStep(step3);
                }

                // Get E1 Required Sequences
                long sfukid = 0;
                long sfedbt = 0;
                long qaukid = 0;
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Get Sequences");
                    try
                    {
                        sfukid = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", items.Count);
                        sfedbt = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
                        foreach (var item in items)
                        {
                            item.TransferId = sfukid;
                            item.TransferBatchNum = sfedbt;
                            sfukid++;
                        }
                        response.SerializedObjects.Add("MachinePartConsumedItems", items);
                        step4.RowsAffected = items.Count;
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    step4.Done();
                    response.AddStep(step4);
                }

                // Transform E1 Records
                F580030s f580030s = new F580030s();
                if (items.Count > 0 && !response.HasErrors())
                {
                    var step5 = new ProcessStep("Transform F580030 Records");
                    try
                    {
                        f580030s = sfManager.Transform(items);
                        response.SerializedObjects.Add("F580030s", f580030s);
                        step5.RowsAffected = f580030s.Count;
                    }
                    catch (Exception e)
                    {
                        step5.Errors.Add(new Error(e));
                    }
                    step5.Done();
                    response.AddStep(step5);
                }

                // Save to E1
                if (items.Count > 0)
                {
                    var step6 = new ProcessStep("Save F580030 Records");
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        step6.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        e1Trans.Commit();
                        step6.RowsAffected = f580030s.Count;
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        step6.Errors.Add(new Error(e));
                    }
                    step6.Done();
                    response.AddStep(step6);
                }

                // Submit E1 processing record
                if (items.Count > 0)
                {
                    var step7 = new ProcessStep("Save F580030A Record");
                    var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                        step7.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
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
                }

                // Update Shop Floor Completed
                if (items.Count > 0)
                {
                    var step8 = new ProcessStep("Set SF Transfer Properties");
                    try
                    {
                        step8.SqlDmlResponses.AddRange(sfManager.SaveTransferProperties(items, 100, sqlCns["PLANT.INTEGRATION"]));
                        step8.RowsAffected = items.Count;
                    }
                    catch (Exception e)
                    {
                        step8.Errors.Add(new Error(e));
                    }
                    step8.Done();
                    response.AddStep(step8);
                }

                // Summary Record
                if (items.Count >= 0)
                {
                    var step8a = new ProcessStep("Batch Summary Collection");
                    try
                    {
                        var headerSummary = new
                        {
                            Plant = plant.PlantNum,
                            PlantCode = plant.PlantCode,
                            BatchNum = items.Select(x => x.TransferBatchNum).FirstOrDefault(),
                            StartDate = response.StartDate,
                            EndDate = DateTime.Now,
                            ElapsedSeconds = (DateTime.Now - response.StartDate).Seconds
                        };
                        response.SerializedObjects.Add("Header", headerSummary, null);
                        step8a.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step8a.Errors.Add(new Error(e));
                    }
                    step8a.Done();
                    response.ProcessSteps.Add(step8a);
                }

                // Send Mail
                response.Done();
                if (items.Count > 0 || response.HasErrors())
                {
                    var step9 = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml,
                            "IGServices.Common.Xslt.MachinePartUploadReport.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        //Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        //    System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        //    System.Configuration.ConfigurationManager.AppSettings["MailMachinePartAddress"],
                        //    null,
                        //    plant.PlantCode + " [" +
                        //    System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " +
                        //    "Machine Part Upload ",
                        //    xslOutput);
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
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static Errors Validate(BusinessSystemSession session, Plant plant, MachinePartConsumedItems items, F580030Manager mgr)
        {
            var errors = new Errors();
            foreach (var item in items)
            {
                if (mgr.Exists(session, plant.PlantNum, item.ConsumedId.ToString(), "HM"))
                {
                    item.TransferStatus = "E";
                    item.TransferMessage = "Previously Uploaded to F580030";
                    errors.Add(new Error($"MachinePartConsumedId: {item.ConsumedId} Previously Uploaded to F580030", false));
                }
            }
            return errors;
        }
    }
}
