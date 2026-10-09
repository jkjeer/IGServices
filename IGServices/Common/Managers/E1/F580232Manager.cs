using System;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class F580232Manager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static iDB2Command GetF580232()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
                        BDMCU,
                        BDZ58JOBID,
                        BDZ58BNUM,
                        BDZ58WODC,
                        BDZ58WOTY,
                        BDDOCO,
                        BDDCTO,
                        BDKCOO,
                        BDLNID,
                        BDZ58SFS,
                        BDZ58IGLN,
                        BDZ58FLEX1,
                        BDZ58FLEX2,
                        BDZ58FLEX3,
                        BDZ58FLEX4,
                        BDUSER,
                        BDPID,
                        BDJOBN,
                        BDUPMJ,
                        BDUPMT,
                        BDSEQ,
                        BDZ58BGEE,
                        BDZ58BGSZ,
                        BDZ58BOXP,
                        BDZ58CCV01,
                        BDZ58CCV02,
                        BDZ58CCV03,
                        BDZ58CCV04,
                        BDZ58CCV05,
                        BDZ58CCV06,
                        BDZ58CCV07,
                        BDZ58CCV08,
                        BDZ58CCV09,
                        BDZ58CCV10,
                        BDZ58CCV11,
                        BDZ58CCV12,
                        BDZ58CCV13,
                        BDZ58CCV14,
                        BDZ58CCV15,
                        BDDRQJ,
                        BDZ58CRIT,
                        BHDS01,
                        BHZ58GDES,
                        BHZ58BDES,
						SHDEL1,
						SHDEL2
                    FROM
                        F580232
                        INNER JOIN F580230
                        ON  BHMCU = BDMCU AND
                            BHZ58JOBID = BDZ58JOBID AND
                            BHZ58BNUM = BDZ58BNUM
						LEFT OUTER JOIN F4201
						ON SHDOCO = BDDOCO AND
							SHDCTO = BDDCTO AND
							SHMCU = BDMCU
                    WHERE 
                        BDMCU = ? AND
                        EXISTS (SELECT 1 from F5834A11 WHERE S5MCU = BDMCU AND S5DOCO = BDZ58WODC AND S5Z58SCN = ?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Z58SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58SCN"].Precision = 5;
            cmd.Parameters["Z58SCN"].Scale = 0;
            return cmd;
        }

        public static F580232s GetF580232(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum)
        {
            var f580232s = new F580232s();
            try
            {
                var cmd = GetF580232();
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["Z58SCN"].Value = scheduleNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    var f580232 = new F580232();
                    f580232.BDMCU = CastDbNull.To<string>(reader["BDMCU"], String.Empty).Trim();
                    f580232.BDZ58JOBID = CastDbNull.To<decimal>(reader["BDZ58JOBID"], 0);
                    f580232.BDZ58BNUM = CastDbNull.To<decimal>(reader["BDZ58BNUM"], 0);
                    f580232.BDZ58WODC = CastDbNull.To<decimal>(reader["BDZ58WODC"], 0);
                    f580232.BDZ58WOTY = CastDbNull.To<string>(reader["BDZ58WOTY"], String.Empty).Trim();
                    f580232.BDDOCO = CastDbNull.To<decimal>(reader["BDDOCO"], 0);
                    f580232.BDDCTO = CastDbNull.To<string>(reader["BDDCTO"], String.Empty).Trim();
                    f580232.BDKCOO = CastDbNull.To<string>(reader["BDKCOO"], String.Empty).Trim();
                    f580232.BDLNID = CastDbNull.To<decimal>(reader["BDLNID"], 0);
                    f580232.BDZ58SFS = CastDbNull.To<string>(reader["BDZ58SFS"], String.Empty).Trim();
                    f580232.BDZ58IGLN = CastDbNull.To<string>(reader["BDZ58IGLN"], String.Empty).Trim();
                    f580232.BDZ58FLEX1 = CastDbNull.To<string>(reader["BDZ58FLEX1"], String.Empty).Trim();
                    f580232.BDZ58FLEX2 = CastDbNull.To<string>(reader["BDZ58FLEX2"], String.Empty).Trim();
                    f580232.BDZ58FLEX3 = CastDbNull.To<string>(reader["BDZ58FLEX3"], String.Empty).Trim();
                    f580232.BDZ58FLEX4 = CastDbNull.To<string>(reader["BDZ58FLEX4"], String.Empty).Trim();
                    f580232.BDUSER = CastDbNull.To<string>(reader["BDUSER"], String.Empty).Trim();
                    f580232.BDPID = CastDbNull.To<string>(reader["BDPID"], String.Empty).Trim();
                    f580232.BDJOBN = CastDbNull.To<string>(reader["BDJOBN"], String.Empty).Trim();
                    f580232.BDUPMJ = CastDbNull.To<decimal>(reader["BDUPMJ"], 0);
                    f580232.BDUPMT = CastDbNull.To<decimal>(reader["BDUPMT"], 0);
                    f580232.BDSEQ = CastDbNull.To<decimal>(reader["BDSEQ"], 0);
                    f580232.BDZ58BGEE = CastDbNull.To<string>(reader["BDZ58BGEE"], String.Empty).Trim();
                    f580232.BDZ58BGSZ = CastDbNull.To<decimal>(reader["BDZ58BGSZ"], 0);
                    f580232.BDZ58BOXP = CastDbNull.To<string>(reader["BDZ58BOXP"], String.Empty).Trim();
                    f580232.BDZ58CCV01 = CastDbNull.To<string>(reader["BDZ58CCV01"], String.Empty).Trim();
                    f580232.BDZ58CCV02 = CastDbNull.To<string>(reader["BDZ58CCV02"], String.Empty).Trim();
                    f580232.BDZ58CCV03 = CastDbNull.To<string>(reader["BDZ58CCV03"], String.Empty).Trim();
                    f580232.BDZ58CCV04 = CastDbNull.To<string>(reader["BDZ58CCV04"], String.Empty).Trim();
                    f580232.BDZ58CCV05 = CastDbNull.To<string>(reader["BDZ58CCV05"], String.Empty).Trim();
                    f580232.BDZ58CCV06 = CastDbNull.To<string>(reader["BDZ58CCV06"], String.Empty).Trim();
                    f580232.BDZ58CCV07 = CastDbNull.To<string>(reader["BDZ58CCV07"], String.Empty).Trim();
                    f580232.BDZ58CCV08 = CastDbNull.To<string>(reader["BDZ58CCV08"], String.Empty).Trim();
                    f580232.BDZ58CCV09 = CastDbNull.To<string>(reader["BDZ58CCV09"], String.Empty).Trim();
                    f580232.BDZ58CCV10 = CastDbNull.To<string>(reader["BDZ58CCV10"], String.Empty).Trim();
                    f580232.BDZ58CCV11 = CastDbNull.To<string>(reader["BDZ58CCV11"], String.Empty).Trim();
                    f580232.BDZ58CCV12 = CastDbNull.To<string>(reader["BDZ58CCV12"], String.Empty).Trim();
                    f580232.BDZ58CCV13 = CastDbNull.To<string>(reader["BDZ58CCV13"], String.Empty).Trim();
                    f580232.BDZ58CCV14 = CastDbNull.To<string>(reader["BDZ58CCV14"], String.Empty).Trim();
                    f580232.BDZ58CCV15 = CastDbNull.To<string>(reader["BDZ58CCV15"], String.Empty).Trim();
                    f580232.BDDRQJ = CastDbNull.To<decimal>(reader["BDDRQJ"], 0);
                    f580232.BDZ58CRIT = CastDbNull.To<decimal>(reader["BDZ58CRIT"], 0);
                    f580232.BHDS01 = CastDbNull.To<string>(reader["BHDS01"], String.Empty).Trim();
                    f580232.BHZ58GDES = CastDbNull.To<string>(reader["BHZ58GDES"], String.Empty).Trim();
                    f580232.BHZ58BDES = CastDbNull.To<string>(reader["BHZ58BDES"], String.Empty).Trim();

                    f580232.SHDEL1 = CastDbNull.To<string>(reader["SHDEL1"], String.Empty).Trim();
                    f580232.SHDEL2 = CastDbNull.To<string>(reader["SHDEL2"], String.Empty).Trim();

                    f580232s.Add(f580232);
                }

                reader.Close();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            return f580232s;
        }
    }
}
