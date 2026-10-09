using System;
using System.Data;
using System.Data.SqlClient;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class HighWaterMarkManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                    ShopFloor.EBS.HighWaterMark
                WHERE 
                    MarkName = @MarkName "
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public static HighWaterMark GetHighWaterMark(SqlConnection cn, Plant plant, string markName)
        {
            var mark = new HighWaterMark(markName, plant.PlantNum);
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
                    throw new Exception($"High Water Mark [{markName}] does NOT exist at Plant {plant.PlantNum} {plant.PlantCode}");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return mark;
        }


        public static HighWaterMark GetHighWaterMark(Plant plant, string markName)
        {
            var mark = new HighWaterMark(markName, plant.PlantNum);
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
                throw new Exception(e.Message);
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
                    ShopFloor.EBS.HighWaterMark
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
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.Int));
            return cmd;
        }

        private static SqlCommand InsertHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                INSERT INTO ShopFloor.EBS.HighWaterMark (
                    MarkName ,
                    EnterpriseDateTime ,
                    LocalDateTime ,
                    UniqueID)
			    VALUES ( 
                    @MarkName,
					@EnterpriseDateTime,
					@LocalDateTime,
                    @UniqueID)"
            };
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueID", SqlDbType.Int));
            return cmd;
        }

        public static SqlDmlResponse SaveHighWaterMark(SqlConnection cn, HighWaterMark mark)
        {
            var response = new SqlDmlResponse();
            try
            {
                var plant = PlantManager.GetPlant(mark.PlantNum);
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
                response.Errors.Add(new Error(e));
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

    }
}
