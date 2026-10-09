using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using NLog;

namespace IGServices.RackTagSvc
{
    public class RackTagProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RackStatusCodes GetRackStatusCodes()
        {
            var e1Session = GetE1Session();
            var rackStatusCodes = new RackStatusCodes();
            try
            {
                var manager = new RackStatusCodeManager(e1Session);
                rackStatusCodes = manager.GetRackStatusCodes(e1Session);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return rackStatusCodes;
        }


        public List<decimal> GetRackIdListByStatus(string plantCode, string statusCode, TimeSpan timeSpan)
        {
            var e1Session = GetE1Session();
            var rackIds = new List<decimal>();
            try
            {
                var manager = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                rackIds = manager.GetRackIdListByStatus(e1Session, plant, statusCode, timeSpan);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return rackIds;
        }

        public List<decimal> GetRackIdListBySchedule(string plantCode, decimal ScheduleNum)
        {
            var e1Session = GetE1Session();
            var rackIds = new List<decimal>();
            try
            {
                var manager = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                rackIds = manager.GetRackIdListBySchedule(e1Session, plant, ScheduleNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return rackIds;
        }

        public Racks GetRackListBySchedule(string plantCode, decimal ScheduleNum)
        {
            var e1Session = GetE1Session();
            var racks = new Racks();
            try
            {
                var manager = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                racks = manager.GetRackListBySchedule(e1Session, plant, ScheduleNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return racks;
        }

        public List<decimal> GetRackIdListByLoad(string plantCode, decimal loadNum)
        {
            var e1Session = GetE1Session();
            var rackIds = new List<decimal>();
            try
            {
                var manager = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                rackIds = manager.GetRackIdListByLoad(e1Session, plant, loadNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return rackIds;
        }

        public RackTagItems GetRackTagItems(decimal rackId, bool byUnitId)
        {
            var e1Session = GetE1Session();
            var items = new RackTagItems();
            try
            {
                var itemMgr = new RackTagItemManager(e1Session);
                items = itemMgr.GetRackTagItems(e1Session, rackId, byUnitId);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return items;
        }

        public RackTagItems GetRackTagItemsBySchedule(string plantCode, decimal scheduleNum, bool byUnitId)
        {
            var e1Session = GetE1Session();
            var items = new RackTagItems();
            try
            {
                var itemMgr = new RackTagItemManager(e1Session);
                var rackIds = GetRackIdListBySchedule(plantCode, scheduleNum);
                foreach (decimal rackId in rackIds)
                {
                    var rackItems = itemMgr.GetRackTagItems(e1Session, rackId, byUnitId);
                    items.AddRange(rackItems);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return items;
        }

        public RackTagItems GetRackTagItemsByLoad(string plantCode, decimal loadNum, bool byUnitId)
        {
            var e1Session = GetE1Session();
            var items = new RackTagItems();
            try
            {
                var itemMgr = new RackTagItemManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                var rackIds = GetRackIdListByLoad(plantCode, loadNum);
                foreach (decimal rackId in rackIds)
                {
                    var rackItems = itemMgr.GetRackTagItems(e1Session, rackId, byUnitId);
                    items.AddRange(rackItems);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return items;
        }

        public RackTagItems FilterRackTags(RackTagItems rackTags, string filter)
        {
            var results = rackTags.AsQueryable().Where(filter);
            var returnTags = new RackTagItems();
            foreach (var result in results)
            {
                returnTags.Add(result);
            }
            return returnTags;
        }

        public List<decimal> GetLockedRacks(string plantCode, decimal minutesLatency)
        {
            var e1Session = GetE1Session();
            var rackIds = new List<decimal>();
            try
            {
                var mgr = new RackIdListManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                rackIds = mgr.GetLockedRacks(e1Session, plant, minutesLatency);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return rackIds;
        }

        private static BusinessSystemSession GetE1Session()
        {
            var connectionString = new ConnectionString();
            var e1Session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["RackTagEnvironment"]);
            return e1Session;
        }

        private static void CloseE1Session(BusinessSystemSession e1Session)
        {
            var e1SessionMgr = new BusinessSystemSessionManager();
            e1SessionMgr.CloseBusinessSession(e1Session);
        }
    }
}
