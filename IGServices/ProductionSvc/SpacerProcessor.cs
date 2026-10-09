using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.ProductionSvc
{
    public class SpacerProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse UploadSpacerConsumption(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Spacer Upload Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                response.ProcessResponses.Add(SpacerConsumptionUploadManager.Process(session, sqlCns, plant, statusCode));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static SpacerConsumption GetSpacerConsumption(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var cnSFInt = new SqlConnection();
            SpacerConsumption spacerConsumption;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                cnSFInt = GetSFIntSqlConnection(plant.PlantNum);
                spacerConsumption = new SpacerConsumption();
                //boxingSchedule = BoxingScheduleManager.Process(e1Session, cnSF, plant, scheduleNum);
                //var spacerConsumptionMgr = new SpacerConsumptionUploadManager();
                //response.SpacerConsumptions = spacerConsumptionMgr.GetSpacerConsumption(cnSFInt, plantNum);
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                cnSFInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumption;
        }

        private static SqlConnection GetCorpIntSqlConnection(string plantNum)
        {
            var cnCorpInt = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION", false);
                    cnCorpInt.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($@"{cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnCorpInt;
        }

        private static SqlConnection GetSFIntSqlConnection(string plantNum)
        {
            var cnSFInt = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnSFInt = ConnectionManager.GetSqlConnectionFromKey($@"{plantNum}.INTEGRATION", false);
                    cnSFInt.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($@"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSFInt;
        }
    }
}
