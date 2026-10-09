using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGGServices.Common.Types.Enterprise;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using IGServices.ScheduleSvc;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class MTSScheduleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttributesManager;

        /*****************************************************************************
        *   CONSTRUCTOR
        ******************************************************************************/
        public MTSScheduleDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"GetF580200Single", GetF580200Single()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttributesManager = new E1.ItemAttributeManager(session);
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ScheduleEvent evt, bool force)
        {
            var response = new ProcessResponse("MTS Schedule Download");
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                foreach (var sqlCn in sqlCns)
                {
                    response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                }

                var schedule = new Schedule();
                var f5834A01Mgr = new F5834A01Manager(session);

                /********************************************************/
                var stepHeaderInUse = new ProcessStep("Set E1 Header Status - In Use");
                try
                {
                    var dmlResponse = ScheduleEventManager.UpdateBeforeStatus(session, evt);
                    stepHeaderInUse.SqlDmlResponses.Add(dmlResponse);
                    stepHeaderInUse.RowsAffected = dmlResponse.RowsAffected;
                    ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "U", "E1 In Use");
                }
                catch (Exception e)
                {
                    stepHeaderInUse.Errors.Add(new Error(e));
                }
                stepHeaderInUse.Done();
                response.AddStep(stepHeaderInUse);

                /********************************************************/
                var stepF580020 = new ProcessStep("E1 Schedule - F580020");
                try
                {
                    schedule.Plant = plant;
                    schedule.ScheduleEvent = evt;
                    schedule.ScheduleNum = evt.ScheduleNum;
                    schedule.ScheduleDateE1 = evt.ScheduleDateE1;
                    schedule.f5834A01 = f5834A01Mgr.GetSchedule(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleDateE1);
                    schedule.f580020 = F580020Manager.GetF580020(session, evt.PlantNum, evt.ScheduleNum,
                        evt.ScheduleDateE1, evt.BatchNum);
                    stepF580020.RowsAffected = (schedule.ScheduleEvent.ScheduleNum > 0) ? 1 : 0;
                    stepF580020.Messages.Add(new Message("Schedule Key", $"Plant {plant.PlantNum} ScheduleNum {schedule.ScheduleEvent.ScheduleNum} ScheduleDateE1 {schedule.ScheduleEvent.ScheduleDateE1} Batch {schedule.ScheduleEvent.BatchNum}"));
                }
                catch (Exception e)
                {
                    stepF580020.Errors.Add(new Error(e));
                }
                stepF580020.Done();
                response.AddStep(stepF580020);

                /********************************************************/
                var stepF580021 = new ProcessStep("E1 Schedule - F580021");
                try
                {
                    schedule.f580021s = F580021Manager.GetF580021s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.BatchNum);
                    stepF580021.RowsAffected = schedule.f580021s.Count;
                    response.RowsAffected = schedule.f580021s.Count;
                }
                catch (Exception e)
                {
                    stepF580021.Errors.Add(new Error(e));
                }
                stepF580021.Done();
                response.AddStep(stepF580021);

                /********************************************************/
                var stepF580024 = new ProcessStep("E1 Schedule - F580024");
                try
                {
                    schedule.f580024s = F580024Manager.GetF580024s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.BatchNum);
                    stepF580024.RowsAffected = schedule.f580024s.Count;
                }
                catch (Exception e)
                {
                    stepF580024.Errors.Add(new Error(e));
                }
                stepF580024.Done();
                response.AddStep(stepF580024);

                /********************************************************/
                var stepF580232 = new ProcessStep("E1 Schedule - F580232");
                try
                {
                    schedule.f580232s = F580232Manager.GetF580232(session, plant.PlantNum, schedule.ScheduleNum);
                    stepF580232.RowsAffected = schedule.f580232s.Count;
                }
                catch (Exception e)
                {
                    stepF580232.Errors.Add(new Error(e));
                }
                stepF580232.Done();
                response.AddStep(stepF580232);

                /********************************************************/
                var stepBusinessValidate = new ProcessStep("Validate Business System");
                try
                {
                    var validationErrors = ValidateBusinessSystem(session, sqlCns["PLANT.CLEARPATH"], schedule);
                    if (validationErrors.Any())
                    {
                        stepBusinessValidate.Errors.AddRange(validationErrors);
                        stepBusinessValidate.RowsAffected = validationErrors.Count;
                    }
                }
                catch (Exception e)
                {
                    stepBusinessValidate.Errors.Add(new Error(e));
                }
                stepBusinessValidate.Done();
                response.AddStep(stepBusinessValidate);

                /********************************************************/
                var scheduleLines = new ScheduleLines();
                var stepScheduleLines = new ProcessStep("Schedule Line Collections");
                try
                {
                    foreach (var f580021 in schedule.f580021s)
                    {
                        var scheduleLine = new ScheduleLine();

                        // F580021
                        scheduleLine.F580021 = f580021;

                        // F580024
                        scheduleLine.F580024 =
                            schedule.f580024s?.Find(
                                item =>
                                    item.S8EDBT == f580021.S7EDBT && item.S8EDLN == f580021.S7EDLN &&
                                    item.S8Z58SCN == f580021.S7Z58SCN && item.S8Z58SCSN == f580021.S7Z58SCSN);
                        var s7Rorn = Data.ParseStringToDecimal(f580021.S7RORN);

                        // F580232
                        scheduleLine.F580232 =
                            schedule.f580232s?.Find(
                                item =>
                                    item.BDZ58WODC == f580021.S7DOCO);


                        // F3211
                        scheduleLine.ItemAttributes = _itemAttributesManager.GetDiscreteItemAttributes(session, f580021.S7ITM);

                        // Schedule Line First Class Objects
                        scheduleLine.BatchNum = scheduleLine.F580232?.BDZ58BNUM ?? 0;
                        scheduleLine.BatchName = scheduleLine.F580232?.BHZ58BDES ?? "";
                        scheduleLine.BatchGroupName = scheduleLine.F580232?.BHZ58GDES ?? "";
                        scheduleLine.BatchDynamicName = scheduleLine.F580232?.BHDS01 ?? "";

                        scheduleLines.Add(scheduleLine);
                    }
                    schedule.ScheduleLines = scheduleLines;
                    stepScheduleLines.RowsAffected = scheduleLines.Count;
                }
                catch (Exception e)
                {
                    stepScheduleLines.Errors.Add(new Error(e));
                }
                stepScheduleLines.Done();
                response.AddStep(stepScheduleLines);

                /********************************************************/
                if (!response.HasErrors())
                {
                    var stepBulkCopy = new ProcessStep("Bulk Schedule Tables");
                    var txn = sqlCns["PLANT.INTEGRATION"].BeginTransaction("OrderTables");
                    try
                    {
                        var orderTable = new ScheduleInfoMTSTable();
                        orderTable.Add(schedule);
                        stepBulkCopy.SqlBulkCopyResponses.Add(
                            BulkCopyManager.BulkTable(orderTable.ExtendedProperties["Target"].ToString(), orderTable, sqlCns["PLANT.INTEGRATION"], txn));
                        txn.Commit();
                        stepBulkCopy.RowsAffected = orderTable.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        txn.Rollback();
                        stepBulkCopy.Errors.Add(new Error(e));
                    }
                    stepBulkCopy.Done();
                    response.AddStep(stepBulkCopy);
                }

                /********************************************************/
                var stepHeaderFinal = new ProcessStep("Set E1 Header Status - Success/Failure");
                try
                {
                    SqlDmlResponse dmlResponse;
                    if (response.HasErrors())
                    {
                        dmlResponse = ScheduleEventManager.UpdateAfterFail(session, evt);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "N", "E1 Download Failed");
                    }
                    else
                    {
                        dmlResponse = ScheduleEventManager.UpdateAfterSuccess(session, evt);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "Y", "E1 Download Success");
                    }
                    stepHeaderFinal.SqlDmlResponses.Add(dmlResponse);
                    stepHeaderFinal.RowsAffected = dmlResponse.RowsAffected;
                }
                catch (Exception e)
                {
                    stepHeaderFinal.Errors.Add(new Error(e));
                }
                stepHeaderFinal.Done();
                response.AddStep(stepHeaderFinal);

                /********************************************************/
                // Summary
                var stepSummary = new ProcessStep("Schedule Summary Collections");
                try
                {
                    var scheduleHeaderSummary = new
                    {
                        Plant = plant.PlantCode,
                        ScheduleNum = schedule.ScheduleNum,
                        ScheduleId = schedule.ScheduleId,
                        ScheduleDate = E1TimeManager.CastToDateTime(schedule.ScheduleDateE1).ToString("MM/dd/yyyy"),
                        BatchNum = schedule.ScheduleEvent.BatchNum,
                        StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds =
                            (decimal)
                                (response.ProcessSteps.Max(x => x.EndDate) -
                                    response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add("ScheduleHeader", scheduleHeaderSummary, null);

                    var scheduleCustomerSummary =
                        schedule.ScheduleLines.GroupBy(x => new { x.F580021.S7AN8DR, x.F580021.S7DOCO })
                            .Select(
                                ng =>
                                    new
                                    {
                                        Customer = ng.Key.S7AN8DR,
                                        CustomerName = ng.Select(x => x.F580021.S7ALPH).FirstOrDefault()?.Trim(),
                                        WorkOrderNum = ng.Key.S7DOCO,
                                        ScheduleLines = ng.Count(),
                                        SalesOrders = ng.Select(x => x.F580021.S7RORN).Distinct().Count(),
                                        WorkOrders = ng.Select(x => x.F580021.S7DOCO).Distinct().Count(),
                                        MonoPickUnits = ng.Where(x => x.F580021.S7Z58L1RG == "K" && x.F580021.S7Z58L2TK == 0).Sum(x => x.F580021.S7Z58QTY),
                                        MonoCutUnits = ng.Where(x => x.F580021.S7Z58L1RG.Contains("C") && x.F580021.S7Z58L2TK == 0).Sum(x => x.F580021.S7Z58QTY),
                                        IG2PickUnits = ng.Where(x => x.F580021.S7Z58L2TK > 0 && x.F580021.S7Z58L3TK == 0 && x.F580021.S7Z58L1RG == "K" && x.F580021.S7Z58L2RG == "K").Sum(x => x.F580021.S7Z58QTY),
                                        IG2LineUnits = ng.Where(x => x.F580021.S7Z58L2TK > 0 && x.F580021.S7Z58L3TK == 0 && (x.F580021.S7Z58L1RG != "K" || x.F580021.S7Z58L2RG != "K")).Sum(x => x.F580021.S7Z58QTY),
                                        IG3PickUnits = ng.Where(x => x.F580021.S7Z58L3TK > 0 && x.F580021.S7Z58L1RG == "K" && x.F580021.S7Z58L2RG == "K" && x.F580021.S7Z58L3RG == "K").Sum(x => x.F580021.S7Z58QTY),
                                        IG3LineUnits = ng.Where(x => x.F580021.S7Z58L3TK > 0 && (x.F580021.S7Z58L1RG != "K" || x.F580021.S7Z58L2RG != "K" || x.F580021.S7Z58L3RG != "K")).Sum(x => x.F580021.S7Z58QTY),
                                        TotalUnits = ng.Sum(x => x.F580021.S7Z58QTY),
                                        MinRequestedDate =
                                            ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Min().ToString("MM/dd/yyyy"),
                                        MaxRequestedDate =
                                            ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Max().ToString("MM/dd/yyyy")
                                    })
                            .ToList();
                    response.SerializedObjects.Add("CustomerSummary", scheduleCustomerSummary, "CustomerLines", "CustomerLine");
                    stepSummary.RowsAffected = scheduleCustomerSummary.Count;
                }
                catch (Exception e)
                {
                    stepSummary.Errors.Add(new Error(e));
                }
                stepSummary.Done();
                response.AddStep(stepSummary);

                /********************************************************/
                // Send Mail
                Logger.Info("Serialized Process: " + XmlSerializationUtility.SerializeObject(Encoding.UTF8, response));
                if (schedule.ScheduleEvent.ScheduleNum > 0 || response.HasErrors() || response.SerializedObjects.Exists(x => x.Name == "ValidationErrors"))
                {
                    var stepMail = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.ScheduleDownloadReportMTS.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var subject = $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule {schedule.ScheduleNum} Download";

                        //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                        //Directory.CreateDirectory(plantPathName);
                        //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                        //File.Delete(fileName);
                        //File.WriteAllText(fileName, xslOutput);

                        Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE", "ADMIN", null, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE", "PUBLIC", plant.PlantCode, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE", "USER", plant.PlantCode, null));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            subject,
                            xslOutput);
                        //$"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule {schedule.ScheduleNum} Download",
                    }
                    catch (Exception e)
                    {
                        stepMail.Errors.Add(new Error(e));
                    }
                    stepMail.Done();
                    response.AddStep(stepMail);
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            response.Close();
            return response;
        }

        /*****************************************************************************
        *   VALIDATE BUSINESS SYSTEM
        ******************************************************************************/
        public Errors ValidateBusinessSystem(BusinessSystemSession session, SqlConnection cnCp, Schedule schedule)
        {
            var errors = new Errors();
            try
            {
                if (schedule.f580021s.All(x => x.S7EDBT == null))
                {
                    errors.Add(new Error("No F580021 records found in E1"));
                }
                if (schedule.f580024s.All(x => x.S8EDBT == null))
                {
                    errors.Add(new Error("No F580024 records found in E1"));
                }
                if (schedule.f580021s.Count(x => x.S7EDBT != null) != schedule.f580024s.Count(x => x.S8EDBT != null))
                {
                    errors.Add(new Error($"F580021 record count {schedule.f580021s.Count(x => x.S7EDBT != null)} does not match F580024 record count {schedule.f580024s.Count(x => x.S8EDBT != null)} in E1"));
                }
                if (schedule.f580021s.Any(x => x.S7Z58SCDDJ == 0))
                {
                    errors.Add(new Error(schedule.f580021s.Count(x => x.S7Z58SCDDJ == 0) + " Work Orders on this MTS Schedule have a missing Due Date (S7Z58SCDDJ) in E1"));
                    var lines = schedule.f580021s.Where(x => x.S7Z58SCDDJ == 0);
                    errors.AddRange(lines.Select(line => new Error($"Work Order {line.S7DOCO}")));
                }
                if (schedule.f580021s.Any(x => x.S7Z58QTY == 0))
                {
                    errors.Add(new Error(schedule.f580021s.Count(x => x.S7Z58QTY == 0) + " Work Orders on this MTS Schedule have Zero Quantity (S7Z58QTY) E1"));
                    var lines = schedule.f580021s.Where(x => x.S7Z58QTY == 0);
                    errors.AddRange(lines.Select(line => new Error($"Work Order {line.S7DOCO}")));
                }
                if (schedule.f580021s.Any(x => Data.ParseStringToDecimal(x.WASRST) > 92))
                {
                    errors.Add(new Error("Work Orders on this MTO Schedule already have Status > 92 in E1"));
                    var lines = schedule.f580021s.Where(x => Data.ParseStringToDecimal(x.WASRST) > 92);
                    errors.AddRange(lines.Select(line => new Error($"Work Order {line.S7DOCO} Status {line.WASRST}")));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return errors;
        }


        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force, ServiceContext serviceContext, bool parallel, ProcessResponse responseStub = null)
        {
            var response = new ProcessResponse($@"Download MTS Incoming Orders ({plant.PlantCode}) Schedule {scheduleNum} {(parallel ? "Parallel" : "Synchronous")}", true);
            if (responseStub != null)
            {
                response.ProcessSteps.AddRange(responseStub.ProcessSteps);
            }
            response.ServiceContext = serviceContext ?? response.ServiceContext;
            response.Plant = plant;
            response.IsParallel = parallel;
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    mtsSchedule.TransferStatus = "N";
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                if (!step1GetMTSSchedule.HasErrors())
                {
                    var step2Validate = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Validate Schedule");
                    try
                    {
                        mtsSchedule.ValidationErrors = MTSScheduleManager.Validate(mtsSchedule);
                        step2Validate.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        step2Validate.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2Validate.Done();
                        response.ProcessSteps.Add(step2Validate);
                    }
                }

                if ((!mtsSchedule.ValidationErrors.Any()) || force && !mtsSchedule.IsFatal())
                {
                    var step3UpdateBeforeProcessingF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update Before Processing F580020");
                    try
                    {
                        step3UpdateBeforeProcessingF580020.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        step3UpdateBeforeProcessingF580020.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3UpdateBeforeProcessingF580020.Done();
                        response.ProcessSteps.Add(step3UpdateBeforeProcessingF580020);
                    }

                    if ((!step3UpdateBeforeProcessingF580020.HasErrors() && mtsSchedule.IsValid()) || force && !mtsSchedule.IsFatal())
                    {
                        var step4DownloadIncomingOrdersTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download ScheduleInfoMTS Table");
                        try
                        {
                            response.RowsAffected = DownloadScheduleInfoMTSTable(cnSFInt, mtsSchedule.MTSIncomingOrderLines);
                            step4DownloadIncomingOrdersTable.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        }
                        catch (Exception e)
                        {
                            step4DownloadIncomingOrdersTable.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4DownloadIncomingOrdersTable.Done();
                            response.ProcessSteps.Add(step4DownloadIncomingOrdersTable);
                        }

                        if (!step4DownloadIncomingOrdersTable.HasErrors())
                        {
                            var step6UpdateAfterProcessingSuccessF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update After Processing Success F580020");
                            try
                            {
                                mtsSchedule.TransferStatus = "Y";
                                mtsSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                step6UpdateAfterProcessingSuccessF580020.RowsAffected = F580020Manager.UpdateAfterProcessingSuccessF580020(e1Session, plant.PlantNum, scheduleNum, mtsSchedule.ScheduleDateE1);
                            }
                            catch (Exception e)
                            {
                                step6UpdateAfterProcessingSuccessF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6UpdateAfterProcessingSuccessF580020.Done();
                                response.ProcessSteps.Add(step6UpdateAfterProcessingSuccessF580020);
                            }
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())

                        var step7DownloadRawGlassTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download Raw Glass Table");
                        try
                        {
                            step7DownloadRawGlassTable.RowsAffected = RawGlassDownloadManager.Process(e1Session, cnSFInt, plant, scheduleNum, mtsSchedule.ScheduleDateE1, mtsSchedule.BatchNum, 0);
                        }
                        catch (Exception e)
                        {
                            step7DownloadRawGlassTable.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step7DownloadRawGlassTable.Done();
                            response.ProcessSteps.Add(step7DownloadRawGlassTable);
                        }
                    } // if (!step1GetMTSSchedule.HasErrors() && !mtsSchedule.ValidationErrors.Any())
                } // if ((!mtsSchedule.ValidationErrors.Any()) || force && !mtsSchedule.IsFatal())
                else
                {
                    mtsSchedule.TransferStatus = "E";
                    mtsSchedule.TransferMessage = mtsSchedule.ValidationStatus;
                    var step3CancelMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel After Validation Failure F580020");
                    try
                    {
                        mtsSchedule.TransferDate = DateTime.Now.ToLocalTime();
                        step3CancelMTOSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum, "Validation failure");
                    }
                    catch (Exception e)
                    {
                        step3CancelMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3CancelMTOSchedule.Done();
                        response.ProcessSteps.Add(step3CancelMTOSchedule);
                    }
                }
                if (mtsSchedule.ValidationErrors != null)
                {
                    foreach (var validationError in mtsSchedule.ValidationErrors)
                    {
                        response.Errors.Add(new Error(validationError.Message, false));
                    }
                }
                response.Done();
                MTSScheduleManager.SummarizeMTSSchedule(response, mtsSchedule);
                SummarizeMTSScheduleDownload(response, mtsSchedule, force);
                NotifyMTSScheduleDownload(response, mtsSchedule, force);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadMTSIncomingOrders(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ServiceContext serviceContext, bool parallel = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var response = new ProcessResponse($"Download MTS Incoming Orders ({plant.PlantCode})", true);
            response.ServiceContext = serviceContext;
            response.Plant = plant;
            response.IsParallel = parallel;
            response.ConnectionStrings.Add(new ConnectionString("E1", e1Session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            try
            {
                ProcessResponse childResponse;
                var mtsScheduleEvents = MTSScheduleEventsManager.ReserveUnprocessedMTSScheduleEvents(e1Session, plant, out childResponse);
                //var mtsScheduleEvents = MTSScheduleEventsManager.GetUnprocessedMTSScheduleEvents(e1Session, plant, out childResponse);
                response.ProcessSteps.AddRange(childResponse.ProcessSteps);

                if (mtsScheduleEvents.Any())
                {
                    try
                    {
                        foreach (var mtsSchedule in mtsScheduleEvents)
                        {
                            try
                            {
                                Logger.Info($"MTS SCHEDULE: {mtsSchedule.ScheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
                                response.ProcessResponses.Add(Process(e1Session, sqlCns["PLANT.INTEGRATION"], plant, mtsSchedule.ScheduleNum, 0, "", false, null, false));
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtsSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                                response.Close();
                                MTSScheduleProcessor.NotifyMTSScheduleDownload(response, mtsSchedule.ScheduleNum);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                        response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                        response.Close();
                        MTSScheduleProcessor.NotifyMTSScheduleDownload(response, 0);
                    }
                    finally
                    {
                        response.Done();
                        Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: PLANT: {plant.PlantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleDownload(response, 0);
            }
            finally
            {
                response.Close();
                Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void NotifyMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule, bool force)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] MTS Schedule {mtsSchedule.ScheduleNum} Download";
                if (force) subject = $@"Force - {subject}";
                if (response.HasErrors() || mtsSchedule.ValidationErrors.Any()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void SummarizeMTSScheduleDownload(ProcessResponse response, MTSSchedule mtsSchedule, bool force)
        {
            var stepSummarizeDownload = new ProcessStep("Download Summary Collections");
            try
            {
                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any(),
                    mtsSchedule.TransferMessage,
                    Force = force,
                    response.IsParallel,
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadScheduleInfoMTSTable?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}&Force=true"));
                //actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessMTSScheduleEvent?Plant={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarizeDownload.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeDownload.Done();
                response.ProcessSteps.Add(stepSummarizeDownload);
            }
        }

        public static int DownloadScheduleInfoMTSTable(SqlConnection cnSFInt, MTSIncomingOrderLines incomingOrderLines)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount = 0;
            try
            {
                var sheduleInfoMTSTable = new ScheduleInfoMTSTable();
                sheduleInfoMTSTable.Add(incomingOrderLines);
                if (sheduleInfoMTSTable.Rows.Count > 0)
                {
                    var bulkResponse = BulkCopyManager.BulkTable(sheduleInfoMTSTable.ExtendedProperties["Target"].ToString(), sheduleInfoMTSTable, cnSFInt);
                    rowCount = (int) bulkResponse.RowsCopied;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }

        public static int CountDownloadedWorkOrders(SqlConnection cnSFInt, decimal scheduleNum, DateTime scheduleDate, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT  COUNT(*) AS 'RowCount'
                        FROM    ( SELECT DISTINCT
                                            [ScheduleNum]
                                           ,[ScheduleDate]
                                           ,[ScheduleSeq]
                                           ,[WOrderNum]
                                           ,[WOrderType]
                                           ,ROW_NUMBER() OVER (PARTITION BY [ScheduleNum], [ScheduleSeq] ORDER BY [DownloadDate] DESC) AS 'RowNumber'
                                  FROM      [INTEGRATION].[EBS].[MTSIncomingOrdersLedger]
                                  WHERE     [ScheduleNum] = @ScheduleNum
                                        AND [ScheduleDate] = @ScheduleDate
                                        AND [E1BatchID] = @BatchNum
                             ) x WHERE [RowNumber] = 1"
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar));
                cmd.Parameters["@BatchNum"].Value = batchNum;
                cmd.Connection = cnSFInt;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rowCount = CastDbNull.To(reader["RowCount"], 0);
                }
                reader.Close();
                Logger.Trace($@"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }
    }
}
