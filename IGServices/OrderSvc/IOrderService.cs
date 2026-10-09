using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Response;
using SoaCore.Wcf;


namespace IGServices.OrderSvc
{
    [ServiceContract]
    public interface IOrderService
    {

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSalesLines(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0, string nextStatusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0, string statusCodes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadWorkOrderStatus(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal workOrderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadRelatedOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPurchasedLots(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPurchaseOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string documentTypes = null, decimal documentNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadPartList(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal orderNum = 0);
    }

    [ServiceContract]
    public interface IOrderRestService
    {

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSalesLines?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SalesOrderNum={workOrderNum}&NextStatusCode={nextStatusCodes}")]
        ProcessResponse DownloadSalesLines(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum, string nextStatusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&WorkOrderNum={workOrderNum}&StatusCode={statusCodes}")]
        ProcessResponse DownloadWorkOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum, string statusCodes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadWorkOrderStatus?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&WorkOrderNum={workOrderNum}")]
        ProcessResponse DownloadWorkOrderStatus(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadRelatedOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadRelatedOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadPurchasedLots?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadPurchasedLots(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadPurchaseOrders?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&OrderNum={orderNum}")]
        ProcessResponse DownloadPurchaseOrders(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtoSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&DocumentTypes={documentTypes}&DocumentNum={documentNum}")]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes, decimal documentNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtsSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadPartList?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&OrderNum={orderNum}")]
        ProcessResponse DownloadPartList(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum);
    }
}
