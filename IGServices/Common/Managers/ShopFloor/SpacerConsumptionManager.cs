using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class SpacerConsumptionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static SqlCommand GetSpacerConsumptionAggregationsGuid()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  [ConsumptionAggregateId]
                            ,sca.[SourceRows]
                            ,ISNULL(si.[SpacerItemId], 0) AS 'SpacerItemId'
                            ,CONVERT(VARCHAR, sca.[ItemNum]) AS 'ItemNum'
                            ,sca.[Quantity]
                            ,sca.[ProductionDate]
                            ,sca.[MinWrittenDate]
                            ,sca.[MaxWrittenDate]
                            ,'N' AS 'TransferStatus'
                            ,sca.[TransferDate]
                            ,sca.[TransferMessage]
                            ,sca.[TransferBatch] AS 'TransferBatch'
                            ,sca.[TransferUniqueID]
                    FROM    [dbo].[SpacerConsumedAggregate] sca
                            LEFT OUTER JOIN [SHOPFLOOR].[EBS].[Item] si ON si.[ItemNum] = sca.[ItemNum]
                    WHERE   sca.[TransferStatus] IN ( '', 'N' )
                            AND sca.[ItemNum] > 0
                            AND sca.[TransferGUID] = @TransferGuid
                ";
                cmd.Parameters.Add(new SqlParameter("@TransferGuid", SqlDbType.UniqueIdentifier));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public static SqlCommand GetShopFloorSpacerConsumptionAggregationsGuid()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  [ConsumptionAggregateId]
                            ,sca.[SourceRows]
                            ,ISNULL(si.[SpacerItemId], 0) AS 'SpacerItemId'
                            ,CONVERT(VARCHAR, sca.[ItemNum]) AS 'ItemNum'
                            ,sca.[Quantity]
                            ,sca.[ProductionDate]
                            ,sca.[MinWrittenDate]
                            ,sca.[MaxWrittenDate]
                            ,'N' AS 'TransferStatus'
                            ,sca.[TransferDate]
                            ,sca.[TransferMessage]
                            ,sca.[TransferBatch] AS 'TransferBatch'
                            ,sca.[TransferUniqueID]
                            ,sca.[UnitOfMeasure]
                    FROM    [EBS].[SpacerConsumedAggregate] sca
                            LEFT OUTER JOIN [EBS].[Item] si ON si.[ItemNum] = sca.[ItemNum]
                    WHERE   sca.[TransferStatus] IN ( '', 'N' )
                            AND sca.[ItemNum] > 0
                            AND sca.[TransferGUID] = @TransferGuid";
                cmd.Parameters.Add(new SqlParameter("@TransferGuid", SqlDbType.UniqueIdentifier));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public static SqlCommand GetSpacerConsumptionAggregations()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  [ConsumptionAggregateId]
                            ,sca.[SourceRows]
                            ,ISNULL(si.[ItemId], 0) AS 'SpacerItemId'
                            ,CONVERT(VARCHAR, sca.[ItemNum]) AS 'ItemNum'
                            ,sca.[Quantity]
                            ,sca.[ProductionDate]
                            ,sca.[MinWrittenDate]
                            ,sca.[MaxWrittenDate]
                            ,'N' AS 'TransferStatus'
                            ,sca.[TransferDate]
                            ,sca.[TransferMessage]
                            ,sca.[TransferBatch] AS 'TransferBatch'
                            ,sca.[TransferUniqueID]
                    FROM    [dbo].[SpacerConsumedAggregate] sca
                            LEFT OUTER JOIN [ShopFloor].[EBS].[Item] si ON si.[ItemNum] = sca.[ItemNum]
                    WHERE   sca.[TransferStatus] IN ( '', 'N' )
                            AND sca.[ItemNum] > 0
                ";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public static SqlCommand GetShopFloorSpacerConsumptionAggregations()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  sca.[ConsumptionAggregateId]
                            ,sca.[SourceRows]
                            ,ISNULL(si.[SpacerItemId], 0) AS 'SpacerItemId'
                            ,CONVERT(VARCHAR, sca.[ItemNum]) AS 'ItemNum'
                            ,sca.[Quantity]
                            ,sca.[ProductionDate]
                            ,sca.[MinWrittenDate]
                            ,sca.[MaxWrittenDate]
                            ,'N' AS 'TransferStatus'
                            ,sca.[TransferDate]
                            ,sca.[TransferMessage]
                            ,sca.[TransferBatch] 
                            ,sca.[TransferUniqueID]
                            ,sca.[UnitOfMeasure]
                    FROM    [EBS].[SpacerConsumedAggregate] sca
                    LEFT OUTER JOIN [EBS].[Item] si 
                            ON si.[ItemNum] = sca.[ItemNum]
                    WHERE   sca.[TransferStatus] IN ( '', 'N' )
                            AND sca.[ItemNum] > 0
                    ";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public static SqlCommand GetSpacerConsumption()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  COUNT(sa.[SpacerLengthsCompleteId]) AS 'SourceRows'
                            ,ISNULL(si.[SpacerItemId], 0) AS 'SpacerItemId'
		                    ,CONVERT(VARCHAR, sa.[ItemNum]) AS 'ItemNum'
		                    ,SUM(sa.[Quantity]) AS 'Quantity'
		                    ,CONVERT(DATE, (sa.[WrittenDate])) AS 'ProductionDate'
		                    ,MIN(sa.[WrittenDate]) AS 'MinWrittenDate'
		                    ,MAX(sa.[WrittenDate]) AS 'MaxWrittenDate'
		                    ,'N' AS 'TransferStatus'
                            ,sa.[TransferDate]
                            ,sa.[TransferMessage]
                            ,sa.[TransferBatchNum] as 'TransferBatch'
                            ,sa.[TransferUniqueID]
                    FROM    [INTEGRATION].[dbo].[SpacerConsumed] sa
                            LEFT OUTER JOIN [ShopFloor].[EBS].[Item] si ON si.[ItemNum] = sa.[ItemNum]
                    WHERE   sa.[TransferStatus] IN ( '', 'N' )
                    GROUP BY sa.[ItemNum]
                            ,ISNULL(si.[SpacerItemId], 0)
		                    ,CONVERT(DATE, (sa.[WrittenDate]))
                            ,sa.[TransferDate]
                            ,sa.[TransferMessage]
                            ,sa.[TransferBatchNum]
                            ,sa.[TransferUniqueID]";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public static SqlCommand GetShopFloorSpacerConsumption()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT  COUNT(sa.[SpacerLengthsCompleteId]) AS 'SourceRows'
                            ,ISNULL(si.[SpacerItemId], 0) AS 'SpacerItemId'
		                    ,CONVERT(VARCHAR, sa.[ItemNum]) AS 'ItemNum'
		                    ,SUM(sa.[Quantity]) AS 'Quantity'
		                    ,CONVERT(DATE, (sa.[WrittenDate])) AS 'ProductionDate'
		                    ,MIN(sa.[WrittenDate]) AS 'MinWrittenDate'
		                    ,MAX(sa.[WrittenDate]) AS 'MaxWrittenDate'
		                    ,'N' AS 'TransferStatus'
                            ,sa.[TransferDate]
                            ,sa.[TransferMessage]
                            ,sa.[TransferBatchNum] as 'TransferBatch'
                            ,sa.[TransferUniqueID]
                    FROM    [ShopFloor].[ebs].[SpacerConsumed] sa
                            LEFT OUTER JOIN [ShopFloor].[EBS].[Item] si ON si.[ItemNum] = sa.[ItemNum]
                    WHERE   sa.[TransferStatus] IN ( '', 'N' )
                    GROUP BY sa.[ItemNum]
                            ,ISNULL(si.[SpacerItemId], 0)
		                    ,CONVERT(DATE, (sa.[WrittenDate]))
                            ,sa.[TransferDate]
                            ,sa.[TransferMessage]
                            ,sa.[TransferBatchNum]
                            ,sa.[TransferUniqueID]";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return cmd;
        }

        public SpacerConsumptions GetSpacerConsumption(SqlConnection cnSFInt, string plantNum = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptions = new SpacerConsumptions();
            try
            {
                var cmd = GetSpacerConsumption();
                cmd.Connection = cnSFInt;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var spacerConsumption = new SpacerConsumption
                    {
                        PlantNum = plantNum,
                        SpacerItemId = CastDbNull.To(reader["SpacerItemId"], 0),
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatch"], 0),
                        TransferUniqueID = CastDbNull.To<long>(reader["TransferUniqueID"], 0),
                        ItemNum = CastDbNull.To(reader["ItemNum"], ""),
                        SourceRows = CastDbNull.To(reader["SourceRows"], 0),
                        Quantity = CastDbNull.To(reader["Quantity"], 0),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        MinWrittenDate = CastDbNull.To(reader["MinWrittenDate"], default(DateTime)),
                        MaxWrittenDate = CastDbNull.To(reader["MaxWrittenDate"], default(DateTime)),
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], "")
                    };
                    spacerConsumptions.Add(spacerConsumption);
                }
                reader.Close();
                Logger.Trace($"ROWS: {spacerConsumptions.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumptions;
        }

        public SpacerConsumptions GetShopFloorSpacerConsumption(SqlConnection cnSF, string plantNum = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptions = new SpacerConsumptions();
            try
            {
                var cmd = GetShopFloorSpacerConsumption();
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var spacerConsumption = new SpacerConsumption
                    {
                        PlantNum = plantNum,
                        SpacerItemId = CastDbNull.To(reader["SpacerItemId"], 0),
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatch"], 0),
                        TransferUniqueID = CastDbNull.To<long>(reader["TransferUniqueID"], 0),
                        ItemNum = CastDbNull.To(reader["ItemNum"], ""),
                        SourceRows = CastDbNull.To(reader["SourceRows"], 0),
                        Quantity = CastDbNull.To(reader["Quantity"], 0),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        MinWrittenDate = CastDbNull.To(reader["MinWrittenDate"], default(DateTime)),
                        MaxWrittenDate = CastDbNull.To(reader["MaxWrittenDate"], default(DateTime)),
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], ""),
                        UnitOfMeasure = CastDbNull.To(reader["UnitOfMeasure"], "")
                    };
                    spacerConsumptions.Add(spacerConsumption);
                }
                reader.Close();
                Logger.Trace($"ROWS: {spacerConsumptions.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumptions;
        }

        public static short Aggregate(SqlConnection cnSFInt, Guid transferGuid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            short rowCount;
            try
            {
                var cmd = new SqlCommand("[INTEGRATION].[dbo].[usp_AggregateSpacerConsumption]", cnSFInt)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@TransferGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@RetVal", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters["@TransferGuid"].Value = transferGuid;
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
                rowCount = Convert.ToInt16(cmd.Parameters["@RetVal"].Value);
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }

        public static short AggregateShopFloor(SqlConnection cnSF, Guid transferGuid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            short rowCount;
            try
            {
                var cmd = new SqlCommand("[EBS].[usp_AggregateShopFloorSpacerConsumption]", cnSF)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@TransferGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@RetVal", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters["@TransferGuid"].Value = transferGuid;
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
                rowCount = Convert.ToInt16(cmd.Parameters["@RetVal"].Value);
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }

        public static SpacerConsumptions GetSpacerConsumptionAggregations(SqlConnection cnSFInt, string plantNum, Guid transferGuid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptions = new SpacerConsumptions();
            try
            {
                SqlCommand cmd;
                //if (transferGuid == Guid.Empty)
                //{
                    cmd = GetSpacerConsumptionAggregations();
                //}
                //else
                //{
                //    cmd = GetSpacerConsumptionAggregationsGuid();
                //    cmd.Parameters["@TransferGuid"].Value = transferGuid;
                //}
                cmd.Connection = cnSFInt;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var spacerConsumption = new SpacerConsumption
                    {
                        ConsumptionAggregateId = CastDbNull.To(reader["ConsumptionAggregateId"], 0),
                        SpacerItemId = CastDbNull.To(reader["SpacerItemId"], 0),
                        PlantNum = plantNum,
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatch"], 0),
                        TransferUniqueID = CastDbNull.To<long>(reader["TransferUniqueID"], 0),
                        ItemNum = CastDbNull.To(reader["ItemNum"], ""),
                        SourceRows = CastDbNull.To(reader["SourceRows"], 0),
                        Quantity = CastDbNull.To(reader["Quantity"], 0),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        MinWrittenDate = CastDbNull.To(reader["MinWrittenDate"], default(DateTime)),
                        MaxWrittenDate = CastDbNull.To(reader["MaxWrittenDate"], default(DateTime)),
                        ValidationStatus = "Valid",
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], "")
                    };
                    spacerConsumption.TransferDateString = spacerConsumption.TransferDate.ToString("M/dd/yyyy hh:mm:ss tt");
                    spacerConsumptions.Add(spacerConsumption);
                }
                reader.Close();
                Logger.Trace($"ROWS: {spacerConsumptions.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumptions;
        }

        public static SpacerConsumptions GetShopFloorSpacerConsumptionAggregations(SqlConnection cnSF, string plantNum, Guid transferGuid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptions = new SpacerConsumptions();
            try
            {
                SqlCommand cmd;
                if (transferGuid == Guid.Empty)
                {
                    cmd = GetShopFloorSpacerConsumptionAggregations();
                }
                else
                {
                    cmd = GetShopFloorSpacerConsumptionAggregationsGuid();
                    cmd.Parameters["@TransferGuid"].Value = transferGuid;
                }
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var spacerConsumption = new SpacerConsumption
                    {
                        ConsumptionAggregateId = CastDbNull.To(reader["ConsumptionAggregateId"], 0),
                        SpacerItemId = CastDbNull.To(reader["SpacerItemId"], 0),
                        PlantNum = plantNum,
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatch"], 0),
                        TransferUniqueID = CastDbNull.To<long>(reader["TransferUniqueID"], 0),
                        ItemNum = CastDbNull.To(reader["ItemNum"], ""),
                        SourceRows = CastDbNull.To(reader["SourceRows"], 0),
                        Quantity = CastDbNull.To(reader["Quantity"], 0),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        MinWrittenDate = CastDbNull.To(reader["MinWrittenDate"], default(DateTime)),
                        MaxWrittenDate = CastDbNull.To(reader["MaxWrittenDate"], default(DateTime)),
                        ValidationStatus = "Valid",
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], ""),
                        UnitOfMeasure = CastDbNull.To(reader["UnitOfMeasure"], "")
                    };
                    spacerConsumption.TransferDateString = spacerConsumption.TransferDate.ToString("M/dd/yyyy hh:mm:ss tt");
                    spacerConsumptions.Add(spacerConsumption);
                }
                reader.Close();
                Logger.Trace($"ROWS: {spacerConsumptions.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumptions;
        }

        public static List<string> Validate(SpacerConsumptions spacerConsumptions, BusinessSystemSession e1Session)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptionInteropManager = new SpacerConsumptionInteropManager(e1Session);
            var validationErrors = new List<string>();
            if (spacerConsumptions.Any())
            {
                foreach (var spacerConsumption in spacerConsumptions.Where(spacerConsumption => spacerConsumption.SpacerItemId == 0))
                {
                    spacerConsumption.IsValid = false;
                    spacerConsumption.Transferred = false;
                    spacerConsumption.TransferStatus = "E";
                    spacerConsumption.ValidationStatus = "Invalid ItemNumber: " + spacerConsumption.ItemNum;
                    spacerConsumption.TransferMessage = "Invalid ItemNumber: " + spacerConsumption.ItemNum;
                    validationErrors.Add("Invalid ItemNumber: " + spacerConsumption.ItemNum);
                }
                validationErrors.AddRange(spacerConsumptionInteropManager.ValidateE1(e1Session, spacerConsumptions));
            }
            Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            return validationErrors;
        }

        public static void UpdateSpacerConsumption(SqlConnection cnSFInt, SpacerConsumptions spacerConsumptions, short sFUpdateBatchSize)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var cmd = new SqlCommand
            {
                Connection = cnSFInt,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var spacerConsumption in spacerConsumptions)
                {
                    rowCount++;
                    var cmdStatement = "EXEC [dbo].[usp_UpdateSpacerConsumptionAggregate] " +
                                        $"{spacerConsumption.ConsumptionAggregateId}, " +
                                        $"'{spacerConsumption.TransferBatchNum}', " +
                                        $"{spacerConsumption.TransferUniqueID}, " +
                                        $"'{spacerConsumption.TransferMessage}', " +
                                        $"'{spacerConsumption.TransferStatus}', " +
                                        $"{spacerConsumption.Transferred};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        Logger.Trace(Logging.DbCmdString(cmd));
                        cmd.ExecuteNonQuery();
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    cmd.ExecuteNonQuery();
                }
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString }  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static void UpdateShopFloorSpacerConsumption(SqlConnection cnSF, SpacerConsumptions spacerConsumptions, short sFUpdateBatchSize)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var cmd = new SqlCommand
            {
                Connection = cnSF,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var spacerConsumption in spacerConsumptions)
                {
                    rowCount++;
                    var cmdStatement = "EXEC [EBS].[usp_UpdateSpacerConsumptionAggregate] " +
                                        $"{spacerConsumption.ConsumptionAggregateId}, " +
                                        $"'{spacerConsumption.TransferBatchNum}', " +
                                        $"{spacerConsumption.TransferUniqueID}, " +
                                        $"'{spacerConsumption.TransferMessage}', " +
                                        $"'{spacerConsumption.TransferStatus}', " +
                                        $"{spacerConsumption.Transferred};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        Logger.Trace(Logging.DbCmdString(cmd));
                        cmd.ExecuteNonQuery();
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    cmd.ExecuteNonQuery();
                }
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString }  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }
    }
}
