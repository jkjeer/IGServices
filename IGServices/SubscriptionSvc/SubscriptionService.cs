using System;
using System.Collections.Generic;
using System.Linq;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.SubscriptionSvc
{
    public class SubscriptionService : ISubscriptionService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Plants GetPlants(string code)
        {
            try
            {
                return SubscriptionProcessor.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public PlantGroups GetPlantGroups()
        {
            try
            {
                return SubscriptionProcessor.GetPlantGroups();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public SubscriptionTypes GetSubscriptionTypes(string subscriptionTypes)
        {
            try
            {
                return SubscriptionProcessor.GetSubscriptionTypes(subscriptionTypes);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


        public SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionProcessor.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunSubscriptionType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
                return BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse InitHighWaterMarks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markNames)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("MarkNames", markNames?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, SubscriptionProcessor.InitHighWaterMarks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class SubscriptionRestService : ISubscriptionRestService
    {
        public Plants GetPlants(string code)
        {
            try
            {
                return SubscriptionProcessor.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public PlantGroups GetPlantGroups()
        {
            try
            {
                return SubscriptionProcessor.GetPlantGroups();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public SubscriptionTypes GetSubscriptionTypes(string subscriptionTypes)
        {
            try
            {
                return SubscriptionProcessor.GetSubscriptionTypes(subscriptionTypes);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionProcessor.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse RunSubscriptionType(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var subscriptionTypes = SubscriptionManager.GetSubscriptionTypes(subscriptionType);
                return BusinessSystemTransferProcessor.Invoke(plants, subscriptionTypes, timeSpan, useHighWaterMark, parallel);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse InitHighWaterMarks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markNames)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("MarkNames", markNames?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, SubscriptionProcessor.InitHighWaterMarks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
