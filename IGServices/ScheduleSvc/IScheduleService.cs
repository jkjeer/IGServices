using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;

namespace IGServices.ScheduleSvc
{
    [ServiceContract]
    public interface IScheduleService
    {
        [OperationContract, OperationInvokerLogger]
        MTOScheduleEvents GetUnprocessedMTOScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CancelMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse BoxedMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse ResetMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse SuccessMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CompleteMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        MTOSchedule GetMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CancelMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse SuccessMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse CompleteMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse BackoutMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        MTOIncomingOrderLines GetMTOIncomingOrderLines(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMTOIncomingOrdersTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadScheduleInfoMTSTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false);

        [OperationContract, OperationInvokerLogger]
        RawGlassItems GetRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadScheduledOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string status = null, decimal scheduleNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadStockRacks(string plant, decimal scheduleNum = 0, decimal boxNum = 0, string workOrders = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse PrintStockTag(string plant, decimal scheduleNum = 0, decimal rackId = 0, string workOrders = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMTOSchedule(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string status = null, decimal scheduleNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMTSSchedule(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string status = null, decimal scheduleNum = 0);

        [OperationContract, OperationInvokerLogger]
        WorkOrder GetWorkOrder(decimal orderNum);
    }

    [ServiceContract]
    public interface IScheduleRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetUnprocessedMTOScheduleEvents?PlantCode={plantCode}")]
        MTOScheduleEvents GetUnprocessedMTOScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetUnprocessedMTSScheduleEvents?PlantCode={plantCode}")]
        MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CancelMTOScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CancelMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/BoxedMTOScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse BoxedMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ResetMTOScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse ResetMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SuccessMTOScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse SuccessMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CompleteMTOScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CompleteMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CancelMTSScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CancelMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ResetMTSScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SuccessMTSScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse SuccessMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CompleteMTSScheduleEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse CompleteMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/BackoutMTOSchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        ProcessResponse BackoutMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetMTOSchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        MTOSchedule GetMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetMTOIncomingOrderLines?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}")]
        MTOIncomingOrderLines GetMTOIncomingOrderLines(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMTOIncomingOrdersTable?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}&Force={force}")]
        ProcessResponse DownloadMTOIncomingOrdersTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadScheduleInfoMTSTable?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}&Force={force}")]
        ProcessResponse DownloadScheduleInfoMTSTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, bool force = false);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRawGlassItems?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}&WorkOrderNum={workOrderNum}")]
        RawGlassItems GetRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRawGlassItems?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ScheduleDateE1={scheduleDateE1}&BatchNum={batchNum}&WorkOrderNum={workOrderNum}")]
        ProcessResponse DownloadRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadScheduledOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}&ScheduleNum={scheduleNum}")]
        ProcessResponse DownloadScheduledOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadStockRacks?Plant={plant}&ScheduleNum={scheduleNum}&BoxNum={boxNum}&WorkOrder={workOrders}")]
        ProcessResponse DownloadStockRacks(string plant, decimal scheduleNum = 0, decimal boxNum = 0, string workOrders = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PrintStockTag?Plant={plant}&ScheduleNum={scheduleNum}&RackId={rackId}&WorkOrder={workOrders}")]
        ProcessResponse PrintStockTag(string plant, decimal scheduleNum = 0, decimal rackId = 0, string workOrders = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMTOSchedule?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}&ScheduleNum={scheduleNum}")]
        ProcessResponse DownloadMTOSchedule(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMTSSchedule?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}&ScheduleNum={scheduleNum}")]
        ProcessResponse DownloadMTSSchedule(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/BackoutSchedule?Plant={plantCode}&ScheduleNum={scheduleNum}")]
        ProcessResponse BackoutSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ResetSchedule?Plant={plantCode}&ScheduleNum={scheduleNum}")]
        ProcessResponse ResetSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CancelSchedule?Plant={plantCode}&ScheduleNum={scheduleNum}")]
        ProcessResponse CancelSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/CompleteSchedule?Plant={plantCode}&ScheduleNum={scheduleNum}")]
        ProcessResponse CompleteSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SuccessSchedule?Plant={plantCode}&ScheduleNum={scheduleNum}")]
        ProcessResponse SuccessSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetWorkOrder?OrderNum={orderNum}")]
        WorkOrder GetWorkOrder(string orderNum);

    }
}
