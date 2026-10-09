using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Enterprise;
using SoaCore.Wcf;

namespace IGServices.RackTagSvc
{
    [ServiceContract]
    public interface IRackTagService
    {

        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagItems(decimal rackId, bool byUnitId = false);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagItemsDataTable(decimal rackId, string filter, bool byUnitId = false);

        [OperationContract, OperationInvokerLogger]
        RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, string filter, bool byUnitId = false);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagItemsByScheduleDataTable(string plantCode, decimal scheduleNum, string filter, bool byUnitId = false);

        [OperationContract, OperationInvokerLogger]
        DataTable GetRackTagItemsByLoadDataTable(string plantCode, decimal loadNum, string filter, bool byUnitId = false);

        [OperationContract, OperationInvokerLogger]
        List<Decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan);

        [OperationContract, OperationInvokerLogger]
        List<Decimal> GetRackIdListBySchedule(string plantCode, decimal scheduleNum);

        [OperationContract, OperationInvokerLogger]
        List<Decimal> GetRackIdListByLoad(string plantCode, decimal loadNum);

        [OperationContract, OperationInvokerLogger]
        RackStatusCodes GetRackStatusCodes();

        [OperationContract, OperationInvokerLogger]
        List<Decimal> GetLockedRacks(string plantCode, decimal minutesLatency);

    }

    [ServiceContract]
    public interface IRackTagRestService
    {

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItems?RackId={rackId}&ByUnitId={byUnitId}")]
        RackTagItems GetRackTagItems(string rackId, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsDataTable?RackId={rackId}&ByUnitId={byUnitId}")]
        DataTable GetRackTagItemsDataTable(string rackId, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsBySchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ByUnitId={byUnitId}")]
        RackTagItems GetRackTagItemsBySchedule(string plantCode, string scheduleNum, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsByScheduleDataTable?PlantCode={plantCode}&ScheduleNum={scheduleNum}&ByUnitId={byUnitId}")]
        DataTable GetRackTagItemsByScheduleDataTable(string plantCode, string scheduleNum, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackTagItemsByLoadDataTable?PlantCode={plantCode}&LoadNum={loadNum}&ByUnitId={byUnitId}")]
        DataTable GetRackTagItemsByLoadDataTable(string plantCode, decimal loadNum, bool byUnitId = true);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackIdListByStatus?PlantCode={plantCode}&StatusCode={statusCode}&Days={days}&Hours={hours}&Minutes={minutes}")]
        List<Decimal> GetRackIdListByStatus(string plantCode, string statusCode, string days, string hours, string minutes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackIdListBySchedule?PlantCode={plantCode}&ScheduleNum={scheduleNum}")]
        List<Decimal> GetRackIdListBySchedule(string plantCode, string scheduleNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackIdListByLoad?PlantCode={plantCode}&LoadNum={loadNum}")]
        List<Decimal> GetRackIdListByLoad(string plantCode, decimal loadNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetRackStatusCodes")]
        RackStatusCodes GetRackStatusCodes();

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetLockedRacks?PlantCode={plantCode}&Minutes={minutesLatency}")]
        List<Decimal> GetLockedRacks(string plantCode, string minutesLatency);

    }
}
