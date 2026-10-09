using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using SoaCore.Wcf;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;

namespace IGServices.ProductionSvc
{
    [ServiceContract]
    public interface IProductionService
    {
        [OperationContract, OperationInvokerLogger]
        SpacerConsumption GetSpacerConsumption(string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse GetQCInspections(string plantCode);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadSpacerConsumption(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadMachinePartConsumption(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse GlassInventoryTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadRemakes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UnitBusinessTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse GlassBusinessTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);
    }

    [ServiceContract]
    public interface IProductionRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetSpacerConsumption?PlantCode={plantCode}")]
        SpacerConsumption GetSpacerConsumption(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetQCInspections?PlantCode={plantCode}")]
        ProcessResponse GetQCInspections(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadSpacerConsumption?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse UploadSpacerConsumption(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadMachinePartConsumption?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse UploadMachinePartConsumption(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GlassInventoryTransactionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse GlassInventoryTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/HvReceiptUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse HvReceiptUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadRemakes?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse UploadRemakes(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UnitBusinessTransactionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse UnitBusinessTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GlassBusinessTransactionUpload?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&Status={status}")]
        ProcessResponse GlassBusinessTransactionUpload(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status);
    }
}
