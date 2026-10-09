using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.ShopFloor
{
    public class UdcItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static UdcItems GetDuplicateSFEBSHandlingCodes(SqlConnection cnDW, F0005s duplicateDWF0005s)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var f0005s = new F0005s();
            try
            {
                foreach (var d in duplicateDWF0005s)
                {
                    var cmd = GetDwDRSPHD();
                    cmd.Parameters["DRSY"].Value = d.DRSY;
                    cmd.Parameters["DRRT"].Value = d.DRRT;
                    cmd.Parameters["DRSPHD"].Value = d.DRSPHD;
                    cmd.Connection = cnDW;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        var f0005 = new F0005
                        {
                            DRSY = CastDbNull.To(reader["DRSY"], string.Empty).Trim(),
                            DRRT = CastDbNull.To(reader["DRRT"], string.Empty).Trim(),
                            DTDL01 = CastDbNull.To(reader["DTDL01"], string.Empty).Trim(),
                            DRKY = CastDbNull.To(reader["DRKY"], string.Empty).Trim(),
                            DRDL01 = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                            DRDL02 = CastDbNull.To(reader["DRDL02"], string.Empty).Trim(),
                            DRSPHD = CastDbNull.To(reader["DRSPHD"], string.Empty).Trim(),
                            DRUPMJ = CastDbNull.To<decimal>(reader["DRUPMJ"], 0),
                            DRUPMT = CastDbNull.To<decimal>(reader["DRUPMT"], 0)
                        };
                        f0005s.Add(f0005);
                    }
                    reader.Close();
                    Logger.Trace($"ROWS: {f0005s.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return f0005s;
        }

        public static F0005s GetSFEBSUdcItemsByHandlingCode(SqlConnection cnDW, F0005s duplicateDWF0005s)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var f0005s = new F0005s();
            try
            {
                foreach (var d in duplicateDWF0005s)
                {
                    var cmd = GetDwDRSPHD();
                    cmd.Parameters["DRSY"].Value = d.DRSY;
                    cmd.Parameters["DRRT"].Value = d.DRRT;
                    cmd.Parameters["DRSPHD"].Value = d.DRSPHD;
                    cmd.Connection = cnDW;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        var f0005 = new F0005
                        {
                            DRSY = CastDbNull.To(reader["DRSY"], string.Empty).Trim(),
                            DRRT = CastDbNull.To(reader["DRRT"], string.Empty).Trim(),
                            DTDL01 = CastDbNull.To(reader["DTDL01"], string.Empty).Trim(),
                            DRKY = CastDbNull.To(reader["DRKY"], string.Empty).Trim(),
                            DRDL01 = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                            DRDL02 = CastDbNull.To(reader["DRDL02"], string.Empty).Trim(),
                            DRSPHD = CastDbNull.To(reader["DRSPHD"], string.Empty).Trim(),
                            DRUPMJ = CastDbNull.To<decimal>(reader["DRUPMJ"], 0),
                            DRUPMT = CastDbNull.To<decimal>(reader["DRUPMT"], 0)
                        };
                        f0005s.Add(f0005);
                    }
                    reader.Close();
                    Logger.Trace($"ROWS: {f0005s.Count}");
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnDW.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return f0005s;
        }

    }
}
