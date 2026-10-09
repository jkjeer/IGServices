using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.ShopFloor;
using NLog;

namespace IGServices.Common.Utilities
{
    public class BatchNumManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /******************************************************************
        * For Boxing Schedule Uploads
        ******************************************************************/
        public static string GetNextBIEDBT(SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var sBatchNum = string.Empty;
            try
            {
                var cmd = new SqlCommand()
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[BIEDBT] AS 'BIEDBT'",
                    Connection = cnCorpInt
                };
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    var batchNum = CastDbNull.To<long>(reader["BIEDBT"], 0);
                    sBatchNum = Convert.ToString(batchNum);
                    rowCount++;
                }
                reader.Close();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return sBatchNum;
        }

        public static void GetBIUKIDs(BoxingSchedule boxingSchedule, SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var numberOfBIUKIDS = boxingSchedule.BoxItems.Count;
                var firstBIUKID = GetFirstBIUKIDRange(cnCorpInt, numberOfBIUKIDS);
                foreach (var boxItem in boxingSchedule.BoxItems)
                {
                    boxItem.UniqueID = firstBIUKID;
                    firstBIUKID++;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static decimal GetFirstBIUKIDRange(SqlConnection cnCorpInt, int numberOfBIUKIDS)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            decimal dFirstBIUKID;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnCorpInt,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sys.sp_sequence_get_range"
                };
                cmd.Parameters.AddWithValue("@sequence_name", "E1.BIUKID");
                cmd.Parameters.AddWithValue("@range_size", numberOfBIUKIDS);
                var firstValueInRange = new SqlParameter("@range_first_value", SqlDbType.Variant)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(firstValueInRange);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
                var firstBIUKID = CastDbNull.To<long>(firstValueInRange.Value, 0);
                dFirstBIUKID = Convert.ToDecimal(firstBIUKID);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return dFirstBIUKID;
        }

        public decimal GetNextBIUKID(SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            decimal dUniqueID = 0;
            try
            {
                var cmd = new SqlCommand()
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[BIUKID] AS 'BIUKID'",
                    Connection = cnCorpInt
                };
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    var uniqueID = CastDbNull.To<long>(reader["BIUKID"], 0);
                    dUniqueID = Convert.ToDecimal(uniqueID);
                    rowCount++;
                }
                reader.Close();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return dUniqueID;
        }

        public static void GetBoxingBatch(BoxingSchedule boxingSchedule, SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                boxingSchedule.TransferBatchNum = GetNextBIEDBT(cnCorpInt);
                boxingSchedule.BatchDate = DateTime.Now.ToLocalTime();
                foreach (var boxItem in boxingSchedule.BoxItems)
                {
                    boxItem.TransferBatchNum = boxingSchedule.TransferBatchNum;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static decimal GetNextQAUKID(SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long uniqueId = 0;
            decimal dUniqueId;
            try
            {
                var cmd = new SqlCommand()
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[QAUKID] AS 'QAUKID'",
                    Connection = cnCorpInt
                };
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    uniqueId = CastDbNull.To<long>(reader["QAUKID"], 0);
                    rowCount++;
                }
                reader.Close();
                Logger.Trace($"ROWS: {rowCount}");
                dUniqueId = Convert.ToDecimal(uniqueId);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return dUniqueId;
        }

        /******************************************************************
        * For Spacer Conusmption Uploads
        ******************************************************************/
        public static long GetNextSFEDBT(SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long batchNum = 0;
            try
            {
                var cmd = new SqlCommand()
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[SFEDBT] AS 'SFEDBT'",
                    Connection = cnCorpInt
                };
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    batchNum = CastDbNull.To<long>(reader["SFEDBT"], 0);
                    rowCount ++;
                }
                reader.Close();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return batchNum;
        }

        public static void GetSFUKIDs(SpacerConsumptions spacerConsumptions, SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var numberOfSFUKIDS = spacerConsumptions.Count(spacerConsumption => spacerConsumption.IsValid);
                var firstSFUKID = GetFirstSFUKIDRange(cnCorpInt, numberOfSFUKIDS);
                foreach (var spacerConsumption in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                {
                    spacerConsumption.TransferUniqueID = firstSFUKID;
                    firstSFUKID++;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static long GetFirstSFUKIDRange(SqlConnection cnCorpInt, int numberOfSFUKIDS)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long firstSFUKID;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnCorpInt,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sys.sp_sequence_get_range"
                };
                cmd.Parameters.AddWithValue("@sequence_name", "E1.SFUKID");
                cmd.Parameters.AddWithValue("@range_size", numberOfSFUKIDS);
                var firstValueInRange = new SqlParameter("@range_first_value", SqlDbType.Variant)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(firstValueInRange);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                firstSFUKID = CastDbNull.To<long>(firstValueInRange.Value, 0);
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return firstSFUKID;
        }
    }
}
