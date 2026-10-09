using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.SubscriptionSvc
{
    public class SubscriptionProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * METHODS
        ********************************************************************************/
        public static Plants GetPlants(string code)
        {
            try
            {
                return PlantManager.GetPlants(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static PlantGroups GetPlantGroups()
        {
            try
            {
                return PlantManager.GetPlantGroupCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static SubscriptionTypes GetSubscriptionTypes(string code)
        {
            try
            {
                return SubscriptionManager.GetSubscriptionTypes(code);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            try
            {
                return SubscriptionManager.RefreshSubscriptionTypeCache();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ProcessResponse InitHighWaterMarks(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Init High Water Marks");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var markNames = Data.UnboxParm.To(parms, "MarkNames", new List<string>());
                //var mgr = new HighWaterMarkInitManager(session);
                //response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, markNames);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }
    }
}
