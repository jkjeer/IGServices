using System;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;

namespace IGServices.BoxingSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]

    public class BoxingService : IBoxingService
    {
        public BoxingEvent GetBoxingEvent(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED");
            }
            try
            {
                return BoxingProcessor.GetBoxingEvent(plantCode, Convert.ToDecimal(scheduleNum));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public BoxingEvents GetUnprocessedBoxingEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            try
            {
                return BoxingProcessor.GetUnprocessedBoxingEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public BoxingSchedule GetBoxingSchedule(string plantCode, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return BoxingProcessor.GetBoxingSchedule(plantCode, scheduleNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse UploadBoxingSchedule(string plantCode, decimal scheduleNum, bool force)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return BoxingProcessor.UploadBoxingSchedule(plantCode, scheduleNum, force);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class BoxingRestService : IBoxingRestService
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public BoxingEvent GetBoxingEvent(string plantCode, string scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            if (string.IsNullOrEmpty(scheduleNum))
            {
                throw new ApplicationException("Schedule Number is REQUIRED");
            }
            try
            {
                return BoxingProcessor.GetBoxingEvent(plantCode, Convert.ToDecimal(scheduleNum));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public BoxingEvents GetUnprocessedBoxingEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            try
            {
                return BoxingProcessor.GetUnprocessedBoxingEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public BoxingSchedule GetBoxingSchedule(string plantCode, string scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            if (string.IsNullOrEmpty(scheduleNum))
            {
                throw new ApplicationException("Schedule Number is REQUIRED");
            }
            try
            {
                return BoxingProcessor.GetBoxingSchedule(plantCode, Convert.ToDecimal(scheduleNum));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse UploadBoxingSchedule(string plantCode, string scheduleNum, bool force)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            if (string.IsNullOrEmpty(scheduleNum))
            {
                throw new ApplicationException("Schedule Number is REQUIRED");
            }
            try
            {
                return BoxingProcessor.UploadBoxingSchedule(plantCode, Convert.ToDecimal(scheduleNum), Convert.ToBoolean(force));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }
    }
}
