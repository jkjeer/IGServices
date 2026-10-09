using System;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.ScheduleSvc
{
    public class RawGlassProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static RawGlassItems GetRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            RawGlassItems rawGlassItems;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSFInt = GetSFIntSqlConnection(plant.PlantNum);
                rawGlassItems = RawGlassManager.GetRawGlassItems(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                cnSFInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rawGlassItems;
        }

        public static ProcessResponse DownloadRawGlassItems(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Download Raw Glass Table", true);
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSFInt = GetSFIntSqlConnection(plant.PlantCode);
                response.RowsAffected = RawGlassDownloadManager.Process(e1Session, cnSFInt, plant, scheduleNum, scheduleDateE1, batchNum, workOrderNum);                
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                cnSFInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
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
                    Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{cnSFInt.ConnectionString }  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSFInt;
        }
    }
}
