using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
//using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class GlassConsumptionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlassConsumptionManager()
        {
        }

        public GlassConsumptionManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetExistingF580030", GetExistingF580030()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command GetExistingF580030()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                        SELECT
                            COUNT(*) AS RECS
                        FROM
                            F580030
                        WHERE
                            SFMMCU = ?
                            AND SFEDCT = ?
                            AND SFZ58UID = ? ";
                cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
                cmd.Parameters.Add(new iDB2Parameter("SFZ58UID", iDB2DbType.iDB2Decimal));
                cmd.Parameters["SFZ58UID"].Precision = 14;
                cmd.Parameters["SFZ58UID"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public SqlCommand GetUnprocessedGlassConsumption()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
			        SELECT  [GLStockID]
                            ,[ProductionLineCode]
                            ,[LineOperationCode]
                            ,[LineTypeCode]
                            ,[E1WorkCenter]
                            ,[E1Operation]
                            ,[RawThick]
                            ,[RawCTCFCode]
                            ,[Color]
                            ,[HeatTreatment]
                            ,[1stSrfCoating]
                            ,[2ndSrfCoating]
                            ,[Pattern]
                            ,[RawGlassBaseL]
                            ,[RawGlassBaseN]
                            ,[RawGlassBaseD]
                            ,[RawGlassLeftL]
                            ,[RawGlassLeftN]
                            ,[RawGlassLeftD]
                            ,[SheetsConsumed]
                            ,[SheetsRejected]
                            ,[Shift]
                            ,[CuttingTable]
                            ,[RawCrateNumber]
                            ,[WrittenDate]
                            ,[Complete]
                            ,[Transferred]
                            ,[TransferDate]
                            ,[Upload]
                            ,[Crew]
                            ,[RejectCode]
                            ,[ProductionDate]
                            ,[Workstation]
                            ,[RawGlassRightL]
                            ,[RawGlassRightN]
                            ,[RawGlassRightD]
                            ,[RawGlassTopL]
                            ,[RawGlassTopN]
                            ,[RawGlassTopD]
                            ,[RawGlassM1L]
                            ,[RawGlassM1N]
                            ,[RawGlassM1D]
                            ,[RawGlassM2L]
                            ,[RawGlassM2N]
                            ,[RawGlassM2D]
                            ,[DoNotUpload]
                            ,[ShiftOverride]
                            ,[DefaultSN]
                            ,[DefaultSNNotification]
                            ,[DefaultSNNotificationTS]
                            ,[Archive]
                            ,[SNUpdateUID]
                            ,[ConsumedStoceID]
                            ,[RejectedStoceID]
                            ,[ReportingOnly]
                            ,[TimeStamp]
                            ,[TransferStatus]
                            ,[TransferMessage]
                            ,[TransferBatchNum] 
                            ,[TransferUniqueID]
			        FROM    [SHOPFLOOR].[dbo].[GlassStock] 
                    WHERE ( [TransferStatus] = 'L' )
                       OR ( ISNULL([Upload], 0) = 0
                            AND [Complete] != 'Yes'
                            AND ISNULL([DoNotUpload], 0) = 0
                          )
                    "
            };
            return cmd;
        }

        public SqlCommand GetLockedGlassConsumption()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
			        SELECT  [GLStockID]
                            ,[ProductionLineCode]
                            ,[LineOperationCode]
                            ,[LineTypeCode]
                            ,[E1WorkCenter]
                            ,[E1Operation]
                            ,[RawThick]
                            ,[RawCTCFCode]
                            ,[Color]
                            ,[HeatTreatment]
                            ,[1stSrfCoating]
                            ,[2ndSrfCoating]
                            ,[Pattern]
                            ,[RawGlassBaseL]
                            ,[RawGlassBaseN]
                            ,[RawGlassBaseD]
                            ,[RawGlassLeftL]
                            ,[RawGlassLeftN]
                            ,[RawGlassLeftD]
                            ,[SheetsConsumed]
                            ,[SheetsRejected]
                            ,[Shift]
                            ,[CuttingTable]
                            ,[RawCrateNumber]
                            ,[WrittenDate]
                            ,[Complete]
                            ,[Transferred]
                            ,[TransferDate]
                            ,[Upload]
                            ,[Crew]
                            ,[RejectCode]
                            ,[ProductionDate]
                            ,[Workstation]
                            ,[RawGlassRightL]
                            ,[RawGlassRightN]
                            ,[RawGlassRightD]
                            ,[RawGlassTopL]
                            ,[RawGlassTopN]
                            ,[RawGlassTopD]
                            ,[RawGlassM1L]
                            ,[RawGlassM1N]
                            ,[RawGlassM1D]
                            ,[RawGlassM2L]
                            ,[RawGlassM2N]
                            ,[RawGlassM2D]
                            ,[DoNotUpload]
                            ,[ShiftOverride]
                            ,[DefaultSN]
                            ,[DefaultSNNotification]
                            ,[DefaultSNNotificationTS]
                            ,[Archive]
                            ,[SNUpdateUID]
                            ,[ConsumedStoceID]
                            ,[RejectedStoceID]
                            ,[ReportingOnly]
                            ,[TimeStamp]
                            ,[TransferStatus]
                            ,[TransferMessage]
                            ,[TransferBatchNum] 
                            ,[TransferUniqueID]
			        FROM    [SHOPFLOOR].[dbo].[GlassStock] 
                    WHERE   [TransferStatus] = 'L'
                    "
            };
            return cmd;
        }

        public GlassConsumptions GetUnprocessedGlassConsumption(string plantNum, SqlConnection cnSF)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var glassConsumptions = new GlassConsumptions();
            try
            {
                var cmd = GetUnprocessedGlassConsumption();
                cmd.Connection = cnSF;
                Logger.Trace("CN: " + cnSF.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var glassConsumption = new GlassConsumption
                    {
                        PlantNum = plantNum,
                        ConsumptionId = CastDbNull.To(reader["GLStockID"], 0),
                        RawCrateNumber = CastDbNull.To(reader["RawCrateNumber"], ""),
                        Shift = CastDbNull.To(reader["Shift"], ""),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        SheetsConsumed = CastDbNull.To(reader["SheetsConsumed"], 0),
                        SheetsRejected = CastDbNull.To(reader["SheetsRejected"], 0),
                        CuttingTable = CastDbNull.To(reader["CuttingTable"], ""),
                        RawCTCFCode = CastDbNull.To(reader["RawCTCFCode"], ""),
                        Crew = CastDbNull.To(reader["Crew"], ""),
                        E1WorkCenter = CastDbNull.To(reader["E1WorkCenter"], ""),
                        E1Operation = CastDbNull.To(reader["E1Operation"], ""),
                        RawGlassBaseL = CastDbNull.To<byte>(reader["RawGlassBaseL"], 0),
                        RawGlassLeftL = CastDbNull.To<byte>(reader["RawGlassLeftL"], 0),
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatchNum"], 0),
                        TransferUniqueID = CastDbNull.To<decimal>(reader["TransferUniqueID"], 0),
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], "")
                    };
                    if (glassConsumption.SheetsConsumed == 0 && glassConsumption.SheetsRejected == 0)
                    {
                        glassConsumption.Quantity = 1;
                    }
                    else
                    {
                        glassConsumption.Quantity = glassConsumption.SheetsConsumed;
                    }
                    glassConsumptions.Add(glassConsumption);
                }
                reader.Close();
                Logger.Trace("ROWS: " + glassConsumptions.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSF.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return glassConsumptions;
        }

        public GlassConsumptions GetLockedGlassConsumption(string plantNum, SqlConnection cnSF)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var glassConsumptions = new GlassConsumptions();
            try
            {
                var cmd = GetLockedGlassConsumption();
                cmd.Connection = cnSF;
                Logger.Trace("CN: " + cnSF.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var glassConsumption = new GlassConsumption
                    {
                        PlantNum = plantNum,
                        ConsumptionId = CastDbNull.To(reader["GLStockID"], 0),
                        RawCrateNumber = CastDbNull.To(reader["RawCrateNumber"], ""),
                        Shift = CastDbNull.To(reader["Shift"], ""),
                        ProductionDate = CastDbNull.To(reader["ProductionDate"], default(DateTime)),
                        SheetsConsumed = CastDbNull.To(reader["SheetsConsumed"], 0),
                        SheetsRejected = CastDbNull.To(reader["SheetsRejected"], 0),
                        CuttingTable = CastDbNull.To(reader["CuttingTable"], ""),
                        RawCTCFCode = CastDbNull.To(reader["RawCTCFCode"], ""),
                        Crew = CastDbNull.To(reader["Crew"], ""),
                        E1WorkCenter = CastDbNull.To(reader["E1WorkCenter"], ""),
                        E1Operation = CastDbNull.To(reader["E1Operation"], ""),
                        RawGlassBaseL = CastDbNull.To<byte>(reader["RawGlassBaseL"], 0),
                        RawGlassLeftL = CastDbNull.To<byte>(reader["RawGlassLeftL"], 0),
                        TransferBatchNum = CastDbNull.To<long>(reader["TransferBatchNum"], 0),
                        TransferUniqueID = CastDbNull.To<decimal>(reader["TransferUniqueID"], 0),
                        TransferStatus = CastDbNull.To(reader["TransferStatus"], ""),
                        TransferDate = CastDbNull.To(reader["TransferDate"], DateTime.Now.ToLocalTime()),
                        TransferMessage = CastDbNull.To(reader["TransferMessage"], "")
                    };
                    if (glassConsumption.SheetsConsumed == 0 && glassConsumption.SheetsRejected == 0)
                    {
                        glassConsumption.Quantity = 1;
                    }
                    else
                    {
                        glassConsumption.Quantity = glassConsumption.SheetsConsumed;
                    }
                    glassConsumptions.Add(glassConsumption);
                }
                reader.Close();
                Logger.Trace("ROWS: " + glassConsumptions.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSF.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return glassConsumptions;
        }

        public bool Lock(SqlConnection cnSFInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            bool isLock;
            try
            {
                var cmd = new SqlCommand("[INTEGRATION].[dbo].[usp_LockGlassStock]", cnSFInt)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@RetVal", SqlDbType.Bit).Direction = ParameterDirection.Output;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                cmd.ExecuteNonQuery();
                var lockCount = Convert.ToInt16(cmd.Parameters["@RetVal"].Value);
                Logger.Trace("ROWS: " + lockCount);
                isLock = (lockCount > 0);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return isLock;
        }

        public bool Unlock(SqlConnection cnSFInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            bool isLock;
            try
            {
                var cmd = new SqlCommand("[INTEGRATION].[dbo].[usp_UnlockGlassStock]", cnSFInt)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@RetVal", SqlDbType.Bit).Direction = ParameterDirection.Output;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                cmd.ExecuteNonQuery();
                var lockCount = Convert.ToInt16(cmd.Parameters["@RetVal"].Value);
                Logger.Trace("ROWS: " + lockCount);
                isLock = (lockCount > 0);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return isLock;
        }

        public bool Validate(BusinessSystemSession e1Session, SqlConnection cnSFInt, GlassConsumptions glassConsumptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var isValid = true;
            if (glassConsumptions.Any())
            {
                if (!ValidateE1(e1Session, glassConsumptions))
                {
                    isValid = false;
                }
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return isValid;
        }

        public bool ValidateE1(BusinessSystemSession e1Session, GlassConsumptions glassConsumptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var isValid = true;
            foreach (var glassConsumption in glassConsumptions)
            {
                if (IsExistingF580030(e1Session, glassConsumption.PlantNum, glassConsumption.ConsumptionId))
                {
                    isValid = false;
                    glassConsumption.IsValid = false;
                    glassConsumption.Transferred = true;
                    glassConsumption.TransferStatus = "Y";
                    glassConsumption.ValidationStatus = "Previously Uploaded";
                    glassConsumption.TransferMessage = "Previously Uploaded to F580030";
                }
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return isValid;
        }

        public bool IsExistingF580030(BusinessSystemSession e1Session, string plantNum, long consumptionId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retValue = false;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GlassConsumptionManager.GetExistingF580030")) ? e1Session.DbCommands["GlassConsumptionManager.GetExistingF580030"] : GetExistingF580030();
                cmd.Parameters["SFMMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SFEDCT"].Value = "GS";
                cmd.Parameters["SFZ58UID"].Value = consumptionId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                var recs = 0;
                while (reader.Read())
                {
                    recs = CastDbNull.To(reader["RECS"], 0);
                    if (recs > 0) { retValue = true; }
                }
                reader.Close();
                Logger.Trace("ROWS: " + recs);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retValue;
        }

        /******************************************************************
        * Get E1 SFEDBT Batch 
        ******************************************************************/
        public long GetNextSFEDBT(SqlConnection cnEDBT)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            long batchNum = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[SFEDBT] AS 'SFEDBT'",
                    Connection = cnEDBT
                };
                Logger.Trace("CN: " + cnEDBT.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    batchNum = CastDbNull.To<long>(reader["SFEDBT"], 0);
                    counter += 1;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnEDBT.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return batchNum;
        }

        public void GetSFUKIDs(GlassConsumptions glassConsumptions, SqlConnection cnSFUKID)
        {
            var numberOfSFUKIDS = glassConsumptions.Count(glassConsumption => glassConsumption.IsValid);
            var firstSFUKID = GetFirstSFUKIDRange(cnSFUKID, numberOfSFUKIDS);
            foreach (var glassConsumption in glassConsumptions.Where(glassConsumption => glassConsumption.IsValid))
            {
                glassConsumption.TransferUniqueID = firstSFUKID;
                firstSFUKID = firstSFUKID + 1;
            }
        }

        public long GetFirstSFUKIDRange(SqlConnection cnSFUKID, int numberOfSFUKIDS)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            long firstSFUKID;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSFUKID,
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
                Logger.Trace("CN: " + cnSFUKID.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                cmd.ExecuteNonQuery();
                firstSFUKID = CastDbNull.To<long>(firstValueInRange.Value, 0);
                Logger.Trace("ROWS: " + numberOfSFUKIDS);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSFUKID.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return firstSFUKID;
        }

        public void UpdateGlassConsumption(GlassConsumptions glassConsumptions, short sFUpdateBatchSize, SqlConnection cnSFInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var cmd = new SqlCommand
            {
                Connection = cnSFInt,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var glassConsumption in glassConsumptions)
                {
                    rowCount += 1;
                    var cmdStatement = "EXEC [INTEGRATION].[dbo].[usp_UpdateGlassConsumption] " +
                                        $"{glassConsumption.ConsumptionId}, " +
                                        $"'{glassConsumption.TransferBatchNum}', " +
                                        $"{glassConsumption.TransferUniqueID}, " +
                                        $"'{glassConsumption.TransferMessage}', " +
                                        $"'{glassConsumption.TransferStatus}', " +
                                        $"{glassConsumption.Transferred};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        cmd.ExecuteNonQuery();
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    Logger.Trace("CN: " + cnSFInt.ConnectionString);
                    Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSFInt.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }
    }
}
