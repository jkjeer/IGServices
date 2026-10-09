using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;
using SqlDmlResponse = IGServices.Common.Types.Response.SqlDmlResponse;

namespace IGServices.WOStatusSvc
{
    [ServiceContract]
    public interface IWorkOrderStatusService
    {
        [OperationContract, OperationInvokerLogger]
        PendingUpdatesResults GetPlantsWithPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        SqlDmlResponse SetPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        WorkOrderStatuses GetPendingUpdatesByPlant(string plantCode);

        [OperationContract, OperationInvokerLogger]
        PlantOperationResponses ProcessAll();
    }

    [ServiceContract]
    public interface IWorkOrderStatusRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetPlantsWithPendingUpdates")]
        PendingUpdatesResults GetPlantsWithPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/SetPendingUpdates")]
        SqlDmlResponse SetPendingUpdates();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/ProcessAll")]
        PlantOperationResponses ProcessAll();
    }
}
