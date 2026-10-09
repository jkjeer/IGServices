using System;
using System.Collections.Generic;
using System.Linq;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Enum;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;

namespace IGServices.ScheduleSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]

    public class ScheduleService : IScheduleService
    {
        public MTOScheduleEvents GetUnprocessedMTOScheduleEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetUnprocessedMTOScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.GetUnprocessedMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CancelMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.CancelMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse BoxedMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.BoxedMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.ResetMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.SuccessMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.CompleteMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CancelMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CancelMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.SuccessMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CompleteMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse BackoutMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("4 digit Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.BackoutMTOSchedule(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTOSchedule GetMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetMTOSchedule(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTOIncomingOrderLines GetMTOIncomingOrderLines(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetMTOIncomingOrderLines(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadMTOIncomingOrdersTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.DownloadMTOIncomingOrdersTable(plantCode, scheduleNum, scheduleDateE1, batchNum, force);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadScheduleInfoMTSTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.DownloadScheduleInfoMTSTable(plantCode, scheduleNum, scheduleDateE1, batchNum, force);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public RawGlassItems GetRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0 && workOrderNum == 0)
            {
                throw new ApplicationException("Schedule Number or Work Order Number is REQUIRED.");
            }
            try
            {
                return RawGlassProcessor.GetRawGlassItems(plantCode, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0 && workOrderNum == 0)
            {
                throw new ApplicationException("Schedule Number or Work Order Number is REQUIRED.");
            }
            try
            {
                return RawGlassProcessor.DownloadRawGlassItems(plantCode, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadMTOSchedule(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, MTOScheduleProcessor.DownloadMTOSchedule, false, parms, resources);
                //return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadMtoSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMTSSchedule(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadMtsSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadScheduledOrders(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "PLANT.CLEARPATH", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadScheduledOrders, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadStockRacks(string plantCode, decimal scheduleNum = 0, decimal boxNum = 0, string workOrders = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", new TimeSpan(0)},
                    {"HighWaterMark", false},
                    {"ScheduleNum", scheduleNum},
                    {"BoxNum", boxNum},
                    {"WorkOrders", workOrders?.Split(',').Where(x => decimal.TryParse(x, out _)).Select(decimal.Parse).ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadStockRacks, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse PrintStockTag(string plantCode, decimal scheduleNum = 0, decimal rackId = 0, string workOrders = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", new TimeSpan(0)},
                    {"HighWaterMark", false},
                    {"ScheduleNum", scheduleNum},
                    {"RackId", rackId},
                    {"WorkOrders", workOrders?.Split(',').Where(x => decimal.TryParse(x, out _)).Select(decimal.Parse).ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.PrintStockTag, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public WorkOrder GetWorkOrder(decimal orderNum)
        {
            if (orderNum == 0)
            {
                throw new ApplicationException("Order Num is REQUIRED");
            }
            try
            {
                var processor = new MTOScheduleProcessor();
                return processor.GetWorkOrder(orderNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ScheduleRestService : IScheduleRestService
    {
        public MTOScheduleEvents GetUnprocessedMTOScheduleEvents(string plantCode = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetUnprocessedMTOScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.GetUnprocessedMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CancelMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.CancelMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse BoxedMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.BoxedMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.ResetMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.SuccessMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.CompleteMTOScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CancelMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CancelMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.SuccessMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CompleteMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse BackoutMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.BackoutMTOSchedule(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTOSchedule GetMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetMTOSchedule(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public MTOIncomingOrderLines GetMTOIncomingOrderLines(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.GetMTOIncomingOrderLines(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadMTOIncomingOrdersTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTOScheduleProcessor.DownloadMTOIncomingOrdersTable(plantCode, scheduleNum, scheduleDateE1, batchNum, force);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadScheduleInfoMTSTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.DownloadScheduleInfoMTSTable(plantCode, scheduleNum, scheduleDateE1, batchNum, force);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public RawGlassItems GetRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                if (scheduleNum == 0 && workOrderNum == 0)
                {
                    throw new ApplicationException("Schedule Number or Work Order Number is REQUIRED.");
                }
            }
            try
            {
                return RawGlassProcessor.GetRawGlassItems(plantCode, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0 && workOrderNum == 0)
            {
                throw new ApplicationException("Schedule Number or Work Order Number is REQUIRED.");
            }
            try
            {
                return RawGlassProcessor.DownloadRawGlassItems(plantCode, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadMTOSchedule(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "PLANT.CLEARPATH", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadMtoSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMTSSchedule(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "PLANT.CLEARPATH", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadMtsSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadScheduledOrders(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status = null, decimal scheduleNum = 0)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    { "TimeSpan", timeSpan},
                    { "HighWaterMark", useHighWaterMark},
                    { "Status", status},
                    { "ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "PLANT.CLEARPATH", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadScheduledOrders, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse BackoutSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("4 digit Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.BackoutSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse ResetSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum},
                    {"UpdateType", Schedule.StatusUpdateEvent.Reset}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.UpdateScheduleStatus, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse CancelSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum},
                    {"UpdateType", Schedule.StatusUpdateEvent.Cancel}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.UpdateScheduleStatus, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse CompleteSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum},
                    {"UpdateType", Schedule.StatusUpdateEvent.Complete}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.UpdateScheduleStatus, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse SuccessSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum},
                    {"UpdateType", Schedule.StatusUpdateEvent.Success}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.UpdateScheduleStatus, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadStockRacks(string plantCode, decimal scheduleNum = 0, decimal boxNum = 0, string workOrders = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", new TimeSpan(0)},
                    {"HighWaterMark", false},
                    {"ScheduleNum", scheduleNum},
                    {"BoxNum", boxNum},
                    {"WorkOrders", workOrders?.Split(',').Where(x => decimal.TryParse(x, out _)).Select(decimal.Parse).ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.DownloadStockRacks, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse PrintStockTag(string plantCode, decimal scheduleNum = 0, decimal rackId = 0, string workOrders = null)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", new TimeSpan(0)},
                    {"HighWaterMark", false},
                    {"ScheduleNum", scheduleNum},
                    {"RackId", rackId},
                    {"WorkOrders", workOrders?.Split(',').Where(x => decimal.TryParse(x, out _)).Select(decimal.Parse).ToList()}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.PrintStockTag, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public WorkOrder GetWorkOrder(string orderNum)
        {
            if (string.IsNullOrEmpty(orderNum))
            {
                throw new ApplicationException("Order Num is REQUIRED");
            }
            try
            {
                var processor = new MTOScheduleProcessor();
                return processor.GetWorkOrder(Convert.ToDecimal(orderNum));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

    }
}
