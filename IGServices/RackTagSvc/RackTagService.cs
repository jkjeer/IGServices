using System;
using System.Collections.Generic;
using System.Data;
using IGServices.Common.Types.Enterprise;
using SoaCore.Wcf;

namespace IGServices.RackTagSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackTagService : IRackTagService
    {

        public RackTagItems GetRackTagItems(decimal rackId, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItems(rackId, byUnitId);
        }

        public DataTable GetRackTagItemsDataTable(decimal rackId, string filter = null, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItems(rackId, byUnitId).GetDataTable();
            }
            var rackTagItems = processor.GetRackTagItems(rackId, byUnitId);
            return processor.FilterRackTags(rackTagItems, filter).GetDataTable();
        }

        //public DataTable GetRackTagItemsDataTable(decimal rackId, string filter)
        //{
        //    var processor = new RackTagProcessor();
        //    return processor.GetRackTagItems(rackId).GetDataTable();
        //}

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, string filter, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
            }
            var items = processor.GetRackTagItemsBySchedule(plantCode, scheduleNum, byUnitId);
            return processor.FilterRackTags(items, filter);
        }

        public DataTable GetRackTagItemsByScheduleDataTable(string plantCode, decimal scheduleNum, string filter, bool byUnitId = false)
        {
            return GetRackTagItemsBySchedule(plantCode, scheduleNum, filter, byUnitId).GetDataTable();
        }

        public RackTagItems GetRackTagItemsByLoad(string plantCode, decimal loadNum, string filter, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            if (String.IsNullOrEmpty(filter))
            {
                return processor.GetRackTagItemsByLoad(plantCode, loadNum, byUnitId);
            }
            var items = processor.GetRackTagItemsByLoad(plantCode, loadNum, byUnitId);
            return processor.FilterRackTags(items, filter);
        }

        public DataTable GetRackTagItemsByLoadDataTable(string plantCode, decimal loadNum, string filter, bool byUnitId = false)
        {
            return GetRackTagItemsByLoad(plantCode, loadNum, filter, byUnitId).GetDataTable();
        }

        public List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListByStatus(plantCode, statusCode, timeSpan);
        }

        public List<Decimal> GetRackIdListBySchedule(string plantCode, decimal scheduleNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListBySchedule(plantCode, scheduleNum);
        }

        public List<Decimal> GetRackIdListByLoad(string plantCode, decimal loadNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListByLoad(plantCode, loadNum);
        }

        public RackStatusCodes GetRackStatusCodes()
        {
            var processor = new RackTagProcessor();
            return processor.GetRackStatusCodes();
        }

        public List<Decimal> GetLockedRacks(string plantCode, decimal minutesLatency)
        {
            var processor = new RackTagProcessor();
            return processor.GetLockedRacks(plantCode, minutesLatency);
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class RackTagRestService : IRackTagRestService
    {

        public RackTagItems GetRackTagItems(string rackId, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId);
        }

        public DataTable GetRackTagItemsDataTable(string rackId, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItems(Convert.ToDecimal(rackId), byUnitId).GetDataTable();
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, string scheduleNum, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, Convert.ToDecimal(scheduleNum), byUnitId);
        }

        public DataTable GetRackTagItemsByScheduleDataTable(string plantCode, string scheduleNum, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsBySchedule(plantCode, Convert.ToDecimal(scheduleNum), byUnitId).GetDataTable();
        }

        public RackTagItems GetRackTagItemsByLoad(string plantCode, decimal loadNum, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsByLoad(plantCode, loadNum, byUnitId);
        }

        public DataTable GetRackTagItemsByLoadDataTable(string plantCode, decimal loadNum, bool byUnitId = false)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackTagItemsByLoad(plantCode, loadNum, byUnitId).GetDataTable();
        }

        public List<Decimal> GetRackIdListByStatus(string plantCode, string statusCode, string days, string hours, string minutes)
        {
            var processor = new RackTagProcessor();
            TimeSpan ts = new TimeSpan(Convert.ToInt16(days), Convert.ToInt16(hours), Convert.ToInt16(minutes), 0);
            return processor.GetRackIdListByStatus(plantCode, statusCode, ts);
        }

        public List<Decimal> GetRackIdListBySchedule(string plantCode, string scheduleNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListBySchedule(plantCode, Convert.ToDecimal(scheduleNum));
        }

        public List<Decimal> GetRackIdListByLoad(string plantCode, decimal loadNum)
        {
            var processor = new RackTagProcessor();
            return processor.GetRackIdListByLoad(plantCode, loadNum);
        }

        public RackStatusCodes GetRackStatusCodes()
        {
            var processor = new RackTagProcessor();
            return processor.GetRackStatusCodes();
        }

        public List<Decimal> GetLockedRacks(string plantCode, string minutesLatency)
        {
            var processor = new RackTagProcessor();
            return processor.GetLockedRacks(plantCode, Convert.ToDecimal(minutesLatency));
        }

    }

}
