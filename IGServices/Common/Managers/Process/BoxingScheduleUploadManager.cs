using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class BoxingScheduleUploadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSF, SqlConnection cnCorpInt, Plant plant, decimal scheduleNum, bool force)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Upload Boxing Schedule {plant.PlantCode}-{plant.PlantNum} Schedule {scheduleNum}", true);
            var boxingSchedule = new BoxingSchedule(scheduleNum);
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnCorpInt", cnCorpInt.ConnectionString));

                var step2GetShopFloorBoxingItems = new ProcessStep($@"Step 1. Get ShopFloor Boxing Items");
                try
                {
                    boxingSchedule = BoxingScheduleManager.GetSFBoxingSchedule(cnSF, plant, scheduleNum); 
                    step2GetShopFloorBoxingItems.RowsAffected = boxingSchedule.BoxItems.Count;
                    response.RowsAffected = step2GetShopFloorBoxingItems.RowsAffected;
                }
                catch (Exception e)
                {
                    step2GetShopFloorBoxingItems.Errors.Add(new Error(e));
                    response.Errors.Add(new Error(e.Message, false));
                }
                finally
                {
                    step2GetShopFloorBoxingItems.Done();
                    response.ProcessSteps.Add(step2GetShopFloorBoxingItems);
                }
                if (!step2GetShopFloorBoxingItems.HasErrors())
                {
                    var step3ValidateBoxingItems = new ProcessStep($@"Step 2. Validate Boxing Items");
                    try
                    {
                        boxingSchedule.ValidationErrors = BoxingScheduleManager.Validate(boxingSchedule, e1Session);
                        if (boxingSchedule.ValidationErrors.Any())
                        {
                            step3ValidateBoxingItems.Errors.Add(new Error(boxingSchedule.ValidationErrors[0].Message));
                            boxingSchedule.TransferStatus = "E";
                            step3ValidateBoxingItems.RowsAffected = boxingSchedule.ValidationErrors.Count;
                            if (boxingSchedule.ValidationErrors.Any(validationError => validationError.Message.Contains("previously uploaded"))) boxingSchedule.ValidationStatus = "Previously Uploaded";
                            
                            //response.SerializedObjects.Add(new SerializedObject("ValidationErrors", XmlSerializationUtility.SerializeObject(Encoding.UTF8, boxingSchedule.ValidationErrors)));
                            // Hack for Karl's Error string-search interface after response email sent.
                            if (boxingSchedule.ValidationErrors != null)
                            {
                                foreach (var validationError in boxingSchedule.ValidationErrors)
                                {
                                    response.Errors.Add(new Error(validationError.Message, false));
                                }
                            }
                        } //  if (!boxingSchedule.IsValid)
                        step3ValidateBoxingItems.RowsAffected = boxingSchedule.BoxItems.Count;
                    }
                    catch (Exception e)
                    {
                        step3ValidateBoxingItems.Errors.Add(new Error(e));
                        response.Errors.Add(new Error(e.Message, false));
                    }
                    finally
                    {
                        step3ValidateBoxingItems.Done();
                        response.ProcessSteps.Add(step3ValidateBoxingItems);
                    }

                    if (boxingSchedule.IsValid() || (force && !boxingSchedule.IsFatal()))
                    {
                        var step4BuildInterops = new ProcessStep($@"Step 3. Build E1 Interops");
                        var boxingInteropMgr = new BoxingInteropManager(e1Session);
                        boxingSchedule.BoxF580200 = new BoxF580200();
                        try
                        {
                            BatchNumManager.GetBoxingBatch(boxingSchedule, cnCorpInt);
                            if (!step4BuildInterops.HasErrors())
                            {
                                BatchNumManager.GetBIUKIDs(boxingSchedule, cnCorpInt);
                            }
                            if (!step4BuildInterops.HasErrors())
                            {
                                boxingInteropMgr.BuildBoxingInterops(boxingSchedule);
                                step4BuildInterops.RowsAffected = boxingSchedule.BoxItems.Count;
                            }
                            if (!step4BuildInterops.HasErrors())
                            {
                                boxingInteropMgr.BuildBoxingTrigger(boxingSchedule, cnCorpInt);
                            }
                        }
                        catch (Exception e)
                        {
                            step4BuildInterops.Errors.Add(new Error(e));
                            response.Errors.Add(new Error(e.Message, false));
                        }
                        finally
                        {
                            step4BuildInterops.Done();
                            response.ProcessSteps.Add(step4BuildInterops);
                        }
                        if (!step4BuildInterops.HasErrors())
                        {
                            var step5WriteE1Boxing = new ProcessStep($@"Step 4. Write E1 Boxing");
                            var e1Trans = e1Session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                            try
                            {
                                boxingInteropMgr.InsertE1BoxingInterops(e1Session, e1Trans, boxingSchedule);
                                step5WriteE1Boxing.RowsAffected = boxingSchedule.BoxItems.Count;
                                response.RowsAffected = boxingSchedule.BoxItems.Count;
                                if (!step5WriteE1Boxing.HasErrors())
                                {
                                    boxingInteropMgr.InsertE1BoxingTrigger(e1Session, e1Trans, boxingSchedule);
                                }
                                if (!step5WriteE1Boxing.HasErrors())
                                {
                                    e1Trans.Commit();
                                    SuccessE1(boxingSchedule);
                                    var step6UpdateF580020 = new ProcessStep($@"Step 5. Update F580020 EDSP=B");
                                    try
                                    {
                                        F580020Manager.BoxedF580020(e1Session, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, 0, "");
                                        step6UpdateF580020.RowsAffected = 1;
                                    }
                                    catch (Exception e)
                                    {
                                        step6UpdateF580020.Errors.Add(new Error(e));
                                        response.Errors.Add(new Error(e.Message, false));
                                    }
                                    finally
                                    {
                                        step6UpdateF580020.Done();
                                        response.ProcessSteps.Add(step6UpdateF580020);
                                    }
                                } // if (!step5WriteE1Boxing.HasErrors())
                                else
                                {
                                    boxingInteropMgr.RollBack(e1Trans);
                                }
                            }
                            catch (Exception e)
                            {
                                step5WriteE1Boxing.Errors.Add(new Error(e));
                                response.Errors.Add(new Error(e.Message, false));
                                boxingInteropMgr.RollBack(e1Trans);
                            }
                            finally
                            {
                                step5WriteE1Boxing.Done();
                                response.ProcessSteps.Add(step5WriteE1Boxing);
                            }

                            if (!step5WriteE1Boxing.HasErrors())
                            {
                                var step6UpdateShopFloor = new ProcessStep($@"Step 6. Update ShopFloor");
                                var sFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
                                //var shopFloorTran = cnSF.BeginTransaction(IsolationLevel.RepeatableRead);
                                try
                                {
                                    //BoxingScheduleManager.UpdateSFBoxHold(cnSF, shopFloorTran, boxingSchedule, sFUpdateBatchSize);
                                    BoxingScheduleManager.UpdateSFBoxHold(cnSF, boxingSchedule, sFUpdateBatchSize);
                                    step6UpdateShopFloor.RowsAffected = boxingSchedule.BoxItems.Count;
                                    if (!step6UpdateShopFloor.HasErrors())
                                    {
                                        //BoxingScheduleManager.UpdateSFBoxHeader(cnSF, shopFloorTran, boxingSchedule);
                                        BoxingScheduleManager.UpdateSFBoxHeader(cnSF, boxingSchedule);
                                    }
                                    if (!step6UpdateShopFloor.HasErrors())
                                    {
                                        //shopFloorTran.Commit();
                                        SuccessSF(boxingSchedule);
                                    }
                                    else
                                    {
                                        //BoxingScheduleManager.RollBack(shopFloorTran);
                                    }
                                    if (!step6UpdateShopFloor.HasErrors())
                                    {
                                        var step7InsertScheduleStatusEvent = new ProcessStep($@"Step 7. Insert ScheduleStatusEvent");
                                        try
                                        {
                                            step7InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, boxingSchedule.IncomingOrdersMinScheduleDate, "BOXED");
                                            if (step7InsertScheduleStatusEvent.RowsAffected == 0)
                                            {
                                                step7InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, boxingSchedule.IncomingOrdersMinScheduleDate, "BOXED");
                                                if (step7InsertScheduleStatusEvent.RowsAffected == 0)
                                                {
                                                    step7InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, boxingSchedule.IncomingOrdersMinScheduleDate, "BOXED", true);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            step7InsertScheduleStatusEvent.Errors.Add(new Error(e));
                                            response.Errors.Add(new Error(e.Message, false));
                                        }
                                        finally
                                        {
                                            step7InsertScheduleStatusEvent.Done();
                                            response.ProcessSteps.Add(step7InsertScheduleStatusEvent);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    step6UpdateShopFloor.Errors.Add(new Error(e));
                                    response.Errors.Add(new Error(e.Message, false));
                                    //BoxingScheduleManager.RollBack(shopFloorTran);
                                }
                                finally
                                {
                                    step6UpdateShopFloor.Done();
                                    response.ProcessSteps.Add(step6UpdateShopFloor);
                                }
                            } // if (!step5WriteE1Boxing.HasErrors())
                        } // if (!step4BuildInterops.HasErrors())
                        else
                        {
                            step4BuildInterops.RowsAffected = 1;
                        }
                    } // if (!step3ValidateBoxingItems.HasErrors() && (boxingSchedule.IsValid || force))
                    else
                    {
                        if (boxingSchedule.ValidationStatus.Contains("Previously Uploaded"))
                        {
                            BoxingScheduleManager.UpdateSFPreviouslyUploaded(cnSF, scheduleNum);
                            ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, boxingSchedule.IncomingOrdersMinScheduleDate, "BOXED");
                        }
                    }
                } // if (!step2GetShopFloorBoxingItems.HasErrors())
                else
                {
                    step2GetShopFloorBoxingItems.RowsAffected = 1;
                }
                response.Close();
                SummarizeBoxingSchedule(response, boxingSchedule, force);
                NotifyBoxingSchedule(response, boxingSchedule, force);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void NotifyBoxingSchedule(ProcessResponse response, BoxingSchedule boxingSchedule, bool force)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.BoxingScheduleUploadReport.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] MTO Schedule {boxingSchedule.ScheduleNum} Boxing Upload";
                if (force) subject = $@"Force - {subject}";
                if (response.HasErrors() || boxingSchedule.ValidationErrors.Any()) subject = $"Error - {subject}";

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
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void SummarizeBoxingSchedule(ProcessResponse response, BoxingSchedule boxingSchedule, bool force)
        {
            var stepSummarizeUpload = new ProcessStep("Schedule Summary Collections");
            try
            {
                if (boxingSchedule != null)
                {
                    var scheduleHeaderSummary = new
                    {
                        Location = $"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                        boxingSchedule.ScheduleNum,
                        boxingSchedule.TransferBatchNum,
                        UnitsCount = Convert.ToInt16(boxingSchedule.BoxItems?.Count),
                        WorkOrdersCount = Convert.ToInt16(boxingSchedule.BoxItems?.Select(x => x.WorkOrderNum).Distinct().Count()),
                        BoxesCount = Convert.ToInt16(boxingSchedule.BoxItems?.Select(x => x.BoxNum).Distinct().Count()),
                        boxingSchedule.TransferStatus,
                        Force = force,
                        HasErrors = response.HasErrors() || boxingSchedule.ValidationErrors.Any(),
                        StartDate = response.ProcessSteps.Min(x => x.StartDate),
                        StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        EndDate = response.ProcessSteps.Max(x => x.EndDate),
                        EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add(new SerializedObject("ScheduleSummaryNode", XmlSerializationUtility.SerializeAnonymousType(scheduleHeaderSummary, "ScheduleHeader")));

                    var actionLinks = new ActionLinks();
                    actionLinks.Add(new ActionLink("Force", "Force Upload ... Upload with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.BoxingRestService.svc/UploadBoxingSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={boxingSchedule.ScheduleNum}&Force=true"));
                    actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={boxingSchedule.ScheduleNum}"));
                    response.SerializedObjects.Add("ActionLinks", actionLinks);

                    var unitboxSummary = new
                    {
                        boxingSchedule.UnitBoxCount,
                        UnitBoxMaxTimeStamp = boxingSchedule.UnitBoxMaxTimeStamp.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        boxingSchedule.IncomingOrdersUnitCount,
                        boxingSchedule.IncomingOrdersWorkOrderCount,
                        IncomingOrdersMaxDownLoadDate = boxingSchedule.IncomingOrdersMaxDownLoadDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"))
                    };
                    response.SerializedObjects.Add(new SerializedObject("UnitBoxSummaryNode", XmlSerializationUtility.SerializeAnonymousType(unitboxSummary, "UnitBoxHeader")));

                    if (boxingSchedule.BoxItems != null)
                    {
                        var boxes = from i in boxingSchedule.BoxItems
                            group i by new
                            {
                                i.BoxNum,
                                i.RackType,
                                i.RackTypeDesc,
                                i.Customer,
                                i.IncomingOrdersShipToCustNum
                            }
                            into g
                            select new
                            {
                                Keys = g.Key,
                                Values = g,
                                g.Key.BoxNum,
                                g.Key.RackType,
                                g.Key.RackTypeDesc,
                                CustomerName = g.Key.Customer,
                                g.Key.IncomingOrdersShipToCustNum,
                                MinUnitID = g.Min(x => x.UnitId),
                                MaxUnitID = g.Max(x => x.UnitId),
                                MinDueDate = g.Min(x => x.UnitBoxDueDate),
                                MaxDueDate = g.Max(x => x.UnitBoxDueDate),
                                UnitsQty = g.Count(),
                                MinWorkOrderNum = g.Min(x => x.WorkOrderNum),
                                MaxWorkOrderNum = g.Max(x => x.WorkOrderNum)
                            };

                        var scheduleCustomerLines = boxingSchedule.BoxItems.GroupBy(x => new
                        {
                            x.BoxNum,
                            x.RackType,
                            x.RackTypeDesc,
                            x.Customer,
                            x.IncomingOrdersShipToCustNum
                        }).Select(ng => new
                        {
                            ng.Key.BoxNum,
                            ng.Key.RackType,
                            ng.Key.RackTypeDesc,
                            CustomerName = ng.Select(x => x.Customer).FirstOrDefault()?.Trim(),
                            IncomingOrdersShipToCustNum = ng.Select(x => x.IncomingOrdersShipToCustNum).FirstOrDefault(),
                            MinUnitID = ng.Select(x => x.UnitId).Min(),
                            MaxUnitID = ng.Select(x => x.UnitId).Max(),
                            MinDueDate = ng.Select(x => x.UnitBoxDueDate).Min(),
                            MaxDueDate = ng.Select(x => x.UnitBoxDueDate).Max(),
                            UnitsQty = (ng.Count()),
                            MinWorkOrderNum = ng.Select(x => x.WorkOrderNum).Min(),
                            MaxWorkOrderNum = ng.Select(x => x.WorkOrderNum).Max()
                        }).ToList();
                        response.SerializedObjects.Add("ScheduleCustomerLine", scheduleCustomerLines, "ScheduleCustomerLines");
                        stepSummarizeUpload.RowsAffected = scheduleCustomerLines.Count;
                    } // if (boxingSchedule.BoxItems != null)
                } // if (boxingSchedule != null)
            }
            catch (Exception e)
            {
                stepSummarizeUpload.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeUpload.Done();
                response.ProcessSteps.Add(stepSummarizeUpload);
            }
        }

        private static void SuccessE1(BoxingSchedule boxingSchedule)
        {
            try
            {
                foreach (var boxInterop in boxingSchedule.BoxF580200.BoxInterops)
                {
                    boxInterop.TransferStatus = "Y";
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        //private static void FailE1(BoxingSchedule boxingSchedule)
        //{
        //    try
        //    {
        //        foreach (var boxInterop in boxingSchedule.BoxF580200.BoxInterops)
        //        {
        //            boxInterop.TransferStatus = "E";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ApplicationException(e.ToString());
        //    }
        //}

        private static void SuccessSF(BoxingSchedule boxingSchedule)
        {
            try
            {
                boxingSchedule.Transferred = true;
                boxingSchedule.TransferStatus = "Y";
                boxingSchedule.TransferDate = DateTime.Now.ToLocalTime();
                boxingSchedule.TransferDateString = boxingSchedule.TransferDate.ToString("M/dd/yyyy hh:mm:ss tt");
                foreach (var boxItem in boxingSchedule.BoxItems)
                {
                    boxItem.Transferred = true;
                    boxItem.TransferStatus = "Y";
                    boxItem.TransferDate = boxingSchedule.TransferDate;
                    boxItem.TransferDateString = boxingSchedule.TransferDateString;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }
    }
}