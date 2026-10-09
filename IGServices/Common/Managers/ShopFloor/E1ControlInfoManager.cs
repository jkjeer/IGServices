using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.ShopFloor
{
    public class E1ControlInfoManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static E1ControlInfos GetDuplicateSFShortDescriptions(SqlConnection cnSF)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var e1ControlInfos = new E1ControlInfos();
            try
            {
                var cmd = GetDuplicateSFShortDescriptions();
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var e1ControlInfo = new E1ControlInfo
                    {
                        AppCode = CastDbNull.To(reader["AppCode"], string.Empty).Trim(),
                        SysCode = CastDbNull.To(reader["SysCode"], string.Empty).Trim(),
                        ShortDescription = CastDbNull.To(reader["ShortDescription"], string.Empty).Trim(),
                    };
                    e1ControlInfos.Add(e1ControlInfo);
                }
                reader.Close();
                Logger.Trace($"ROWS: {e1ControlInfos.Count}");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return e1ControlInfos;
        }

        private static SqlCommand GetDuplicateSFShortDescriptions()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT  TOP (3)
		                     [AppCode]
                           , [SysCode]
                           , UPPER([ShortDescription]) AS ShortDescription
                           , COUNT(*) AS freq
                    FROM     [dbo].[E1ControlInfo]
                    WHERE    [AppCode] = '57'
                             AND [SysCode] IN ( 'TH', 'CL', 'TM', 'C1', 'CT', 'PT', 'LM' )
                             AND [ShortDescription] > ''
                    GROUP BY [AppCode]
                           , [SysCode]
                           , UPPER([ShortDescription])
                    --HAVING   COUNT(*) > 1 "
            };
            return cmd;
        }

        public static E1ControlInfos GetSFE1ControlInfoByShortDescription(SqlConnection cnSF, E1ControlInfos duplicateSFShortDescriptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var e1ControlInfos = new E1ControlInfos();
            try
            {
                foreach (var d in duplicateSFShortDescriptions)
                {
                    var filters = new List<string>
                    {
                        $@"[AppCode] = '{d.AppCode}' AND ",
                        $@"[SysCode] = '{d.SysCode}' AND ",
                        $@"UPPER([ShortDescription]) = '{d.ShortDescription}' "
                    };
                    var cmd = GetSFE1ControlInfoByShortDescription(filters);
                    cmd.Connection = cnSF;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        var e1ControlInfo = new E1ControlInfo
                        {
                            AppCode = CastDbNull.To(reader["AppCode"], string.Empty).Trim(),
                            SysCode = CastDbNull.To(reader["SysCode"], string.Empty).Trim(),
                            //UserCodeType = CastDbNull.To(reader["UserCodeType"], string.Empty).Trim(),
                            KeyValue = CastDbNull.To(reader["KeyValue"], string.Empty).Trim(),
                            DescriptionOne = CastDbNull.To(reader["DescriptionOne"], string.Empty).Trim(),
                            DescriptionTwo = CastDbNull.To(reader["DescriptionTwo"], string.Empty).Trim(),
                            ShortDescription = CastDbNull.To(reader["ShortDescription"], string.Empty).Trim(),
                            WrittenDate = CastDbNull.To(reader["WrittenDate"], new DateTime(1900, 1, 1)),
                            WrittenBy = CastDbNull.To(reader["WrittenBy"], "")
                        };
                        e1ControlInfos.Add(e1ControlInfo);
                    }
                    reader.Close();
                    Logger.Trace($"ROWS: {e1ControlInfos.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return e1ControlInfos;
        }

        private static SqlCommand GetSFE1ControlInfoByShortDescription(IEnumerable<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT
                        [DRSY],
                        [DRRT],
		                [DTDL01],
                        [DRKY],
		                [DRDL01],
		                [DRDL02],
                        [DRSPHD],
                        [DRUPMJ],
                        [DRUPMT]
                    FROM
                        [PSBI_WH_PROD].[PROD].[PS_JWS_F0005] 
	                WHERE 
		                {filterString} "
            };
            return cmd;
        }
    }
}
