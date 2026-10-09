using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;
using SqlBulkCopyResponse = IGServices.Common.Types.Response.SqlBulkCopyResponse;

namespace IGServices.RackSvc
{
    [ServiceContract]
    public interface IRackService
    {

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse DownloadRackEvents(string plants, TimeSpan span, bool useHighWaterMark, decimal rackId);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse DownloadRackItemEvents(string plantCode, decimal ukid);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackItems(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadCurrentTags(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string serialNum = null);

        //[OperationContract, OperationInvokerLogger]
        //ProcessResponse UploadCrateTags(string plant, bool parallel = false, decimal scheduleNum = 0, decimal workOrderNum = 0, string serialNum = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadShippableRacks(string plant);
    }

    [ServiceContract]
    public interface IRackRestService
    {

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/DownloadRackEvents?Plant={plants}&LookbackSpan={span}&UseHighWaterMark={useHighWaterMark}&RackId={rackId}")]
        //ProcessResponse DownloadRackEvents(string plants, TimeSpan span, bool useHighWaterMark, decimal rackId);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/DownloadRackItemEvents?PlantCode={plantCode}&Id={ukid}")]
        //SqlBulkCopyResponse DownloadRackItemEvents(string plantCode, decimal ukid);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRacks?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRacks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackItems?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackItems(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRackDetails?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&StatusCodes={statusCodes}")]
        ProcessResponse DownloadRackDetails(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRawTags?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}")]
        ProcessResponse DownloadRawTags(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadCurrentTags?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&RackId={rackId}&SerialNum={serialNum}")]
        ProcessResponse DownloadCurrentTags(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal rackId = 0, string serialNum = null);

        //[OperationContract, OperationInvokerLogger]
        //[WebGet(UriTemplate = "/UploadCrateTags?Plant={plant}&Parallel={parallel}&ScheduleNum={scheduleNum}&WorkOrderNum={workOrderNum}&SerialNum={serialNum}")]
        //ProcessResponse UploadCrateTags(string plant, bool parallel = false, decimal scheduleNum = 0, decimal workOrderNum = 0, string serialNum = null);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadShippableRacks?Plant={plant}")]
        ProcessResponse DownloadShippableRacks(string plant);
    }

}


