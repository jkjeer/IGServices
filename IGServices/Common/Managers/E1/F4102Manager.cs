using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.E1
{
    public class F4102Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F4102Manager()
        {
        }

        public F4102Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F4102Manager.GetF4102", GetF4102()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
     * Prepared Commands
     ********************************************************************************/
        public iDB2Command GetF4102()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        IBANPL,
                        ABALPH,
                        IBGLPT,
                        IBUPMJ,
                        IBTDAY
                    FROM
                        F4102
				        INNER JOIN F0101 ON IBANPL = ABAN8 
                    WHERE
                        IBMCU >= ? AND
                        IBITM >= ? 
                    "
            };
            cmd.Parameters.Add(new iDB2Parameter("IBMCU", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("CMDOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CMDOCO"].Precision = 8;
            cmd.Parameters["CMDOCO"].Scale = 0;
            return cmd;
        }

        public F4102 GetF4102(BusinessSystemSession e1Session, decimal workOrderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var f4102 = new F4102();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("F4102Manager.GetF4102")) ? e1Session.DbCommands["F4102Manager.GetF4102"] : GetF4102();
                cmd.Parameters["CMDOCO"].Value = workOrderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var i = 0;
                while (reader.Read())
                {
                    var f = new F4102
                    {
                        //CMCFGID = CastDbNull.To<decimal>(reader["CMCFGID"], 0),
                        //CMKCOO = CastDbNull.To(reader["CMKCOO"], string.Empty).Trim(),
                        //CMDOCO = CastDbNull.To<decimal>(reader["CMDOCO"], 0),
                        //CMDCTO = CastDbNull.To(reader["CMDCTO"], string.Empty).Trim(),
                        //CMSFXO = CastDbNull.To(reader["CMSFXO"], string.Empty).Trim(),
                        //CMLNID = CastDbNull.To<decimal>(reader["CMLNID"], 0),
                        //CMCFGCID = CastDbNull.To<decimal>(reader["CMCFGCID"], 0),
                        //CMSY = CastDbNull.To(reader["CMSY"], string.Empty).Trim(),
                        //CMAN8 = CastDbNull.To<decimal>(reader["CMAN8"], 0),
                        //CMSHAN = CastDbNull.To<decimal>(reader["CMSHAN"], 0),
                        //CMEMCU = CastDbNull.To(reader["CMEMCU"], string.Empty).Trim(),
                        //CMCO = CastDbNull.To(reader["CMCO"], string.Empty).Trim(),
                        //CMCRRM = CastDbNull.To(reader["CMCRRM"], string.Empty).Trim(),
                        //CMCRCD = CastDbNull.To(reader["CMCRCD"], string.Empty).Trim(),
                        //CMCRDC = CastDbNull.To(reader["CMCRDC"], string.Empty).Trim(),
                        //CMURCD = CastDbNull.To(reader["CMURCD"], string.Empty).Trim(),
                        //CMURDT = CastDbNull.To<decimal>(reader["CMURDT"], 0),
                        //CMURAT = CastDbNull.To<decimal>(reader["CMURAT"], 0),
                        //CMURAB = CastDbNull.To<decimal>(reader["CMURAB"], 0),
                        //CMURRF = CastDbNull.To(reader["CMURRF"], string.Empty).Trim(),
                        //CMUSER = CastDbNull.To(reader["CMUSER"], string.Empty).Trim(),
                        //CMPID = CastDbNull.To(reader["CMPID"], string.Empty).Trim(),
                        //CMJOBN = CastDbNull.To(reader["CMJOBN"], string.Empty).Trim(),
                        //CMUPMJ = CastDbNull.To<decimal>(reader["CMUPMJ"], 0),
                        //CMTDAY = CastDbNull.To<decimal>(reader["CMTDAY"], 0)
                    };
                    f4102 = f;
                    i++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + i);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return f4102;
        }
    }
}
