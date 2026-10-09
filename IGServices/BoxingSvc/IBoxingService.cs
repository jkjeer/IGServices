using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;

namespace IGServices.BoxingSvc
{
    [ServiceContract]
    public interface IBoxingService
    {
        [OperationContract, OperationInvokerLogger]
        BoxingEvent GetBoxingEvent(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        BoxingEvents GetUnprocessedBoxingEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        BoxingSchedule GetBoxingSchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse UploadBoxingSchedule(string plantCode, decimal scheduleNum, bool force);
    }

    [ServiceContract]
    public interface IBoxingRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetBoxingEvent?PlantCode={plantCode}&ScheduleNum={scheduleNum}")]
        BoxingEvent GetBoxingEvent(string plantCode, string scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetUnprocessedBoxingEvents?PlantCode={plantCode}")]
        BoxingEvents GetUnprocessedBoxingEvents(string plantCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetBoxingSchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}")]
        BoxingSchedule GetBoxingSchedule(string plantCode, string scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/UploadBoxingSchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}&Force={force}")]
        ProcessResponse UploadBoxingSchedule(string plantCode, string scheduleNum, bool force = false);
    }
}
