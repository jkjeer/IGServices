using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.E1
{
    public class F580020Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string shopFloorSystemIGMTS = "6";
        //private const string shopFloorSystemIGMTO = "2";

        private static iDB2Command UpdateBeforeProcessingF580020()
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'U', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'In Use',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    WHERE S6MCU = ?
                        AND S6Z58SCN = ?
                        AND S6Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCN"].Precision = 5;
            cmd.Parameters["S6Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCD"].Precision = 6;
            cmd.Parameters["S6Z58SCD"].Scale = 0;
            return cmd;
        }

        private static iDB2Command UpdateAfterProcessingSuccessF580020()
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'Y', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Complete',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    WHERE S6MCU = ?
                    AND S6Z58SCN = ?
                    AND S6Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCN"].Precision = 5;
            cmd.Parameters["S6Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCD"].Precision = 6;
            cmd.Parameters["S6Z58SCD"].Scale = 0;
            return cmd;
        }

        private static iDB2Command UpdateAfterProcessingNetworkF580020()
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'N', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Retry',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    WHERE S6MCU = ?
                    AND S6Z58SCN = ?
                    AND S6Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCN"].Precision = 5;
            cmd.Parameters["S6Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCD"].Precision = 6;
            cmd.Parameters["S6Z58SCD"].Scale = 0;
            return cmd;
        }

        private static iDB2Command UpdateAfterProcessingFailF580020()
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'E', 
	                    S6WOPST = 'Error',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    WHERE S6MCU = ?
                    AND S6Z58SCN = ?
                    AND S6Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCN"].Precision = 5;
            cmd.Parameters["S6Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCD"].Precision = 6;
            cmd.Parameters["S6Z58SCD"].Scale = 0;
            return cmd;
        }

        private static iDB2Command GetRecentScheduleDateE1()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
	                SELECT
                        S6EDBT, 
                        S6Z58SCD 
                    FROM 
                        F580020 
                    WHERE 
                        S6MCU = ? AND
                        S6Z58SCN = ? 
                    ORDER BY S6Z58SCD DESC 
                    FETCH FIRST 1 ROW ONLY
                    FOR READ ONLY
                "
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            return cmd;
        }

        private static iDB2Command CancelF580020sDynamic(IReadOnlyCollection<string> filters, string desc = null)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            if (desc == null)
            {
                desc = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'E', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Complete',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command ResetF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'N', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Reset',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command SuccessF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'Y', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Complete',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command CompleteF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'C', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Complete',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command ErrorF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'E', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Error',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command BoxedF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'B', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Complete',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Get F580020s Dynamic
         *******************************************************************************/
        private static iDB2Command GetF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        S6EDBT,
                        S6MCU,
                        S6Z58SCN,
                        S6Z58SCD,
                        S6Z58SFS,
                        DRDL01,
                        S6TSC,
                        S6EDSP,
                        S6MATH06,
                        S6WOPST,
                        S6DS50
                    FROM
                        F580020
						INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
						ON  DRSY = '58' AND 
                            DRRT = 'SY' AND 
                            TRIM(DRKY) = S6Z58SFS
                    {filterString}
                    FOR READ ONLY"
            };
            return cmd;
        }

        /*********************************************************************************
        * Reserve F580020s Dynamic
        *******************************************************************************/
        private static iDB2Command ReserveF580020sDynamic(IReadOnlyCollection<string> filters)
        {
            var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime());
            var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime());
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE F580020
                    SET S6EDSP = 'R', 
	                    S6MATH06 = 0, 
	                    S6WOPST = 'Reserved',
	                    S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50)),
                        S6UPMJ = {e1Date},
                        S6UPMT = {e1Time}
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
          * Methods
          ********************************************************************************/
        public static int ReserveUnprocessedF580020s(BusinessSystemSession e1Session, string plantNum, string shopFloorSystem)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("S6MCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // IG ShopFloorSystem
                if (shopFloorSystem == shopFloorSystemIGMTS)
                {
                    // IG MTS
                    filters.Add("S6Z58SFS IN ( '6' ) AND \r\n");
                }
                else
                {
                    // IG MTO
                    filters.Add("S6Z58SFS IN ( '2', '7' ) AND \r\n");
                }

                // Unprocessed
                filters.Add("S6TSC = 'Y' AND \r\n");
                int lookbackMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["EDSPLookbackMinutes"]);
                var e1Date = E1TimeManager.CastToE1Date(DateTime.Now.ToLocalTime().AddMinutes(-lookbackMinutes));
                var e1Time = E1TimeManager.CastToE1Time(DateTime.Now.ToLocalTime().AddMinutes(-lookbackMinutes));
                filters.Add($@"( ( S6EDSP = 'N' ) OR ( S6EDSP in ( 'R', 'U' ) AND ( ( S6UPMJ < {e1Date} ) OR ( S6UPMJ = {e1Date} AND S6UPMT < {e1Time} ) ) ) )");

                var cmd = ReserveF580020sDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static F580020s GetReservedF580020s(BusinessSystemSession e1Session, string plantNum, string shopFloorSystem)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f580020s = new F580020s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S6MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // IG ShopFloorSystem
                if ( shopFloorSystem == shopFloorSystemIGMTS)
                {
                    // IG MTS
                    filters.Add("S6Z58SFS IN ( '6' ) AND \r\n");
                }
                else
                {
                    // IG MTO
                    filters.Add("S6Z58SFS IN ( '2', '7' ) AND \r\n");
                }
                // Unprocessed
                filters.Add("S6TSC = 'Y' AND \r\n");
                filters.Add("S6EDSP = 'R' AND \r\n");
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetF580020sDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var f580020 = new F580020
                    {
                        S6EDBT = CastDbNull.To(reader["S6EDBT"], string.Empty).Trim(),
                        S6MCU = CastDbNull.To(reader["S6MCU"], string.Empty).Trim(),
                        S6Z58SCN = CastDbNull.To<decimal>(reader["S6Z58SCN"], 0),
                        S6Z58SCD = CastDbNull.To<decimal>(reader["S6Z58SCD"], 0),
                        S6Z58SFS = CastDbNull.To(reader["S6Z58SFS"], string.Empty).Trim(),
                        S6TSC = CastDbNull.To(reader["S6TSC"], string.Empty).Trim(),
                        S6EDSP = CastDbNull.To(reader["S6EDSP"], string.Empty).Trim(),
                        S6MATH06 = CastDbNull.To<decimal>(reader["S6MATH06"], 0),
                        S6WOPST = CastDbNull.To(reader["S6WOPST"], string.Empty).Trim(),
                        S6DS50 = CastDbNull.To(reader["S6DS50"], string.Empty).Trim()
                    };
                    f580020s.Add(f580020);
                }
                reader.Close();
                Logger.Trace($"ROWS: {f580020s.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return f580020s;
        }

        public static F580020s GetUnprocessedF580020s(BusinessSystemSession e1Session, string plantNum, string shopFloorSystem)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var f580020s = new F580020s();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S6MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // IG ShopFloorSystem
                if (shopFloorSystem == shopFloorSystemIGMTS)
                {
                    // IG MTS
                    filters.Add("S6Z58SFS IN ( '6' ) AND \r\n");
                }
                else
                {
                    // IG MTO
                    filters.Add("S6Z58SFS IN ( '2', '7' ) AND \r\n");
                }
                // Unprocessed
                filters.Add("S6TSC = 'Y' AND \r\n");
                filters.Add("S6EDSP = 'N' AND \r\n");
                //filters.Add("( ( S6EDSP = 'N' ) OR ( S6EDSP = 'U' AND S6DS50 <= CHAR( CURRENT_TIMESTAMP - 3 MINUTES ) ) ) AND \r\n");
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                var cmd = GetF580020sDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var f580020 = new F580020
                    {
                        S6EDBT = CastDbNull.To(reader["S6EDBT"], string.Empty).Trim(),
                        S6MCU = CastDbNull.To(reader["S6MCU"], string.Empty).Trim(),
                        S6Z58SCN = CastDbNull.To<decimal>(reader["S6Z58SCN"], 0),
                        S6Z58SCD = CastDbNull.To<decimal>(reader["S6Z58SCD"], 0),
                        S6Z58SFS = CastDbNull.To(reader["S6Z58SFS"], string.Empty).Trim(),
                        S6TSC = CastDbNull.To(reader["S6TSC"], string.Empty).Trim(),
                        S6EDSP = CastDbNull.To(reader["S6EDSP"], string.Empty).Trim(),
                        S6MATH06 = CastDbNull.To<decimal>(reader["S6MATH06"], 0),
                        S6WOPST = CastDbNull.To(reader["S6WOPST"], string.Empty).Trim(),
                        S6DS50 = CastDbNull.To(reader["S6DS50"], string.Empty).Trim()
                    };
                    f580020s.Add(f580020);
                }
                reader.Close();
                Logger.Trace($"ROWS: {f580020s.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return f580020s;
        }

        public static int CancelF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, string desc = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = CancelF580020sDynamic(filters, desc);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int ResetF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, string desc = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = ResetF580020sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int SuccessF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, string desc = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = SuccessF580020sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int CompleteF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum, string desc = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = CompleteF580020sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int ErrorF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = ErrorF580020sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int BoxedF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                        throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                    throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}");
                }
                else
                {
                    // Plant Num
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        filters.Add("S6MCU = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plantNum.PadLeft(12);
                    }
                    // MTOSchedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("S6Z58SCN = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 5;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }
                    // MTOSchedule Date
                    if (scheduleDateE1 > 0)
                    {
                        filters.Add("S6Z58SCD = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                        parameters.Last().Precision = 6;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleDateE1;
                    }
                    // Batch Num
                    if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                    {
                        filters.Add("S6EDBT = ? AND \r\n");
                        parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                        parameters.Last().Value = batchNum;
                    }
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = BoxedF580020sDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = e1Session.DbConnection;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount = cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static decimal GetRecentScheduleDateE1(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            decimal recentScheduleDateE1 = 0;
            try
            {
                var cmd = GetRecentScheduleDateE1();
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    var f580020 = new F580020
                    {
                        S6EDBT = CastDbNull.To(reader["S6EDBT"], string.Empty).Trim(),
                        S6Z58SCD = CastDbNull.To<decimal>(reader["S6Z58SCD"], 0)
                    };
                    recentScheduleDateE1 = f580020.S6Z58SCD;
                    rowCount++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return recentScheduleDateE1;
        }

        public static F580020 GetF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            var f580020 = new F580020();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    var step1GetRecentScheduleDateE1 = new ProcessStep("Step 1. Get Recent Schedule Date E1 F580020");
                    try
                    {
                        scheduleDateE1 = GetRecentScheduleDateE1(e1Session, plantNum, scheduleNum);
                        step1GetRecentScheduleDateE1.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                        throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
                    }
                    finally
                    {
                        step1GetRecentScheduleDateE1.Done();
                    }
                }

                if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
                {
                    //processResponse.Errors.Add(new Error($@"Unable to identify a schedule date for Plant {plantNum} Schedule {scheduleNum}"));
                }
                else
                {
                    var step2BuildDynamicSQL = new ProcessStep("Step 2. Build dynamic SQL statement");
                    try
                    {
                        // Plant Num
                        if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                        {
                            filters.Add("S6MCU = ? AND \r\n");
                            parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                            parameters.Last().Value = plantNum.PadLeft(12);
                        }
                        else
                        {
                            step2BuildDynamicSQL.Errors.Add(new Error($@"GetF580020 called for Sched {scheduleNum} without a Plant Code"));
                        }
                        // MTOSchedule Num
                        if (scheduleNum > 0)
                        {
                            filters.Add("S6Z58SCN = ? AND \r\n");
                            parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                            parameters.Last().Precision = 5;
                            parameters.Last().Scale = 0;
                            parameters.Last().Value = scheduleNum;
                        }
                        else
                        {
                            step2BuildDynamicSQL.Errors.Add(new Error($@"GetF580020 called for Plant {plantNum} without a Schedule Number"));
                        }
                        // MTOSchedule Date
                        if (scheduleDateE1 > 0)
                        {
                            filters.Add("S6Z58SCD = ? AND \r\n");
                            parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                            parameters.Last().Precision = 6;
                            parameters.Last().Scale = 0;
                            parameters.Last().Value = scheduleDateE1;
                        }
                        // Batch Num
                        if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                        {
                            filters.Add("S6EDBT = ? AND \r\n");
                            parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                            parameters.Last().Value = batchNum;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                        throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
                    }
                    finally
                    {
                        step2BuildDynamicSQL.Done();
                    }

                    if (!step2BuildDynamicSQL.HasErrors())
                        {
                        var step3GetF580020 = new ProcessStep("Step 3. Get F580020");
                        try
                        {
                            if (filters.Any())
                            {
                                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                                var cmd = GetF580020sDynamic(filters);
                                foreach (var parameter in parameters)
                                {
                                    cmd.Parameters.Add(parameter);
                                }
                                cmd.Connection = e1Session.DbConnection;
                                Logger.Trace(Logging.DbCmdString(cmd));
                                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                                var rowCount = 0;
                                while (reader.Read())
                                {
                                    f580020.S6EDBT = CastDbNull.To(reader["S6EDBT"], string.Empty).Trim();
                                    f580020.S6MCU = CastDbNull.To(reader["S6MCU"], string.Empty).Trim();
                                    f580020.S6Z58SCN = CastDbNull.To<decimal>(reader["S6Z58SCN"], 0);
                                    f580020.S6Z58SCD = CastDbNull.To<decimal>(reader["S6Z58SCD"], 0);
                                    f580020.S6Z58SFS = CastDbNull.To(reader["S6Z58SFS"], string.Empty).Trim();
                                    f580020.S6Z58SFS_NAME = CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                                    f580020.S6TSC = CastDbNull.To(reader["S6TSC"], string.Empty).Trim();
                                    f580020.S6EDSP = CastDbNull.To(reader["S6EDSP"], string.Empty).Trim();
                                    f580020.S6MATH06 = CastDbNull.To<decimal>(reader["S6MATH06"], 0);
                                    f580020.S6WOPST = CastDbNull.To(reader["S6WOPST"], string.Empty).Trim();
                                    f580020.S6DS50 = CastDbNull.To(reader["S6DS50"], string.Empty).Trim();
                                    rowCount++;
                                }
                                reader.Close();
                                Logger.Trace("ROWS: " + rowCount);
                            }
                            else // if (filters.Any())
                            {
                                step3GetF580020.Errors.Add(new Error(@"GetF580020 called with no filters"));
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                            throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
                        }
                        finally
                        {
                            step3GetF580020.Done();
                        }
                    } // if (!step2BuildDynamicSQL.HasErrors())
                } // if (scheduleDateE1 == 0 && string.IsNullOrEmpty(batchNum))
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return f580020;
        }

        public static int UpdateBeforeProcessingF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F580020Manager.UpdateBeforeProcessingF580020")) ? e1Session.DbCommands["F580020Manager.UpdateBeforeProcessingF580020"] : UpdateBeforeProcessingF580020();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Parameters["S6Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int UpdateAfterProcessingSuccessF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F580020Manager.UpdateAfterProcessingSuccessF580020")) ? e1Session.DbCommands["F580020Manager.UpdateAfterProcessingSuccessF580020"] : UpdateAfterProcessingSuccessF580020();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Parameters["S6Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            { 
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int UpdateAfterProcessingNetworkF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F580020Manager.UpdateAfterProcessingNetworkF580020")) ? e1Session.DbCommands["F580020Manager.UpdateAfterProcessingNetworkF580020"] : UpdateAfterProcessingNetworkF580020();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Parameters["S6Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        public static int UpdateAfterProcessingFailF580020(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("PLANT: " + plantNum + " SCHEDULE: " + scheduleNum + " BEGIN: " + DateTime.Now.ToLocalTime());
            int rowCount;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F580020Manager.UpdateAfterProcessingFailF580020")) ? e1Session.DbCommands["F580020Manager.UpdateAfterProcessingFailF580020"] : UpdateAfterProcessingFailF580020();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Parameters["S6Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        /*****************************************************************************
        *   UPDATE STATUS
        ******************************************************************************/
        private static iDB2Command UpdateStatus()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                UPDATE 
                    F580020
                SET 
                    S6EDSP = ?, 
	                S6WOPST = ?,
	                S6DS50 = CAST(CURRENT_TIMESTAMP AS CHAR(50))
                WHERE 
                    S6MCU = ? AND
                    S6Z58SCN = ? AND
                    S6EDBT = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("EDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("OPST", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58SCN"].Precision = 5;
            cmd.Parameters["Z58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("EDBT", iDB2DbType.iDB2Graphic, 15));
            return cmd;
        }

        public static SqlDmlResponse UpdateStatus(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, string batchNum, string edsp, string opst)
        {
            var sqlResponse = new SqlDmlResponse();
            try
            {
                var cmd = UpdateStatus();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["EDSP"].Value = edsp;
                cmd.Parameters["OPST"].Value = opst;
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["Z58SCN"].Value = scheduleNum;
                cmd.Parameters["EDBT"].Value = batchNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                sqlResponse.Done(rows);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateStatus(BusinessSystemSession session, string plantNum, decimal scheduleNum, string edsp, string opst)
        {
            var sqlResponse = new SqlDmlResponse();
            try
            {
                var batchNum = GetRecentBatch(session, plantNum, scheduleNum);
                var cmd = UpdateStatus();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["EDSP"].Value = edsp;
                cmd.Parameters["OPST"].Value = opst;
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["Z58SCN"].Value = scheduleNum;
                cmd.Parameters["EDBT"].Value = batchNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                sqlResponse.Done(rows);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }

        public static SqlDmlResponse UpdateStatus(BusinessSystemSession session, ScheduleEvent evt, string edsp, string opst)
        {
            var sqlResponse = new SqlDmlResponse();
            try
            {
                var cmd = UpdateStatus();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["EDSP"].Value = edsp;
                cmd.Parameters["OPST"].Value = opst;
                cmd.Parameters["MCU"].Value = evt.Plant.PlantNum.PadLeft(12);
                cmd.Parameters["Z58SCN"].Value = evt.ScheduleNum;
                cmd.Parameters["EDBT"].Value = evt.BatchNum;
                Logger.Trace(Logging.DbCmdString(cmd));
                sqlResponse.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                sqlResponse.Done(rows);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e);
            }
            return sqlResponse;
        }


        private static iDB2Command GetRecentBatch()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
	                SELECT
                        S6EDBT
                    FROM 
                        F580020 
                    WHERE 
                        S6MCU = ? AND
                        S6Z58SCN = ? 
                    ORDER BY S6Z58SCD DESC FETCH FIRST 1 ROW ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("S6MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("S6Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S6Z58SCN"].Precision = 5;
            cmd.Parameters["S6Z58SCN"].Scale = 0;
            return cmd;
        }

        public static string GetRecentBatch(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum)
        {
            string batchNum = "";
            try
            {
                var cmd = GetRecentBatch();
                cmd.Parameters["S6MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["S6Z58SCN"].Value = scheduleNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    batchNum = CastDbNull.To(reader["S6EDBT"], String.Empty);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return batchNum;
        }
    }
}
