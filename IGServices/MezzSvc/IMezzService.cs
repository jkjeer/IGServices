using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using SoaCore.Wcf;

namespace IGServices.MezzSvc
{

    [ServiceContract]
    public interface IMezzService
    {
        [OperationContract, OperationInvokerLogger]
        WorkOrder GetWorkOrder(decimal orderNum);

        [OperationContract, OperationInvokerLogger]
        WorkOrders GetCancelledOrdersBySchedule(string plantCode, Decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        SpacerItems GetSpacerItems(string plantCode);

        [OperationContract, OperationInvokerLogger]
        GlassItem GetDiscreteItem(string plantCode, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        GlassItems GetDiscreteItems(string plantCode);

        [OperationContract, OperationInvokerLogger]
        SqlBulkCopyResponse PersistSpacerItems(string plantCode);
    }

    [ServiceContract]
    public interface IMezzRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetWorkOrder?OrderNum={orderNum}")]
        [Description("GET Work Order")]
        WorkOrder GetWorkOrder(string orderNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetCancelledOrdersBySchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}")]
        [Description("GET Cancelled Work Orders (Status 98) for MTOSchedule")]
        WorkOrders GetCancelledOrdersBySchedule(string plantCode, string scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetSpacerItems?PlantCode={plantCode}")]
        [Description("GET All Spacer Items (CHAN) for Plant")]
        SpacerItems GetSpacerItems(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetDiscreteItem?PlantCode={plantCode}&ItemNum={itemNum}")]
        GlassItem GetDiscreteItem(string plantCode, string itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetDiscreteItems?PlantCode={plantCode}")]
        GlassItems GetDiscreteItems(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/PersistSpacerItems?PlantCode={plantCode}")]
        SqlBulkCopyResponse PersistSpacerItems(string plantCode);
    }

}
