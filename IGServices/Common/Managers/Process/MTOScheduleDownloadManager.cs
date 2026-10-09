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

namespace IGServices.Common.Managers.Process
{
    public class MTOScheduleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttributesManager;

        /*****************************************************************************
        *   CONSTRUCTOR
        ******************************************************************************/
        public MTOScheduleDownloadManager(BusinessSystemSession session)
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
            var response = new ProcessResponse("MTO Schedule Download");
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
                var f580220Mgr = new F580220Manager();
                var f4715Mgr = new F4715Manager(session);
                var f584715Mgr = new F584715Manager(session);

                /********************************************************/
                var stepHeaderInUse = new ProcessStep("Set E1 Header Status - In Use");
                try
                {
                    var dmlResponse = ScheduleEventManager.UpdateBeforeStatus(session, evt);
                    stepHeaderInUse.SqlDmlResponses.Add(dmlResponse);
                    stepHeaderInUse.RowsAffected = dmlResponse.RowsAffected;
                    ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "U", "E1 In Use");
                    evt.TransferStatusCode = "U";
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
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
                    schedule.f580020 = F580020Manager.GetF580020(session, evt.PlantNum, evt.ScheduleNum, evt.ScheduleDateE1, evt.BatchNum);
                    stepF580020.RowsAffected = (schedule.ScheduleEvent.ScheduleNum > 0) ? 1 : 0;
                    stepF580020.Messages.Add(new Message("Schedule Key", $"Plant {plant.PlantNum} ScheduleNum {schedule.ScheduleEvent.ScheduleNum} ScheduleDateE1 {schedule.ScheduleEvent.ScheduleDateE1} Batch {schedule.ScheduleEvent.BatchNum}"));
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
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
                    evt.TransferStatusCode = "E";
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
                    evt.TransferStatusCode = "E";
                    stepF580024.Errors.Add(new Error(e));
                }
                stepF580024.Done();
                response.AddStep(stepF580024);

                /********************************************************/
                var stepF580220 = new ProcessStep("E1 Schedule - F580220");
                try
                {
                    schedule.f580220s = f580220Mgr.GetF580220s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.ScheduleDateE1,
                        schedule.ScheduleEvent.BatchNum);
                    stepF580220.RowsAffected = schedule.f580220s.Count;
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
                    stepF580220.Errors.Add(new Error(e));
                }
                stepF580220.Done();
                response.AddStep(stepF580220);

                /********************************************************/
                var stepF4715 = new ProcessStep("E1 Schedule - F4715");
                try
                {
                    foreach (var f580021 in schedule.f580021s)
                    {
                        var s7Rorn = Data.ParseStringToDecimal(f580021.S7RORN);
                        var f4715Values = f4715Mgr.GetF4715s(session, plant.CompanyNum, s7Rorn, f580021.S7RCTO, f580021.S7LNID);
                        schedule.f4715s.AddRange(f4715Values);
                    }
                    stepF4715.RowsAffected = schedule.f4715s.Count;
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
                    stepF4715.Errors.Add(new Error(e));
                }
                stepF4715.Done();
                response.AddStep(stepF4715);

                /********************************************************/
                var stepF584715 = new ProcessStep("E1 Schedule - F584715");
                try
                {
                    foreach (var f580021 in schedule.f580021s)
                    {
                        var s7Rorn = Data.ParseStringToDecimal(f580021.S7RORN);
                        var f584715Values = f584715Mgr.GetF584715s(session, plant.CompanyNum, s7Rorn, f580021.S7RCTO, f580021.S7LNID);
                        schedule.f584715s.AddRange(f584715Values);
                    }
                    stepF584715.RowsAffected = schedule.f584715s.Count;
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
                    stepF584715.Errors.Add(new Error(e));
                }
                stepF584715.Done();
                response.AddStep(stepF584715);

                /********************************************************/
                var stepF580232 = new ProcessStep("E1 Schedule - F580232");
                try
                {
                    schedule.f580232s = F580232Manager.GetF580232(session, plant.PlantNum, schedule.ScheduleNum);
                    stepF580232.RowsAffected = schedule.f580232s.Count;
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
                    stepF580232.Errors.Add(new Error(e));
                }
                stepF580232.Done();
                response.AddStep(stepF580232);

                /********************************************************/
                var stepBusinessValidate = new ProcessStep("Validate");
                try
                {
                    var validationErrors = ValidateBusinessSystem(session, schedule);
                    if (validationErrors.Any())
                    {
                        evt.TransferStatusCode = "E";
                        stepBusinessValidate.Errors.AddRange(validationErrors);
                        stepBusinessValidate.RowsAffected = validationErrors.Count;
                    }
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
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

                        // F580220
                        scheduleLine.F580220s =
                            schedule.f580220s?.FindAll(
                                item =>
                                    (int)item.CRDOCO == s7Rorn && item.CRDCTO == f580021.S7RCTO &&
                                    item.CRLNID == f580021.S7LNID);

                        // F4715
                        scheduleLine.F4715s =
                            schedule.f4715s?.FindAll(
                                item =>
                                    (int)item.ZDDOCO == s7Rorn && item.ZDDCTO == f580021.S7RCTO &&
                                    item.ZDLNID == f580021.S7LNID);

                        // F584715
                        scheduleLine.F584715s =
                            schedule.f584715s?.FindAll(
                                item =>
                                    (int)item.ZDDOCO == s7Rorn && item.ZDDCTO == f580021.S7RCTO &&
                                    item.ZDLNID == f580021.S7LNID);


                        // F580232
                        scheduleLine.F580232 =
                            schedule.f580232s?.Find(
                                item =>
                                    item.BDZ58WODC == f580021.S7DOCO);

                        // F3211
                        scheduleLine.ItemAttributes = _itemAttributesManager.GetConfiguredItemAttributes(session, f580021.CMCFGID);

                        // Schedule Line First Class Objects
                        scheduleLine.BatchNum = scheduleLine.F580232?.BDZ58BNUM ?? 0;
                        scheduleLine.BatchName = scheduleLine.F580232?.BHZ58BDES ?? "";
                        scheduleLine.BatchGroupName = scheduleLine.F580232?.BHZ58GDES ?? "";
                        scheduleLine.BatchDynamicName = scheduleLine.F580232?.BHDS01 ?? "";

                        scheduleLine.DeliveryInstruction1 = scheduleLine.F580232?.SHDEL1 ?? "";
                        scheduleLine.DeliveryInstruction2 = scheduleLine.F580232?.SHDEL2 ?? "";

                        scheduleLines.Add(scheduleLine);
                    }
                    schedule.ScheduleLines = scheduleLines;
                    stepScheduleLines.RowsAffected = scheduleLines.Count;
                }
                catch (Exception e)
                {
                    evt.TransferStatusCode = "E";
                    stepScheduleLines.Errors.Add(new Error(e));
                }
                stepScheduleLines.Done();
                response.AddStep(stepScheduleLines);

                /********************************************************/
                if (!response.HasErrors())
                {
                    var stepBulkCopy = new ProcessStep("Bulk Schedule Tables");
                    //var txn = sqlCns["PLANT.INTEGRATION"].BeginTransaction("OrderTables");
                    try
                    {
                        evt.TransferStatusCode = "N";
                        var orderTable = new MTOIncomingOrderTable();
                        orderTable.Add(schedule);
                        stepBulkCopy.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(orderTable.ExtendedProperties["Target"].ToString(), orderTable, sqlCns["PLANT.INTEGRATION"]));
                        //stepBulkCopy.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(orderTable.ExtendedProperties["Target"].ToString(), orderTable, sqlCns["PLANT.INTEGRATION"], txn));
                        //txn.Commit();
                        stepBulkCopy.RowsAffected = orderTable.Rows.Count;
                        evt.TransferStatusCode = "Y";
                    }
                    catch (Exception e)
                    {
                        //txn.Rollback();
                        evt.TransferStatusCode = "E";
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
                    if (evt.TransferStatusCode == "E")
                    {
                        dmlResponse = ScheduleEventManager.UpdateAfterFail(session, evt);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "E", "E1 Download Failed");
                    }
                    else if (evt.TransferStatusCode == "N")
                    {
                        dmlResponse = ScheduleEventManager.UpdateAfterNetwork(session, evt);
                        ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "N", "Network Retry");
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
                        PlantName = $"{plant.PlantCode} - {plant.PlantNum}",
                        ScheduleNum = schedule.ScheduleNum,
                        ScheduleId = schedule.ScheduleId,
                        ScheduleDate = E1TimeManager.CastToDateTime(schedule.ScheduleDateE1).ToString("MM/dd/yyyy"),
                        WorkOrders = schedule.ScheduleLines.Count(),
                        BatchNum = schedule.ScheduleEvent.BatchNum,
                        TransferStatus = evt.TransferStatusCode,
                        StartDate = response.ProcessSteps.Min(x => x.StartDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDate = response.ProcessSteps.Max(x => x.EndDate).ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds =
                            (decimal)
                                (response.ProcessSteps.Max(x => x.EndDate) -
                                    response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add("ScheduleHeader", scheduleHeaderSummary, null);

                    var scheduleCustomerSummary =
                        schedule.ScheduleLines.GroupBy(x => new { x.F580021.S7AN8DR, x.BatchNum, x.BatchName })
                        //schedule.ScheduleLines.GroupBy(x => new { x.F580021.S7AN8DR, x.F580021.S7Z58PT, x.BatchNum, x.BatchName })
                            .Select(
                                ng =>
                                    new
                                    {
                                        Customer = ng.Key.S7AN8DR,
                                        //ProductType = ng.Key.S7Z58PT,
                                        CustomerName = ng.Select(x => x.F580021.S7ALPH).FirstOrDefault()?.Trim(),
                                        BatchNum = ng.Key.BatchNum,
                                        BatchName = ng.Key.BatchName,
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
                                        MinRequestedDate = ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Min().ToString("MM/dd/yyyy"),
                                        MaxRequestedDate = ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Max().ToString("MM/dd/yyyy")
                                    })
                            .ToList();

                    response.SerializedObjects.Add("CustomerSummary", scheduleCustomerSummary, "CustomerLines", "CustomerLine");

                    var actionLinks = new ActionLinks();
                    //actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOIncomingOrdersTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}&Force=true"));
                    actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/BackoutMtoSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}"));
                    actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}"));
                    actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}"));
                    actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}"));
                    actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={evt.ScheduleNum}"));
                    response.SerializedObjects.Add("ActionLinks", actionLinks);

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
                        var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.ScheduleDownloadReportMTO.xslt", Encoding.UTF8);
                        Logger.Info("Report XML: " + xslOutput);

                        var subject = $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] MTO Schedule {schedule.ScheduleNum} Download";

                        Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE", "ADMIN", null, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE", "PUBLIC", plant.PlantCode, null));
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE", "USER", plant.PlantCode, null));

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
                    response.AddStep(stepSummary);
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
        public Errors ValidateBusinessSystem(BusinessSystemSession session, Schedule schedule)
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
                    errors.Add(new Error(schedule.f580021s.Count(x => x.S7Z58SCDDJ == 0) + " Work Orders on this MTO Schedule have a missing Due Date (S7Z58SCDDJ) in E1"));
                    var lines = schedule.f580021s.Where(x => x.S7Z58SCDDJ == 0);
                    errors.AddRange(lines.Select(line => new Error($"Work Order {line.S7DOCO}")));
                }
                if (schedule.f580021s.Any(x => x.S7Z58QTY == 0))
                {
                    errors.Add(new Error(schedule.f580021s.Count(x => x.S7Z58QTY == 0) + " Work Orders on this MTO Schedule have Zero Quantity (S7Z58QTY) E1"));
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
        
        /***********************************************************
         * ALTERNATE
         **********************************************************/
        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force, ServiceContext serviceContext, bool parallel, ProcessResponse responseStub = null)
        {
            var response = new ProcessResponse($@"Download MTO Incoming Orders ({plant.PlantCode}) Schedule {scheduleNum} {(parallel ? "Parallel" : "Synchronous")}", true);
            if (responseStub != null)
            {
                response.ProcessSteps.AddRange(responseStub.ProcessSteps);
            }
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            mtoSchedule.TransferStatus = "R";
            try
            {
                response.ServiceContext = serviceContext ?? response.ServiceContext;
                response.Plant = plant;
                response.IsParallel = parallel;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                }
                catch (Exception e)
                {
                    mtoSchedule.TransferStatus = "E";
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2Validate = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Validate Schedule");
                    try
                    {
                        mtoSchedule.ValidationErrors = MTOScheduleManager.Validate(mtoSchedule, e1Session);
                        if (mtoSchedule.ValidationErrors.Any()) mtoSchedule.TransferStatus = "E";
                        step2Validate.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                    }
                    catch (Exception e)
                    {
                        mtoSchedule.TransferStatus = "E";
                        step2Validate.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2Validate.Done();
                        response.ProcessSteps.Add(step2Validate);
                    }
                }

                if ((!mtoSchedule.ValidationErrors.Any()) || force && !mtoSchedule.IsFatal())
                {
                    var step3UpdateBeforeProcessingF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update Before Processing F580020");
                    try
                    {
                        step3UpdateBeforeProcessingF580020.RowsAffected = F580020Manager.UpdateBeforeProcessingF580020(e1Session, plant.PlantNum, mtoSchedule.ScheduleNum, mtoSchedule.ScheduleDateE1);
                    }
                    catch (Exception e)
                    {
                        mtoSchedule.TransferStatus = "E";
                        step3UpdateBeforeProcessingF580020.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3UpdateBeforeProcessingF580020.Done();
                        response.ProcessSteps.Add(step3UpdateBeforeProcessingF580020);
                        mtoSchedule.TransferStatus = "U";
                    }

                    if ((!step3UpdateBeforeProcessingF580020.HasErrors() && mtoSchedule.IsValid()) || force && !mtoSchedule.IsFatal())
                    {
                        var step4DownloadIncomingOrdersTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download Incoming Orders Table");
                        var startTime = DateTime.Now.ToLocalTime();
                        Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
                        int rowCount = 0;
                        try
                        {
                            //response.RowsAffected = DownloadIncomingOrdersTable(cnSFInt, mtoSchedule.MTOIncomingOrderLines);
                            var incomingOrderTable = new MTOIncomingOrderTable();
                            incomingOrderTable.Add(mtoSchedule.MTOIncomingOrderLines);
                            if (incomingOrderTable.Rows.Count > 0)
                            {
                                var bulkResponse = BulkCopyManager.BulkTable(incomingOrderTable.ExtendedProperties["Target"].ToString(), incomingOrderTable, cnSFInt);
                                rowCount = (int)bulkResponse.RowsCopied;
                                mtoSchedule.TransferStatus = "Y";
                            }
                            Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                            step4DownloadIncomingOrdersTable.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                            response.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        }
                        catch (Exception e)
                        {
                            mtoSchedule.TransferStatus = "N";
                            step4DownloadIncomingOrdersTable.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step4DownloadIncomingOrdersTable.Done();
                            response.ProcessSteps.Add(step4DownloadIncomingOrdersTable);
                        }

                        if (mtoSchedule.TransferStatus == "E")
                        {
                            var step6UpdateAfterProcessingFailF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update After Processing Fail F580020");
                            try
                            {
                                mtoSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                step6UpdateAfterProcessingFailF580020.RowsAffected = F580020Manager.UpdateAfterProcessingFailF580020(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDateE1);
                            }
                            catch (Exception e)
                            {
                                mtoSchedule.TransferStatus = "E";
                                step6UpdateAfterProcessingFailF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6UpdateAfterProcessingFailF580020.Done();
                                response.ProcessSteps.Add(step6UpdateAfterProcessingFailF580020);
                            }
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())
                        else if (mtoSchedule.TransferStatus == "N")
                        {
                            var step6UpdateAfterProcessingNetworkF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update After Processing Network F580020");
                            try
                            {
                                mtoSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                step6UpdateAfterProcessingNetworkF580020.RowsAffected = F580020Manager.UpdateAfterProcessingNetworkF580020(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDateE1);
                            }
                            catch (Exception e)
                            {
                                mtoSchedule.TransferStatus = "E";
                                step6UpdateAfterProcessingNetworkF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6UpdateAfterProcessingNetworkF580020.Done();
                                response.ProcessSteps.Add(step6UpdateAfterProcessingNetworkF580020);
                            }
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())
                        else if (mtoSchedule.TransferStatus == "Y")
                        {
                            var step6UpdateAfterProcessingSuccessF580020 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Update After Processing Success F580020");
                            try
                            {
                                mtoSchedule.TransferStatus = "Y";
                                mtoSchedule.TransferDate = DateTime.Now.ToLocalTime();
                                step6UpdateAfterProcessingSuccessF580020.RowsAffected = F580020Manager.UpdateAfterProcessingSuccessF580020(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDateE1);
                            }
                            catch (Exception e)
                            {
                                mtoSchedule.TransferStatus = "E";
                                step6UpdateAfterProcessingSuccessF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step6UpdateAfterProcessingSuccessF580020.Done();
                                response.ProcessSteps.Add(step6UpdateAfterProcessingSuccessF580020);
                            }

                            if (mtoSchedule.MTOScheduleEvent.ShopFloorSystem == "7")
                            {
                                var step7DownloadRawGlassTable = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Download Raw Glass Table");
                                try
                                {
                                    step7DownloadRawGlassTable.RowsAffected = RawGlassDownloadManager.Process(e1Session, cnSFInt, plant, scheduleNum, mtoSchedule.ScheduleDateE1, mtoSchedule.BatchNum, 0);
                                }
                                catch (Exception e)
                                {
                                    mtoSchedule.TransferStatus = "E";
                                    step7DownloadRawGlassTable.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step7DownloadRawGlassTable.Done();
                                    response.ProcessSteps.Add(step7DownloadRawGlassTable);
                                }
                            } // if (mtoSchedule.MTOScheduleEvent.ShopFloorSystem == "7")
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())
                    } // if (!step1GetMTOSchedule.HasErrors() && !mtoSchedule.ValidationErrors.Any())
                } // if ((!mtoSchedule.ValidationErrors.Any()) || force && !mtoSchedule.IsFatal())
                else
                {
                    mtoSchedule.TransferStatus = "E";
                    mtoSchedule.TransferMessage = mtoSchedule.ValidationStatus;
                    var step3CancelMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel After Validation Failure F580020");
                    try
                    {
                        mtoSchedule.TransferDate = DateTime.Now.ToLocalTime();
                        step3CancelMTOSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum, "Validation failure");
                    }
                    catch (Exception e)
                    {
                        mtoSchedule.TransferStatus = "E";
                        step3CancelMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3CancelMTOSchedule.Done();
                        response.ProcessSteps.Add(step3CancelMTOSchedule);
                    }
                }
                response.Close();
                MTOScheduleManager.SummarizeMTOSchedule(response, mtoSchedule);
                SummarizeMTOScheduleDownload(response, mtoSchedule, force);
                NotifyMTOScheduleDownload(response, mtoSchedule, force);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadMTOIncomingOrders(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ServiceContext serviceContext, bool parallel = false)
        {
            var startTime = DateTime.Now.ToLocalTime();
            var response = new ProcessResponse($"Download MTO Incoming Orders ({plant.PlantCode})");
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
                var mtoScheduleEvents = MTOScheduleEventsManager.ReserveUnprocessedMTOScheduleEvents(e1Session, plant, out var childResponse);
                response.ProcessSteps.AddRange(childResponse.ProcessSteps);

                if (mtoScheduleEvents.Any())
                {
                    try
                    {
                        foreach (var mtoSchedule in mtoScheduleEvents)
                        {
                            try
                            {
                                Logger.Info($"MTO SCHEDULE: {mtoSchedule.ScheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
                                response.ProcessResponses.Add(Process(e1Session, sqlCns["PLANT.INTEGRATION"], plant, mtoSchedule.ScheduleNum, 0, "", false, serviceContext, parallel, response));
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtoSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                                response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} Schedule: {mtoSchedule.ScheduleNum}  {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                                response.Close();
                                MTOScheduleProcessor.NotifyMTOScheduleDownload(response, mtoSchedule.ScheduleNum);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}");
                        response.Errors.Add(new Error($"ERROR: PLANT: {plant.PlantCode} {sqlCns["PLANT.INTEGRATION"].ConnectionString}  {e.Message}"));
                        response.Close();
                        MTOScheduleProcessor.NotifyMTOScheduleDownload(response, 0);
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
                MTOScheduleProcessor.NotifyMTOScheduleDownload(response, 0);
            }
            finally
            {
                response.Close();
                Logger.Info($"PLANT: {plant.PlantCode} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void NotifyMTOScheduleDownload(ProcessResponse response, MTOSchedule mtoSchedule, bool force)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] MTO Schedule {mtoSchedule.ScheduleNum} Download";
                if (force) subject = $@"Force - {subject}";
                if (response.HasErrors() || mtoSchedule.ValidationErrors.Any()) subject = $@"Error - {subject}";

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

        public static void SummarizeMTOScheduleDownload(ProcessResponse response, MTOSchedule mtoSchedule, bool force)
        {
            var stepSummarizeDownload = new ProcessStep("Download Summary Collections");
            try
            {
                F580021 firstF580024 = null;
                foreach (var f580021 in mtoSchedule.E1ScheduleData.F580021s)
                {
                    firstF580024 = f580021;
                    break;
                }
                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    mtoSchedule.TransferDate,
                    s7upmj = firstF580024?.S7UPMJ,
                    s7tday = firstF580024?.S7TDAY,
                    s7StartTime = E1TimeManager.CastToDateTime(firstF580024.S7UPMJ, firstF580024.S7TDAY),
                    s7StartTimeString = E1TimeManager.CastToDateTime(firstF580024.S7UPMJ, firstF580024.S7TDAY).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    e1BuildTime = Math.Round((decimal)(DateTime.Now - E1TimeManager.CastToDateTime(firstF580024.S7UPMJ, firstF580024.S7TDAY)).TotalSeconds/60, 4),
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any(),
                    mtoSchedule.TransferMessage,
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
                //actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?Plant={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        //public static int DownloadIncomingOrdersTable(SqlConnection cnSFInt, MTOIncomingOrderLines incomingOrderLines)
        //{
        //    var startTime = DateTime.Now.ToLocalTime();
        //    Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
        //    int rowCount = 0;
        //    try
        //    {
        //        var incomingOrderTable = new MTOIncomingOrderTable();
        //        incomingOrderTable.Add(incomingOrderLines);
        //        if (incomingOrderTable.Rows.Count > 0)
        //        {
        //            var bulkResponse = BulkCopyManager.BulkTable(incomingOrderTable.ExtendedProperties["Target"].ToString(), incomingOrderTable, cnSFInt);
        //            rowCount = (int) bulkResponse.RowsCopied;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        mtoSchedule.TransferStatus = "N";
        //        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
        //        throw new ApplicationException($"{e.Message}: {e.StackTrace}");
        //    }
        //    finally
        //    {
        //        Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
        //    }
        //    return rowCount;
        //}
    }
}
