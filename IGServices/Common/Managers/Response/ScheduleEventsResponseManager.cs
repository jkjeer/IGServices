using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Managers.Response
{
    public class ScheduleEventsResponseManager
    {
        public void Init(MTOScheduleEventsResponse response, string plantCode)
        {
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.OpenResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            var plantAttribute = new PlantAttribute(plantCode);
            response.PlantNum = plantAttribute.NumericCode;
            response.PlantCode = plantAttribute.Code;
        }
    }
}
