using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Enum;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
using NLog.LayoutRenderers.Wrappers;

// ReSharper disable InconsistentNaming

namespace IGServices.ScheduleSvc
{
    public class ScheduleProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************/
        public static ProcessResponse DownloadMtoSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTO Schedule Download Processor");
            try
            {
                var events = new List<ScheduleEvent>();

                // Parms
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var status = Data.UnboxParm.To<string>(parms, "Status", String.Empty);
                var force = Data.UnboxParm.To<bool>(parms, "Force", false);
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);

                // Resolve run span
                var step0 = new ProcessStep("Resolve Run Span");
                ProcessRunSpan processRunSpan = new ProcessRunSpan();
                HighWaterMark highWaterMark = new HighWaterMark();
                try
                {
                    highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "MTOSchedule") : null;
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                    step0.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
                }
                catch (Exception e)
                {
                    step0.Errors.Add(new Error(e));
                }
                finally
                {
                    step0.Done();
                    response.AddStep(step0);
                }

                // Get Events
                var step1 = new ProcessStep($"Get {plant.PlantCode} Unprocessed F580020 Records");
                try
                {
                    events = ScheduleEventManager.GetEvents(session, plant, new ProcessRunSpan(), status, scheduleNum, new List<string> { "2", "7" });
                    step1.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                step1.Done();
                response.AddStep(step1);

                // Reserve Events
                if (events.Any())
                {
                    var step2 = new ProcessStep($"Reserve {plant.PlantCode} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            step2.SqlDmlResponses.Add(ScheduleEventManager.ReserveStatus(session, evt));
                            ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "R", "E1 Reserved");
                        }

                        step2.RowsAffected = step2.SqlDmlResponses.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step2.Done();
                    response.AddStep(step2);
                }

                // Process Events
                if (events.Any())
                {
                    var step3 = new ProcessStep($"Process {plant.PlantCode} Events");
                    try
                    {
                        var mtoManager = new MTOScheduleDownloadManager(session);
                        foreach (ScheduleEvent evt in events)
                        {
                            evt.TransferStatusCode = "R";
                            var scheduleResponse = mtoManager.Process(session, sqlCns, plant, evt, force);
                            step3.ProcessResponses.Add(scheduleResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step3.Done();
                    response.AddStep(step3);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        /********************************************************/
        public static ProcessResponse DownloadMtsSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTS Schedule Download Processor");
            try
            {
                var events = new List<ScheduleEvent>();

                // Parms
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var status = Data.UnboxParm.To<string>(parms, "Status", String.Empty);
                var force = Data.UnboxParm.To<bool>(parms, "Force", false);
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);

                // Resolve run span
                var step0 = new ProcessStep("Resolve Run Span");
                ProcessRunSpan processRunSpan = new ProcessRunSpan();
                HighWaterMark highWaterMark = new HighWaterMark();
                try
                {
                    highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "MTOSchedule") : null;
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                    step0.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
                }
                catch (Exception e)
                {
                    step0.Errors.Add(new Error(e));
                }
                finally
                {
                    step0.Done();
                    response.AddStep(step0);
                }

                // Get Events
                var step1 = new ProcessStep($"Get {plant.PlantCode} Unprocessed F580020 Records");
                try
                {
                    events = ScheduleEventManager.GetEvents(session, plant, new ProcessRunSpan(), status, scheduleNum, new List<string> { "6" });
                    step1.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                step1.Done();
                response.AddStep(step1);

                // Reserve Events
                if (events.Any())
                {
                    var step2 = new ProcessStep($"Reserve {plant.PlantCode} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            step2.SqlDmlResponses.Add(ScheduleEventManager.ReserveStatus(session, evt));
                            ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "R", "E1 Reserved");
                        }

                        step2.RowsAffected = step2.SqlDmlResponses.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step2.Done();
                    response.AddStep(step2);
                }

                // Process Events
                if (events.Any())
                {
                    var step3 = new ProcessStep($"Process {plant.PlantCode} Events");
                    var mgr = new MTSScheduleDownloadManager(session);
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            var scheduleResponse = mgr.Process(session, sqlCns, plant, evt, force);
                            step3.ProcessResponses.Add(scheduleResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step3.Done();
                    response.AddStep(step3);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        /********************************************************/
        public static ProcessResponse DownloadScheduledOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTS Schedule Download Processor");
            try
            {
                // Parms
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var status = Data.UnboxParm.To<string>(parms, "Status", String.Empty);

                /********************************************************/
                var stepRunSpan = new ProcessStep("Resolve Run Span");
                ProcessRunSpan processRunSpan = new ProcessRunSpan();
                HighWaterMark highWaterMark = new HighWaterMark();
                try
                {
                    highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "ScheduledOrder") : null;
                    processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                    stepRunSpan.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
                }
                catch (Exception e)
                {
                    stepRunSpan.Errors.Add(new Error(e));
                }
                finally
                {
                    stepRunSpan.Done();
                    response.AddStep(stepRunSpan);
                }

                // Get MTS Events
                var events = new List<ScheduleEvent>();
                var stepMtsEvents = new ProcessStep($"Get {plant.PlantCode} MTS Events");
                try
                {
                    events = ScheduleEventManager.GetEvents(session, plant, processRunSpan, status, scheduleNum, new List<string> { "6", "7" });
                    stepMtsEvents.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    stepMtsEvents.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                stepMtsEvents.Done();
                response.AddStep(stepMtsEvents);

                // Get HV MTO Events
                var stepHVMTOEvents = new ProcessStep($"Get {plant.PlantCode} HV MTO Events");
                try
                {
                    var mtoEvents = ScheduleEventManager.GetHvMtoEvents(session, plant, processRunSpan, status, scheduleNum);
                    stepHVMTOEvents.RowsAffected = events.Count;
                    events.AddRange(mtoEvents);
                }
                catch (Exception e)
                {
                    stepHVMTOEvents.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                stepHVMTOEvents.Done();
                response.AddStep(stepHVMTOEvents);

                // Reserve Events
                //if (events.Any())
                //{
                //    var step2 = new ProcessStep($"Reserve {plant.PlantCode} Events");
                //    try
                //    {
                //        foreach (ScheduleEvent evt in events)
                //        {
                //            step2.SqlDmlResponses.Add(ScheduleEventManager.ReserveStatus(session, evt));
                //        }

                //        step2.RowsAffected = step2.SqlDmlResponses.Count;
                //    }
                //    catch (Exception e)
                //    {
                //        step2.Errors.Add(new Error(e));
                //        Logger.Error(e.Message);
                //    }

                //    step2.Done();
                //    response.AddStep(step2);
                //}

                // Process Events
                if (events.Any())
                {
                    var stepProcessEvents = new ProcessStep($"Process {plant.PlantCode} Events");
                    var mgr = new ScheduledOrderDownloadManager(session);
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            var scheduleResponse = mgr.Process(session, sqlCns, plant, evt, false);
                            stepProcessEvents.ProcessResponses.Add(scheduleResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        stepProcessEvents.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    stepProcessEvents.Done();
                    response.AddStep(stepProcessEvents);
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors())
                {
                    var stepFinalMark = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (events.Count > 0)
                        {
                            highWaterMark.LocalDateTime = events.Max(x => x.TransactionDate).AddHours(plant.TimeOffset);
                            highWaterMark.EnterpriseDateTime = events.Max(x => x.TransactionDate);
                        }
                        stepFinalMark.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e)
                    {
                        stepFinalMark.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        stepFinalMark.Done();
                        response.AddStep(stepFinalMark);
                    }
                }

                /********************************************************/
                //if (events.Any())
                //{
                //    var stepRackIds = new ProcessStep($"Process {plant.PlantCode} Stock Rack Ids");
                //    try
                //    {
                //        var mgr = new StockRackDownloadManager(session);
                //        foreach (ScheduleEvent evt in events)
                //        {
                //            if (evt.ShopFloorSystem == "6")
                //            {
                //                stepRackIds.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan,
                //                    useHighWaterMark, evt.ScheduleNum, new List<decimal>()));
                //            }
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        stepRackIds.Errors.Add(new Error(e));
                //        Logger.Error(e.Message);
                //    }
                //    stepRackIds.Done();
                //    response.AddStep(stepRackIds);
                //}

            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse BackoutSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Schedule Backout Upload Processor");
            try
            {
                var events = new List<ScheduleEvent>();
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);

                // Get Events
                var step1 = new ProcessStep($"Get {plant.PlantCode} Unprocessed Backout Records");
                try
                {
                    if (scheduleNum > 0)
                    {
                        events = ScheduleBackoutEventManager.GetEvent(sqlCns["PLANT.SHOPFLOOR"], plant, scheduleNum);
                        if (events.Count == 0)
                        {
                            throw new Exception($"{plant.PlantCode} Schedule {scheduleNum} does NOT exist in EBS.Schedule");
                        }
                    }
                    else
                    {
                        events = ScheduleBackoutEventManager.GetEvents(sqlCns["PLANT.SHOPFLOOR"], plant, "N", scheduleNum);
                    }
                    step1.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                step1.Done();
                response.AddStep(step1);

                // Reserve Events
                if (events.Any())
                {
                    var step2 = new ProcessStep($"Reserve {plant.PlantCode} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            step2.SqlDmlResponses.Add(ScheduleBackoutEventManager.ReserveStatus(sqlCns["PLANT.SHOPFLOOR"], evt));
                        }
                        step2.RowsAffected = step2.SqlDmlResponses.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step2.Done();
                    response.AddStep(step2);
                }

                // Process Events
                if (events.Any())
                {
                    var step3 = new ProcessStep($"Process {plant.PlantCode} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            var scheduleResponse = ScheduleBackoutUploadManager.Process(session, sqlCns, plant, evt);
                            step3.ProcessResponses.Add(scheduleResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step3.Done();
                    response.AddStep(step3);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


        public static ProcessResponse UpdateScheduleStatus(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Schedule Reset Upload Processor");
            try
            {
                var events = new List<ScheduleEvent>();
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var updateType = Data.UnboxParm.To<Schedule.StatusUpdateEvent>(parms, "UpdateType", Schedule.StatusUpdateEvent.Default);

                // Get Events
                var step1 = new ProcessStep($"Get {plant.PlantCode} Schedule Event Records");
                try
                {
                    if (scheduleNum > 0)
                    {
                        events = ScheduleEventManager.GetEvents(session, plant, new ProcessRunSpan(), null, scheduleNum, new List<string> { });
                        if (events.Count == 0)
                        {
                            throw new Exception($"{plant.PlantCode} Schedule {scheduleNum} does NOT exist in E1");
                        }
                    }
                    else
                    {
                        throw new Exception("No Schedule Requested");
                    }
                    step1.RowsAffected = events.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                    Logger.Error(e.Message);
                }
                step1.Done();
                response.AddStep(step1);

                // Reset Events
                if (events.Any())
                {
                    var step2 = new ProcessStep($"Process {plant.PlantCode} {updateType.ToString()} Events");
                    try
                    {
                        foreach (ScheduleEvent evt in events)
                        {
                            step2.ProcessResponses.Add(ScheduleStatusUploadManager.Process(session, sqlCns, plant, evt, updateType));
                        }
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                        Logger.Error(e.Message);
                    }

                    step2.Done();
                    response.AddStep(step2);
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


        /********************************************************/
        public static ProcessResponse DownloadStockRacks(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Stock Rack Request Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var workOrders = Data.UnboxParm.To(parms, "WorkOrders", new List<decimal>());
                var mgr = new StockRackDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, scheduleNum, workOrders);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        /********************************************************/
        public static ProcessResponse PrintStockTag(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Stock Rack Request Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var scheduleNum = Data.UnboxParm.To<decimal>(parms, "ScheduleNum", 0);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var workOrders = Data.UnboxParm.To(parms, "WorkOrders", new List<decimal>());
                var mgr = new StockTagPrintManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, scheduleNum, rackId, workOrders);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

    }
}
