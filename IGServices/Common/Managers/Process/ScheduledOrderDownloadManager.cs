using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using NLog;
using FlexItemManager = IGServices.Common.Managers.E1.FlexItemManager;

namespace IGServices.Common.Managers.Process
{
    class ScheduledOrderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttributesManager;
        private readonly FlexItemManager _flexMgr;
        //private readonly SpecUnitManager _specUnitManager;
        private readonly RelatedSalesLineManager _relatedSalesLineManager;
        private readonly GlassPartManager _glassPartManager;

        /*****************************************************************************
        *   CONSTRUCTOR
        ******************************************************************************/
        public ScheduledOrderDownloadManager(BusinessSystemSession session)
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
            _flexMgr = new FlexItemManager(session, "P5842300");
            //_specUnitManager = new SpecUnitManager(session);
            _relatedSalesLineManager = new RelatedSalesLineManager(session);
            _glassPartManager = new GlassPartManager(session);
        }

        /*****************************************************************************
        *   PROCESS
        ******************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, ScheduleEvent evt, bool force)
        {
            var response = new ProcessResponse("Scheduled Order Download");
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

                /********************************************************/
                //var step0 = new ProcessStep("Set E1 Header Status - In Use");
                //try
                //{
                //    var dmlResponse = MtoScheduleEventManager.UpdateBeforeStatus(session, evt);
                //    step0.SqlDmlResponses.Add(dmlResponse);
                //    step0.RowsAffected = dmlResponse.RowsAffected;
                //}
                //catch (Exception e)
                //{
                //    step0.Errors.Add(new Error(e));
                //}
                //step0.Done();
                //response.AddStep(step0);

                /********************************************************/
                var step1 = new ProcessStep("E1 Schedule - F580020");
                try
                {
                    schedule.Plant = plant;
                    schedule.ScheduleEvent = evt;
                    schedule.ScheduleNum = evt.ScheduleNum;
                    schedule.ScheduleDateE1 = evt.ScheduleDateE1;
                    schedule.f5834A01 = f5834A01Mgr.GetSchedule(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleDateE1);
                    schedule.f580020 = F580020Manager.GetF580020(session, evt.PlantNum, evt.ScheduleNum,
                        evt.ScheduleDateE1, evt.BatchNum);
                    step1.RowsAffected = (schedule.ScheduleEvent.ScheduleNum > 0) ? 1 : 0;
                    step1.Messages.Add(new Message("Schedule Key", $"Plant {plant.PlantNum} ScheduleNum {schedule.ScheduleEvent.ScheduleNum} ScheduleDateE1 {schedule.ScheduleEvent.ScheduleDateE1} Batch {schedule.ScheduleEvent.BatchNum}"));
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);

                /********************************************************/
                var step2 = new ProcessStep("E1 Schedule - F580021");
                try
                {
                    schedule.f580021s = F580021Manager.GetF580021s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.BatchNum);
                    step2.RowsAffected = schedule.f580021s.Count;
                    response.RowsAffected = schedule.f580021s.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                var step3 = new ProcessStep("E1 Schedule - F580024");
                try
                {
                    schedule.f580024s = F580024Manager.GetF580024s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.BatchNum);
                    step3.RowsAffected = schedule.f580024s.Count;
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);

                /********************************************************/
                var step4 = new ProcessStep("E1 Schedule - F580220");
                try
                {
                    schedule.f580220s = f580220Mgr.GetF580220s(session, plant.PlantNum, schedule.ScheduleNum, schedule.ScheduleEvent.ScheduleDateE1,
                        schedule.ScheduleEvent.BatchNum);
                    step4.RowsAffected = schedule.f580220s.Count;
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                step4.Done();
                response.AddStep(step4);

                /********************************************************/
                var step5 = new ProcessStep("E1 Schedule - F4715");
                try
                {
                    foreach (var f580021 in schedule.f580021s)
                    {
                        var s7Rorn = Data.ParseStringToDecimal(f580021.S7RORN);
                        var f4715Values = f4715Mgr.GetF4715s(session, plant.CompanyNum, s7Rorn, f580021.S7RCTO, f580021.S7LNID);
                        schedule.f4715s.AddRange(f4715Values);
                    }
                    step5.RowsAffected = schedule.f4715s.Count;
                }
                catch (Exception e)
                {
                    step5.Errors.Add(new Error(e));
                }
                step5.Done();
                response.AddStep(step5);

                /********************************************************/
                var step5a = new ProcessStep("E1 Schedule - F580232");
                try
                {
                    schedule.f580232s = F580232Manager.GetF580232(session, plant.PlantNum, schedule.ScheduleNum);
                    step5a.RowsAffected = schedule.f580232s.Count;
                }
                catch (Exception e)
                {
                    step5a.Errors.Add(new Error(e));
                }
                step5a.Done();
                response.AddStep(step5a);

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
                var step6 = new ProcessStep("Schedule Line Collections");
                try
                {
                    var custNameMgr = new CustomerNameManager(session);
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


                        // F580232
                        scheduleLine.F580232 =
                            schedule.f580232s?.Find(
                                item =>
                                    item.BDZ58WODC == f580021.S7DOCO);

                        // Flex
                        scheduleLine.FlexItemDescriptions = _flexMgr.GetDescription(session,
                            scheduleLine.ItemAttributes);

                        // Schedule Line First Class Objects
                        scheduleLine.BatchNum = scheduleLine.F580232?.BDZ58BNUM ?? 0;
                        scheduleLine.BatchName = scheduleLine.F580232?.BHZ58BDES;
                        scheduleLine.BatchGroupName = scheduleLine.F580232?.BHZ58GDES;
                        scheduleLine.BatchDynamicName = scheduleLine.F580232?.BHDS01;

                        // Spec Units
                        //var specUnit = _specUnitManager.GetSpecUnit(session, scheduleLine);
                        //scheduleLine.SpecUnit = specUnit;

                        // Glass Part
                        var glassPart = _glassPartManager.GetGlassPart(session, scheduleLine);
                        scheduleLine.GlassPart = glassPart;

                        if (glassPart.StockingType != "C")
                        {
                            var rawGlassParts = _glassPartManager.GetWorkOrderGlassParts(session, scheduleLine.F580021.S7DOCO);
                            switch (rawGlassParts.Count)
                            {
                                case 1:
                                    scheduleLine.F580024.S8Z5851551 = rawGlassParts[0].ItemNum;
                                    break;
                                case 2:
                                    scheduleLine.F580024.S8Z5851551 = rawGlassParts[0].ItemNum;
                                    scheduleLine.F580024.S8Z5851552 = rawGlassParts[1].ItemNum;
                                    break;
                                case 3:
                                    scheduleLine.F580024.S8Z5851551 = rawGlassParts[0].ItemNum;
                                    scheduleLine.F580024.S8Z5851552 = rawGlassParts[1].ItemNum;
                                    scheduleLine.F580024.S8Z5851553 = rawGlassParts[2].ItemNum;
                                    break;
                            }
                        }

                        // F3211
                        scheduleLine.ItemAttributes = _itemAttributesManager.GetItemAttributes(session, scheduleLine.GlassPart.ItemNum, f580021.CMCFGID);

                        // Related Sales Line
                        var relatedSalesLine = _relatedSalesLineManager.GetRelatedSalesLine(session, scheduleLine);
                        scheduleLine.RelatedSalesLine = relatedSalesLine;

                        scheduleLines.Add(scheduleLine);
                    }
                    schedule.ScheduleLines = scheduleLines;
                    step6.RowsAffected = scheduleLines.Count;
                }
                catch (Exception e)
                {
                    step6.Errors.Add(new Error(e));
                }
                step6.Done();
                response.AddStep(step6);

                /********************************************************/
                if (!response.HasErrors())
                {
                    var step9 = new ProcessStep("Assign Sequences");
                    //var txn = cnInt.BeginTransaction("SpecUnit");
                    try
                    {
                        var scheduleId = SequenceManager.GetNextVal(sqlCns["PLANT.INTEGRATION"], "ScheduleId");
                        schedule.ScheduleId = scheduleId;
                    }
                    catch (Exception e)
                    {
                        step9.Errors.Add(new Error(e));
                    }
                    step9.Done();
                    response.AddStep(step9);
                }

                /********************************************************/
                if (!response.HasErrors())
                {
                    var step9a = new ProcessStep("Bulk Schedule Tables");
                    var txn = sqlCns["PLANT.INTEGRATION"].BeginTransaction("OrderTables");
                    try
                    {

                        var dts = new List<DataTable>();
                        var scheduleTable = new ScheduleTable();
                        var orderTable = new ScheduledOrderTable();

                        scheduleTable.Add(schedule);
                        orderTable.Add(schedule);

                        dts.Add(scheduleTable);
                        dts.Add(orderTable);

                        //Logger.Info("Order Table: " + XmlSerializationUtility.SerializeObject(Encoding.UTF8, orderTable));
                        response.SerializedObjects.Add("OrderTableSerialized", orderTable);

                        foreach (DataTable dt in dts)
                        {
                            step9a.SqlBulkCopyResponses.Add(
                                BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, sqlCns["PLANT.INTEGRATION"], txn));
                        }
                        txn.Commit();
                        step9a.RowsAffected = scheduleTable.Rows.Count;
                    }
                    catch (Exception e)
                    {
                        txn.Rollback();
                        step9a.Errors.Add(new Error(e));
                    }
                    step9a.Done();
                    response.AddStep(step9a);
                }

                /********************************************************/
                //var step10 = new ProcessStep("Set E1 Header Status - Success/Failure");
                //try
                //{
                //    var dmlResponse = new SqlDmlResponse();
                //    if (response.HasErrors())
                //    {
                //        dmlResponse = MtoScheduleEventManager.UpdateAfterFail(session, evt);
                //    }
                //    else
                //    {
                //        dmlResponse = MtoScheduleEventManager.UpdateAfterSuccess(session, evt);
                //    }
                //    step10.SqlDmlResponses.Add(dmlResponse);
                //    step10.RowsAffected = dmlResponse.RowsAffected;
                //}
                //catch (Exception e)
                //{
                //    step10.Errors.Add(new Error(e));
                //}
                //step10.Done();
                //response.AddStep(step10);

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

                    if (evt.ShopFloorSystem == "2")
                    {
                        var scheduleCustomerSummary =
                            schedule.ScheduleLines.GroupBy(x => new { x.F580021.S7AN8DR, x.BatchNum, x.BatchName })
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
                                            MinRequestedDate =
                                                ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Min().ToString("MM/dd/yyyy"),
                                            MaxRequestedDate =
                                                ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Max().ToString("MM/dd/yyyy")
                                        })
                                .ToList();
                        response.SerializedObjects.Add("CustomerSummary", scheduleCustomerSummary, "CustomerLines", "CustomerLine");
                        stepSummary.RowsAffected = scheduleCustomerSummary.Count;
                    }
                    else
                    {
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
                                            MonoPickUnits =
                                                ng.Where(x => x.F580021.S7Z58L1RG == "K" && x.F580021.S7Z58L2TK == 0)
                                                    .Sum(x => x.F580021.S7Z58QTY),
                                            MonoCutUnits =
                                                ng.Where(x =>
                                                        x.F580021.S7Z58L1RG.Contains("C") && x.F580021.S7Z58L2TK == 0)
                                                    .Sum(x => x.F580021.S7Z58QTY),
                                            IG2PickUnits =
                                                ng.Where(x =>
                                                        x.F580021.S7Z58L2TK > 0 && x.F580021.S7Z58L3TK == 0 &&
                                                        x.F580021.S7Z58L1RG == "K" && x.F580021.S7Z58L2RG == "K")
                                                    .Sum(x => x.F580021.S7Z58QTY),
                                            IG2LineUnits =
                                                ng.Where(x =>
                                                        x.F580021.S7Z58L2TK > 0 && x.F580021.S7Z58L3TK == 0 &&
                                                        (x.F580021.S7Z58L1RG != "K" || x.F580021.S7Z58L2RG != "K"))
                                                    .Sum(x => x.F580021.S7Z58QTY),
                                            IG3PickUnits =
                                                ng.Where(x =>
                                                        x.F580021.S7Z58L3TK > 0 && x.F580021.S7Z58L1RG == "K" &&
                                                        x.F580021.S7Z58L2RG == "K" && x.F580021.S7Z58L3RG == "K")
                                                    .Sum(x => x.F580021.S7Z58QTY),
                                            IG3LineUnits =
                                                ng.Where(x =>
                                                    x.F580021.S7Z58L3TK > 0 &&
                                                    (x.F580021.S7Z58L1RG != "K" || x.F580021.S7Z58L2RG != "K" ||
                                                     x.F580021.S7Z58L3RG != "K")).Sum(x => x.F580021.S7Z58QTY),
                                            TotalUnits = ng.Sum(x => x.F580021.S7Z58QTY),
                                            MinRequestedDate =
                                                ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Min()
                                                    .ToString("MM/dd/yyyy"),
                                            MaxRequestedDate =
                                                ng.Select(x => E1TimeManager.CastToDateTime(x.F580021.S7RSDJ)).Max()
                                                    .ToString("MM/dd/yyyy")
                                        })
                                .ToList();
                        response.SerializedObjects.Add("CustomerSummary", scheduleCustomerSummary, "CustomerLines", "CustomerLine");
                        stepSummary.RowsAffected = scheduleCustomerSummary.Count;
                    }

                }
                catch (Exception e)
                {
                    stepSummary.Errors.Add(new Error(e));
                }
                stepSummary.Done();
                response.AddStep(stepSummary);

                /********************************************************/
                response.Done();

                //if (!response.HasErrors())
                //{
                //    var step11a = new ProcessStep("Bulk Schedule Response");
                //    try
                //    {
                //        var scheduleResponseTable = new ScheduleResponseTable();
                //        scheduleResponseTable.Add(schedule.ScheduleId, response);

                //        step11a.SqlBulkCopyResponses.Add(
                //            BulkCopyManager.BulkTable(scheduleResponseTable.ExtendedProperties["Target"].ToString(),
                //                scheduleResponseTable, cnInt));

                //        step11a.RowsAffected = scheduleResponseTable.Rows.Count;
                //    }
                //    catch (Exception e)
                //    {
                //        step11a.Errors.Add(new Error(e));
                //    }
                //    step11a.Done();
                //    response.AddStep(step11a);
                //}

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

                        var recipients = new List<string>();
                        string xslOutput;
                        if (evt.ShopFloorSystem == "2")
                        {
                            xslOutput = XsltManager.Transform(serializedXml,
                                "IGServices.Common.Xslt.ScheduleDownloadReportMTO.xslt", Encoding.UTF8);
                            Logger.Info("Report XML: " + xslOutput);

                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE",
                                "ADMIN", null, null));
                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE",
                                "PUBLIC", plant.PlantCode, null));
                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTO_SCHEDULE",
                                "USER", plant.PlantCode, null));
                        }
                        else
                        {
                            xslOutput = XsltManager.Transform(serializedXml,
                                "IGServices.Common.Xslt.ScheduleDownloadReportMTS.xslt", Encoding.UTF8);
                            Logger.Info("Report XML: " + xslOutput);

                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE",
                                "ADMIN", null, null));
                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE",
                                "PUBLIC", plant.PlantCode, null));
                            recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_MTS_SCHEDULE",
                                "USER", plant.PlantCode, null));
                        }

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            recipients,
                            null,
                            $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule {schedule.ScheduleNum} Download",
                            xslOutput);
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
                throw new ApplicationException($"Message: {e.Message} Trace: {e.StackTrace}");
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

        /*****************************************************************************
        *   VALIDATE
        ******************************************************************************/
        public Errors Validate(BusinessSystemSession session, SqlConnection cnCp, Schedule schedule)
        {
            var errors = new Errors();
            try
            {
                if (schedule.ScheduleLines.All(x => x.F580021 == null))
                {
                    errors.Add(new Error("No F580021 records found in E1"));
                }
                if (schedule.ScheduleLines.All(x => x.F580024 == null))
                {
                    errors.Add(new Error("No F580024 records found in E1"));
                }
                if (schedule.ScheduleLines.Count(x => x.F580021 != null) != schedule.ScheduleLines.Count(x => x.F580024 != null))
                {
                    errors.Add(new Error($"F580021 record count {schedule.ScheduleLines.Count(x => x.F580021 != null)} does not match F580024 record count {schedule.ScheduleLines.Count(x => x.F580024 != null)} in E1"));
                }
                if (schedule.ScheduleLines.Any(x => x.F580021.S7Z58SCDDJ == 0))
                {
                    errors.Add(new Error(schedule.ScheduleLines.Count(x => x.F580021.S7Z58SCDDJ == 0) + " Work Orders on this MTO Schedule have a missing Due Date in E1"));
                    var lines = schedule.ScheduleLines.Where(x => x.F580021.S7Z58SCDDJ == 0);
                    errors.AddRange(lines.Select(line => new Error($"Work Order {line.F580021.S7DOCO}")));
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return errors;
        }
    }
}
