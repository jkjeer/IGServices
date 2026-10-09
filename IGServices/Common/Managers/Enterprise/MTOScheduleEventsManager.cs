using System;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using IGServices.ScheduleSvc;
using NLog;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Enterprise
{
    public class MTOScheduleEventsManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string shopFloorSystemIGMTO = "2";

        public static MTOScheduleEvents GetUnprocessedMTOScheduleEvents(BusinessSystemSession e1Session, Plant plant, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTO Schedule", true);
            var mtoScheduleEvents = new MTOScheduleEvents();
            var step1GetUnprocessedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Get Unprocessed F580020s");
            try
            {
                var f580020s = F580020Manager.GetUnprocessedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTO);
                step1GetUnprocessedF580020s.RowsAffected = f580020s.Count;
                step1GetUnprocessedF580020s.Done();
                processResponse.AddStep(step1GetUnprocessedF580020s);
                if (f580020s.Any())
                {
                    var step2BuildMTOScheduleEventsFromF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Build MTO Schedule Events From F580020s");
                    mtoScheduleEvents = BuildMTOScheduleEventsFromF580020s(f580020s);
                    step2BuildMTOScheduleEventsFromF580020s.RowsAffected = f580020s.Count;
                    step2BuildMTOScheduleEventsFromF580020s.Done();
                    processResponse.AddStep(step2BuildMTOScheduleEventsFromF580020s);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {mtoScheduleEvents.Count} ROWS");
            }
            processResponse.Done();
            returnResponse = processResponse;
            return mtoScheduleEvents;
        }

        public static MTOScheduleEvents ReserveUnprocessedMTOScheduleEvents(BusinessSystemSession e1Session, Plant plant, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTO Schedule", true);
            var mtoScheduleEvents = new MTOScheduleEvents();
            var step1ReserveUnprocessedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Reserve Unprocessed F580020s");
            try
            {
                step1ReserveUnprocessedF580020s.RowsAffected = F580020Manager.ReserveUnprocessedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTO);
                step1ReserveUnprocessedF580020s.Done();
                processResponse.AddStep(step1ReserveUnprocessedF580020s);
                //if (step1ReserveUnprocessedF580020s.RowsAffected > 0)
                //{
                    var step2GetReservedF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Get Reserved F580020s");
                    var f580020s = F580020Manager.GetReservedF580020s(e1Session, plant.PlantNum, shopFloorSystemIGMTO);
                    step2GetReservedF580020s.RowsAffected = f580020s.Count;
                    step2GetReservedF580020s.Done();
                    processResponse.AddStep(step2GetReservedF580020s);
                    if (f580020s.Any())
                    {
                        var step3BuildMTOScheduleEventsFromF580020s = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Build MTO Schedule Events From F580020s");
                        mtoScheduleEvents = BuildMTOScheduleEventsFromF580020s(f580020s);
                        step3BuildMTOScheduleEventsFromF580020s.RowsAffected = f580020s.Count;
                        step3BuildMTOScheduleEventsFromF580020s.Done();
                        processResponse.AddStep(step3BuildMTOScheduleEventsFromF580020s);
                    }
                //}
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {mtoScheduleEvents.Count} ROWS");
            }
            processResponse.Done();
            returnResponse = processResponse;
            return mtoScheduleEvents;
        }

        public static ProcessResponse CancelMTOSchedule(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Cancel Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "E";
                        mtoSchedule.TransferMessage = "Canceled";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2CancelMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel MTO Schedule ({scheduleNum})");
                    try
                    {
                        step2CancelMTOSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2CancelMTOSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2CancelMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2CancelMTOSchedule.Done();
                        response.ProcessSteps.Add(step2CancelMTOSchedule);
                    }

                    var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                    try
                    {
                        step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "CANCLD");
                    }
                    catch (Exception e)
                    {
                        step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step6InsertScheduleStatusEvent.Done();
                        response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                    }
                }
                response.Done();
                SummarizeMTOScheduleCancel(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleCancel(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse BoxedMTOScheduleEvent(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Boxed Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "B";
                        mtoSchedule.TransferMessage = "Boxed";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2BoxedMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Boxed MTO Schedule ({scheduleNum})");
                    try
                    {
                        step2BoxedMTOSchedule.RowsAffected = F580020Manager.BoxedF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2BoxedMTOSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2BoxedMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2BoxedMTOSchedule.Done();
                        response.ProcessSteps.Add(step2BoxedMTOSchedule);
                    }

                    var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                    try
                    {
                        step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "BOXED");
                    }
                    catch (Exception e)
                    {
                        step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step6InsertScheduleStatusEvent.Done();
                        response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                    }
                }
                response.Done();
                SummarizeMTOScheduleBoxed(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleBoxed(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse ResetMTOSchedule(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Reset Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "N";
                        mtoSchedule.TransferMessage = "Reset";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2ResetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Reset MTO Schedule ({scheduleNum})");
                    try
                    {
                        step2ResetMTOSchedule.RowsAffected = F580020Manager.ResetF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2ResetMTOSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2ResetMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2ResetMTOSchedule.Done();
                        response.ProcessSteps.Add(step2ResetMTOSchedule);
                    }

                    var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                    try
                    {
                        step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "RESET");
                    }
                    catch (Exception e)
                    {
                        step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step6InsertScheduleStatusEvent.Done();
                        response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                    }
                }
                response.Done();
                SummarizeMTOScheduleReset(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleReset(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse SuccessMTOSchedule(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Success Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "Y";
                        mtoSchedule.TransferMessage = "Success";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2SuccessMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Success MTO Schedule ({scheduleNum})");
                    try
                    {
                        step2SuccessMTOSchedule.RowsAffected = F580020Manager.SuccessF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2SuccessMTOSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2SuccessMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2SuccessMTOSchedule.Done();
                        response.ProcessSteps.Add(step2SuccessMTOSchedule);
                    }

                    //var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                    //try
                    //{
                    //    step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "SUCCES");
                    //}
                    //catch (Exception e)
                    //{
                    //    step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                    //}
                    //finally
                    //{
                    //    step6InsertScheduleStatusEvent.Done();
                    //    response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                    //}
                }
                response.Done();
                SummarizeMTOScheduleSuccess(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleSuccess(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse CompleteMTOSchedule(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Complete Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "C";
                        mtoSchedule.TransferMessage = "Complete";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    var step2CompleteMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Complete MTO Schedule ({scheduleNum})");
                    try
                    {
                        step2CompleteMTOSchedule.RowsAffected = F580020Manager.CompleteF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                        response.RowsAffected = step2CompleteMTOSchedule.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2CompleteMTOSchedule.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2CompleteMTOSchedule.Done();
                        response.ProcessSteps.Add(step2CompleteMTOSchedule);
                    }

                    //var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                    //try
                    //{
                    //    step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "COMPLT");
                    //}
                    //catch (Exception e)
                    //{
                    //    step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                    //}
                    //finally
                    //{
                    //    step6InsertScheduleStatusEvent.Done();
                    //    response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                    //}
                }
                response.Done();
                SummarizeMTOScheduleComplete(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleComplete(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static ProcessResponse BackoutMTOSchedule(BusinessSystemSession e1Session, SqlConnection cnInt, SqlConnection cnSF, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($@"Backout Plant {plant.PlantCode} - {plant.PlantNum} MTO Schedule {scheduleNum}", plant, true);
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnInt", cnInt.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                var step1GetMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get MTO Schedule ({scheduleNum})");
                try
                {
                    ProcessResponse childResponse;
                    mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
                    step1GetMTOSchedule.ProcessResponses.Add(childResponse);
                    if (mtoSchedule.MTOScheduleLines is null)
                    {
                        step1GetMTOSchedule.Errors.Add(new Error($@"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} not found in E1 F580020"));
                        mtoSchedule.TransferStatus = "";
                        mtoSchedule.TransferMessage = "Error";
                    }
                    else
                    {
                        step1GetMTOSchedule.RowsAffected = mtoSchedule.MTOScheduleLines.Count;
                        mtoSchedule.TransferStatus = "E";
                        mtoSchedule.TransferMessage = "Backed Out";
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOSchedule.Errors.Add(new Error(e));
                }
                step1GetMTOSchedule.Done();
                response.ProcessSteps.Add(step1GetMTOSchedule);

                if (!step1GetMTOSchedule.HasErrors())
                {
                    //var step2CancelMTOSchedule = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Cancel MTO Schedule ({scheduleNum})");
                    //try
                    //{
                    //    step2CancelMTOSchedule.RowsAffected = F580020Manager.CancelF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                    //    response.RowsAffected = step2CancelMTOSchedule.RowsAffected;
                    //}
                    //catch (Exception e)
                    //{
                    //    step2CancelMTOSchedule.Errors.Add(new Error(e));
                    //}
                    //finally
                    //{
                    //    step2CancelMTOSchedule.Done();
                    //    response.ProcessSteps.Add(step2CancelMTOSchedule);
                    //}

                    // Get E1 Required Sequences
                    long sfukid = 0;
                    long sfedbt = 0;
                    long qaukid = 0;
                    var step3GetSequences = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get Sequences");
                    try
                    {
                        sfukid = SequenceManager.GetRange(cnInt, "E1.SFUKID", 1);
                        sfedbt = SequenceManager.GetNextVal(cnInt, "E1.SFEDBT");
                        qaukid = SequenceManager.GetNextVal(cnInt, "E1.QAUKID");
                        step3GetSequences.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step3GetSequences.Errors.Add(new Error(e));
                    }
                    step3GetSequences.Done();
                    response.AddStep(step3GetSequences);

                    if (!step3GetSequences.HasErrors())
                    {
                        var step3InsertE1F580030 = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert E1 F580030");
                        try
                        {
                            SqlDmlResponse sqlResponse = F580030Manager.InsertScheduleBackout(e1Session, plant.PlantNum, scheduleNum, sfukid, sfedbt, mtoSchedule.ScheduleDateE1);
                            response.SqlDmlResponses.Add(sqlResponse);
                            step3InsertE1F580030.RowsAffected = sqlResponse.RowsAffected;
                        }
                        catch (Exception e)
                        {
                            step3InsertE1F580030.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3InsertE1F580030.Done();
                            response.ProcessSteps.Add(step3InsertE1F580030);
                        }

                        if (!step3InsertE1F580030.HasErrors())
                        {
                            var step4InsertE1F580030A = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert E1 F580030A");
                            try
                            {
                                SqlDmlResponse sqlResponse = F580030AManager.InsertScheduleBackout(e1Session, plant.PlantNum, scheduleNum, qaukid, sfedbt);
                                response.SqlDmlResponses.Add(sqlResponse);
                                step4InsertE1F580030A.RowsAffected = sqlResponse.RowsAffected;
                            }
                            catch (Exception e)
                            {
                                step4InsertE1F580030A.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4InsertE1F580030A.Done();
                                response.ProcessSteps.Add(step4InsertE1F580030A);
                            }

                            if (!step4InsertE1F580030A.HasErrors())
                            {
                                var step5DeleteIncomingOrders = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Delete ShopFloor Incoming Orders");
                                try
                                {
                                    SqlDmlResponse sqlResponse = MTOIncomingOrderManager.DeleteIncomingOrders(cnSF, scheduleNum, mtoSchedule.ScheduleDate);
                                    response.SqlDmlResponses.Add(sqlResponse);
                                    step5DeleteIncomingOrders.RowsAffected = sqlResponse.RowsAffected;
                                }
                                catch (Exception e)
                                {
                                    step5DeleteIncomingOrders.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step5DeleteIncomingOrders.Done();
                                    response.ProcessSteps.Add(step5DeleteIncomingOrders);
                                }

                                if (!step5DeleteIncomingOrders.HasErrors())
                                {
                                    var step6InsertScheduleStatusEvent = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Insert ShopFloor Schedule Status Event");
                                    try
                                    {
                                        step6InsertScheduleStatusEvent.RowsAffected = ScheduleStatusEventsManager.InsertScheduleStatusEvent(cnSF, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDate, "BAKOUT");
                                    }
                                    catch (Exception e)
                                    {
                                        step6InsertScheduleStatusEvent.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step6InsertScheduleStatusEvent.Done();
                                        response.ProcessSteps.Add(step6InsertScheduleStatusEvent);
                                    }
                                    mtoSchedule.TransferStatus = "Y";
                                }
                            }
                        }
                    }
                }
                response.Done();
                SummarizeMTOScheduleBackout(response, mtoSchedule);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                response.Close();
                MTOScheduleProcessor.NotifyMTOScheduleBackout(response, scheduleNum);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {response.RowsAffected} ROWS");
            }
            return response;
        }

        public static void SummarizeMTOScheduleBackout(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    TransferDate = DateTime.Now.ToLocalTime(),
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static void SummarizeMTOScheduleCancel(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    mtoSchedule.TransferDate,
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static void SummarizeMTOScheduleBoxed(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    mtoSchedule.TransferDate,
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static void SummarizeMTOScheduleReset(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    TransferDate = DateTime.Now.ToLocalTime(),
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static void SummarizeMTOScheduleSuccess(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    TransferDate = DateTime.Now.ToLocalTime(),
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static void SummarizeMTOScheduleComplete(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate,
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                var transferSummary = new
                {
                    mtoSchedule.TransferStatus,
                    TransferDate = DateTime.Now.ToLocalTime(),
                    mtoSchedule.TransferMessage,
                    IsParallel = false,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal) (response.ProcessSteps.Max(x => x.EndDate) - response.StartDate).TotalSeconds,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Download ... Download with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/DownloadMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Reset", "Reset Schedule Status ... F580020 EDSP = N", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/ResetSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Complete", "Complete Schedule Status ... F580020 EDSP = C", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CompleteSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Cancel", "Error Schedule Status ... F580020 EDSP = E", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/CancelSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
                actionLinks.Add(new ActionLink("Success", "Success Schedule Status ... F580020 EDSP = Y", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/SuccessSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={mtoSchedule.ScheduleNum}"));
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

        public static MTOScheduleEvents BuildMTOScheduleEventsFromF580020s(F580020s f580020s)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var scheduleEvents = new MTOScheduleEvents();
            try
            {
                foreach (var f580020 in f580020s)
                {
                    var scheduleEvent = new MTOScheduleEvent
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
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  {scheduleEvents.Count} ROWS");
            }
            return scheduleEvents;
        }

        public static MTOScheduleEvent BuildMTOScheduleEventFromF580020(F580020 f580020)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            MTOScheduleEvent scheduleEvent;
            try
            {
                scheduleEvent = new MTOScheduleEvent
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
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS  1 ROW");
            }
            return scheduleEvent;
        }
    }
}