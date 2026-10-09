using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using IGServices.ScheduleSvc;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Enterprise
{
    public class MTSScheduleEventsManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string shopFloorSystemIGMTS = "6";

        public static MTSScheduleEvents GetUnprocessedMTSScheduleEvents(BusinessSystemSession e1Session, Plant plant, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTS Schedule", true);
            var step1GetUnprocessedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Get Unprocessed F580020s");
            var mtsScheduleEvents = new MTSScheduleEvents();
            try
            {
                var f580020s = F580020Manager.GetUnprocessedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTS);
                step1GetUnprocessedF580020s.RowsAffected = f580020s.Count;
                step1GetUnprocessedF580020s.Done();
                processResponse.AddStep(step1GetUnprocessedF580020s);
                if (f580020s.Any())
                {
                    var step2BuildMTSScheduleEventsFromF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Build MTS Schedule Events From F580020s");
                    mtsScheduleEvents = BuildMTSScheduleEventsFromF580020s(f580020s);
                    step2BuildMTSScheduleEventsFromF580020s.RowsAffected = f580020s.Count;
                    step2BuildMTSScheduleEventsFromF580020s.Done();
                    processResponse.AddStep(step2BuildMTSScheduleEventsFromF580020s);
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            processResponse.Done();
            returnResponse = processResponse;
            return mtsScheduleEvents;
        }

        public static MTSScheduleEvents ReserveUnprocessedMTSScheduleEvents(BusinessSystemSession e1Session, Plant plant, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTS Schedule", true);
            var mtsScheduleEvents = new MTSScheduleEvents();
            var step1ReserveUnprocessedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Reserve Unprocessed F580020s");
            try
            {
                step1ReserveUnprocessedF580020s.RowsAffected = F580020Manager.ReserveUnprocessedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTS);
                step1ReserveUnprocessedF580020s.Done();
                processResponse.AddStep(step1ReserveUnprocessedF580020s);
                if (step1ReserveUnprocessedF580020s.RowsAffected > 0)
                {
                    var step2GetReservedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Get Reserved F580020s");
                    var f580020s = F580020Manager.GetReservedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTS);
                    step2GetReservedF580020s.RowsAffected = f580020s.Count;
                    step2GetReservedF580020s.Done();
                    processResponse.AddStep(step2GetReservedF580020s);
                    if (f580020s.Any())
                    {
                        var step3BuildMTSScheduleEventsFromF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Build MTS Schedule Events From F580020s");
                        mtsScheduleEvents = BuildMTSScheduleEventsFromF580020s(f580020s);
                        step3BuildMTSScheduleEventsFromF580020s.RowsAffected = f580020s.Count;
                        step3BuildMTSScheduleEventsFromF580020s.Done();
                        processResponse.AddStep(step3BuildMTSScheduleEventsFromF580020s);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {mtsScheduleEvents.Count} ROWS");
            }
            processResponse.Done();
            returnResponse = processResponse;
            return mtsScheduleEvents;
        }

        public static ProcessResponse CancelMTSSchedule(BusinessSystemSession e1Session, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Cancel Plant {plant.PlantCode} - {plant.PlantNum} MTS Schedule {scheduleNum}", plant, true);
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    if (mtsSchedule.MTSScheduleLines is null)
                    {
                        step1GetMTSSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtsSchedule.TransferStatus = "";
                        mtsSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        mtsSchedule.TransferStatus = "E";
                        mtsSchedule.TransferMessage = "Canceled";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                if (!step1GetMTSSchedule.HasErrors())
                {
                    var step2CancelMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel MTS Schedule ({scheduleNum})");
                    try
                    {
                        step2CancelMTSSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2CancelMTSSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2CancelMTSSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2CancelMTSSchedule.Done();
                        response.ProcessSteps.Add(step2CancelMTSSchedule);
                    }
                }
                response.Done();
                SummarizeMTSScheduleCancel(response, mtsSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleCancel(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse ResetMTSSchedule(BusinessSystemSession e1Session, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Reset Plant {plant.PlantCode} - {plant.PlantNum} MTS Schedule {scheduleNum}", plant, true);
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    if (mtsSchedule.MTSIncomingOrderLines is null)
                    {
                        step1GetMTSSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtsSchedule.TransferStatus = "";
                        mtsSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSIncomingOrderLines.Count;
                        mtsSchedule.TransferStatus = "N";
                        mtsSchedule.TransferMessage = "Reset";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                if (!step1GetMTSSchedule.HasErrors())
                {
                    var step2ResetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Reset MTS Schedule ({scheduleNum})");
                    try
                    {
                        step2ResetMTSSchedule.RowsAffected = F580020Manager.ResetF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2ResetMTSSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2ResetMTSSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2ResetMTSSchedule.Done();
                        response.ProcessSteps.Add(step2ResetMTSSchedule);
                    }
                }
                response.Done();
                SummarizeMTSScheduleReset(response, mtsSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleReset(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse SuccessMTSSchedule(BusinessSystemSession e1Session, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Success Plant {plant.PlantCode} - {plant.PlantNum} MTS Schedule {scheduleNum}", plant, true);
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    if (mtsSchedule.MTSIncomingOrderLines is null)
                    {
                        step1GetMTSSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtsSchedule.TransferStatus = "";
                        mtsSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSIncomingOrderLines.Count;
                        mtsSchedule.TransferStatus = "Y";
                        mtsSchedule.TransferMessage = "Success";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                if (!step1GetMTSSchedule.HasErrors())
                {
                    var step2SuccessMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Success MTS Schedule ({scheduleNum})");
                    try
                    {
                        step2SuccessMTSSchedule.RowsAffected = F580020Manager.SuccessF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2SuccessMTSSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2SuccessMTSSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2SuccessMTSSchedule.Done();
                        response.ProcessSteps.Add(step2SuccessMTSSchedule);
                    }
                }
                response.Done();
                SummarizeMTSScheduleSuccess(response, mtsSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleSuccess(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse CompleteMTSSchedule(BusinessSystemSession e1Session, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Complete Plant {plant.PlantCode} - {plant.PlantNum} MTS Schedule {scheduleNum}", plant, true);
            var mtsSchedule = new MTSSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTS Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtsSchedule = MTSScheduleManager.BuildMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTSSchedule.ProcessResponses.Add(childResponse);
                    if (mtsSchedule.MTSScheduleLines is null)
                    {
                        step1GetMTSSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtsSchedule.TransferStatus = "";
                        mtsSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTSSchedule.RowsAffected = mtsSchedule.MTSScheduleLines.Count;
                        mtsSchedule.TransferStatus = "C";
                        mtsSchedule.TransferMessage = "Complete";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTSSchedule.Errors.Add(new Error(e));
                }
                step1GetMTSSchedule.Done();
                response.ProcessSteps.Add(step1GetMTSSchedule);

                var step2CompleteMTSSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Complete MTS Schedule ({scheduleNum})");
                try
                {
                    step2CompleteMTSSchedule.RowsAffected = F580020Manager.CompleteF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                    response.RowsAffected = step2CompleteMTSSchedule.RowsAffected;
                }
                catch (Exception e)
                {
                    step2CompleteMTSSchedule.Errors.Add(new Error(e));
                }
                finally
                {
                    step2CompleteMTSSchedule.Done();
                    response.ProcessSteps.Add(step2CompleteMTSSchedule);
                }
                response.Done();
                SummarizeMTSScheduleComplete(response, mtsSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTSScheduleProcessor.NotifyMTSScheduleComplete(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static void SummarizeMTSScheduleCancel(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.Customer).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.ScheduledQty).Sum()),
                    mtsSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    mtsSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                //actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadScheduleInfoMTSTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}&Force=true"));
                //actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
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

        public static void SummarizeMTSScheduleReset(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.Customer).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.ScheduledQty).Sum()),
                    mtsSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    mtsSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadScheduleInfoMTSTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}&Force=true"));
                //actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
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

        public static void SummarizeMTSScheduleSuccess(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.Customer).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.ScheduledQty).Sum()),
                    mtsSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    mtsSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadScheduleInfoMTSTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}&Force=true"));
                //actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
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

        public static void SummarizeMTSScheduleComplete(ProcessResponse response, MTSSchedule mtsSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtsSchedule.ScheduleNum,
                    mtsSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtsSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtsSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.Customer).Distinct().Count()),
                    Lines = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtsSchedule.MTSIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt32(mtsSchedule.MTSIncomingOrderLines.Select(x => x.ScheduledQty).Sum()),
                    mtsSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtsSchedule.TransferStatus,
                    mtsSchedule.TransferDate,
                    mtsSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtsSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadScheduleInfoMTSTable?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}&Force=true"));
                //actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessMTSScheduleEvent?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtsSchedule.ScheduleNum}"));
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

        public static MTSScheduleEvents BuildMTSScheduleEventsFromF580020s(F580020s f580020s)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var scheduleEvents = new MTSScheduleEvents();
            try
            {
                foreach (var f580020 in f580020s)
                {
                    var scheduleEvent = new MTSScheduleEvent
                    {
                        BatchNum = f580020.S6EDBT,
                        CostCenter = f580020.S6MCU,
                        ScheduleNum = f580020.S6Z58SCN,
                        ScheduleDateE1 = f580020.S6Z58SCD,
                        ScheduleDate = Utilities.Common.CastToDateTime(f580020.S6Z58SCD),
                        ShopFloorSystem = f580020.S6Z58SFS,
                        StatusCode = f580020.S6TSC,
                        ProcessFlag = f580020.S6EDSP,
                        MathNumeric06 = f580020.S6MATH06,
                        OperationStatus = f580020.S6WOPST,
                        Description = f580020.S6DS50,
                        PlantNum = f580020.S6MCU //PlantManager.GetPlant(f580020.S6MCU).PlantNum
                    };
                    scheduleEvents.Add(scheduleEvent);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{ e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return scheduleEvents;
        }

        public static MTSScheduleEvent BuildMTSScheduleEventFromF580020(F580020 f580020)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            MTSScheduleEvent scheduleEvent;
            try
            {
                scheduleEvent = new MTSScheduleEvent
                {
                    BatchNum = f580020.S6EDBT,
                    CostCenter = f580020.S6MCU,
                    ScheduleNum = f580020.S6Z58SCN,
                    ScheduleDateE1 = f580020.S6Z58SCD,
                    ScheduleDate = Utilities.Common.CastToDateTime(f580020.S6Z58SCD),
                    ShopFloorSystem = f580020.S6Z58SFS,
                    StatusCode = f580020.S6TSC,
                    ProcessFlag = f580020.S6EDSP,
                    MathNumeric06 = f580020.S6MATH06,
                    OperationStatus = f580020.S6WOPST,
                    Description = f580020.S6DS50,
                    PlantNum = f580020.S6MCU //PlantManager.GetPlant(f580020.S6MCU).PlantNum
                };
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{ e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return scheduleEvent;
        }
    }
}
