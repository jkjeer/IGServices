using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using NLog;

namespace IGServices.Common.Utilities
{
    public class HighWaterMarkManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static SqlCommand GetHighWaterMarks(string filterString)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
                    MarkId,
                    MarkName ,
                    EnterpriseDateTime ,
                    LocalDateTime ,
                    LookbackSeconds ,
                    UniqueID ,
                    WrittenDate,
                    InitStatement
                FROM 
                    EBS.HighWaterMark {filterString}"
            };
            return cmd;
        }

        public static HighWaterMarks GetHighWaterMarks(SqlConnection cn, Plant plant, List<string> markNames)
        {
            var marks = new HighWaterMarks();
            try
            {
                var inString = new StringBuilder();
                if (markNames.Count > 0)
                {
                    inString.Append(" WHERE MarkName IN (");
                    markNames[0] = markNames[0].Replace("MarkNames=", "");
                    foreach (var markName in markNames)
                    {
                        inString.Append($"'{markName}',");
                    }
                    if (inString.Length > 0) inString.Length -= 1; //Strip off trailing comma.
                    inString.Append(")");
                }
                var cmd = GetHighWaterMarks(inString.ToString());
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var mark = new HighWaterMark();
                    mark.PlantCode = plant.PlantCode;
                    mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                    mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                    mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                    mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                    mark.LookbackSeconds = CastDbNull.To(reader["LookbackSeconds"], 0);
                    mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                    mark.EnterpriseTimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.LocalTimeSpan = (DateTime.Now - (mark.LocalDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.InitStatement = CastDbNull.To(reader["InitStatement"], string.Empty).Trim();
                    mark.Exists = true;
                    marks.Add(mark);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            return marks;
        }

        private static SqlCommand GetHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT
                    MarkId,
                    MarkName ,
                    EnterpriseDateTime ,
                    LocalDateTime ,
                    LookbackSeconds ,
                    UniqueID ,
                    WrittenDate
                FROM 
                    EBS.HighWaterMark
                WHERE 
                    MarkName = @MarkName "
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public static HighWaterMark GetHighWaterMark(SqlConnection cn, Plant plant, string markName)
        {
            var mark = new HighWaterMark(markName, plant.PlantCode);
            try
            {
                var cmd = GetHighWaterMark();
                cmd.Parameters["@MarkName"].Value = markName;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                    mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                    mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                    mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                    mark.LookbackSeconds = CastDbNull.To(reader["LookbackSeconds"], 0);
                    mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                    mark.EnterpriseTimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.LocalTimeSpan = (DateTime.Now - (mark.LocalDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.Exists = true;
                }
                reader.Close();
                if (!mark.Exists)
                {
                    //throw new Exception($"High Water Mark [{markName}] does NOT exist at Plant {plant.PlantCode}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            return mark;
        }

        public static HighWaterMark GetHighWaterMark(Plant plant, string markName)
        {
            var mark = new HighWaterMark(markName, plant.PlantCode);
            var cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
                cn.Open();
                var cmd = GetHighWaterMark();
                cmd.Parameters["@MarkName"].Value = markName;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    mark.HighWaterMarkID = CastDbNull.To(reader["MarkId"], 0);
                    mark.HighWaterMarkName = CastDbNull.To(reader["MarkName"], string.Empty).Trim();
                    mark.EnterpriseDateTime = CastDbNull.To(reader["EnterpriseDateTime"], DateTime.MinValue);
                    mark.LocalDateTime = CastDbNull.To(reader["LocalDateTime"], DateTime.MinValue);
                    mark.LookbackSeconds = CastDbNull.To(reader["LookbackSeconds"], 0);
                    mark.UniqueID = CastDbNull.To<long>(reader["UniqueID"], 0);
                    mark.EnterpriseTimeSpan = (DateTime.Now - (mark.EnterpriseDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.LocalTimeSpan = (DateTime.Now - (mark.LocalDateTime.AddSeconds(-CastDbNull.To(reader["LookbackSeconds"], 0))));
                    mark.Exists = true;
                }
                reader.Close();
                if (!mark.Exists)
                {
                    throw new Exception($"High Water Mark [{markName}] does NOT exist");
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            finally
            {
                cn.Close();
            }
            return mark;
        }

        private static SqlCommand UpdateHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                UPDATE 
                    EBS.HighWaterMark
                SET
                    EnterpriseDateTime = @EnterpriseDateTime,
                    LocalDateTime = @LocalDateTime,
                    WrittenDate = GETDATE(),
                    UniqueID = @UniqueID                        
                WHERE 
                    MarkName = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.BigInt));
            return cmd;
        }

        private static SqlCommand InsertHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                INSERT INTO EBS.HighWaterMark (
                    MarkName,
                    EnterpriseDateTime,
                    LocalDateTime,
                    LookbackSeconds,
                    UniqueID)
			    VALUES ( 
                    @MarkName,
					@EnterpriseDateTime,
					@LocalDateTime,
                    120,
                    @UniqueID)"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.BigInt));
            return cmd;
        }

        public static SqlDmlResponse SaveHighWaterMark(SqlConnection cn, HighWaterMark mark)
        {
            var response = new SqlDmlResponse();
            try
            {
                var plant = PlantManager.GetPlant(mark.PlantCode);
                mark.EnterpriseDateTime = mark.EnterpriseDateTime > DateTime.Now ? DateTime.Now : mark.EnterpriseDateTime;
                mark.LocalDateTime = mark.EnterpriseDateTime.AddHours(plant.TimeOffset);
                var cmd = (mark.HighWaterMarkID > 0) ? UpdateHighWaterMark() : InsertHighWaterMark();
                cmd.Parameters["@MarkName"].Value = mark.HighWaterMarkName;
                cmd.Parameters["@EnterpriseDateTime"].Value = mark.EnterpriseDateTime;
                cmd.Parameters["@LocalDateTime"].Value = mark.LocalDateTime;
                cmd.Parameters["@UniqueID"].Value = mark.UniqueID;
                cmd.Connection = cn;
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error($"{cn.ConnectionString}  {e.Message}"));
                Logger.Error($@"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($"{cn.ConnectionString}  {e.Message}");
            }
            return response;
        }
    }
}
