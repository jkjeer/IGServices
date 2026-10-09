using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Enterprise;
using SoaCore.Wcf;
using SqlBulkCopyResponse = IGServices.Common.Types.Response.SqlBulkCopyResponse;
using SqlBulkCopyResponses = IGServices.Common.Types.Response.SqlBulkCopyResponses;

namespace IGServices.PurchasedLotSvc
{
    [ServiceContract]
    public interface IPurchasedLotService
    {
        [OperationContract, OperationInvokerLogger]
        PurchasedLots GetPurchasedLots(string plantCode, int minutes);

        [OperationContract, OperationInvokerLogger]
        SqlBulkCopyResponses PersistAllEvents(string plantCode, int minutes);

        [OperationContract, OperationInvokerLogger]
        SqlBulkCopyResponse PersistPurchasedLotLoads(string plantCode, int minutes);

        [OperationContract, OperationInvokerLogger]
        SqlBulkCopyResponse PersistPurchasedLotReceipts(string plantCode, int minutes);
    }

    [ServiceContract]
    public interface IPurchasedLotRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPurchasedLots?PlantCode={plantCode}&Minutes={minutes}")]
        PurchasedLots GetPurchasedLots(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PersistPurchasedLots?PlantCode={plantCode}&Minutes={minutes}")]
        SqlBulkCopyResponses PersistPurchasedLots(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPurchasedLotLoads?PlantCode={plantCode}&Minutes={minutes}")]
        PurchasedLots GetPurchasedLotLoads(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PersistPurchasedLotLoads?PlantCode={plantCode}&Minutes={minutes}")]
        SqlBulkCopyResponse PersistPurchasedLotLoads(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPurchasedLotReceipts?PlantCode={plantCode}&Minutes={minutes}")]
        PurchasedLots GetPurchasedLotReceipts(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PersistPurchasedLotReceipts?PlantCode={plantCode}&Minutes={minutes}")]
        SqlBulkCopyResponse PersistPurchasedLotReceipts(string plantCode, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PersistAllEvents?PlantCode={plantCode}&Minutes={minutes}")]
        SqlBulkCopyResponses PersistAllEvents(string plantCode, string minutes);
    }
}
