using System;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class RawGlassDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static int Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var rowCount = 0;
            try
            {
                var rawGlassItems = RawGlassManager.GetRawGlassItems(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum, workOrderNum);
                var rawGlassTable = new RawGlassEbsTable();
                rawGlassTable.Add(rawGlassItems);
                if (rawGlassTable.Rows.Count > 0)
                {
                    var bulkResponse = BulkCopyManager.BulkTable(rawGlassTable.ExtendedProperties["Target"].ToString(), rawGlassTable, cnSFInt);
                    rowCount = (int) bulkResponse.RowsCopied;
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSFInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }
    }
}
