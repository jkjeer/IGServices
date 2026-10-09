using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class MuntinBarCodeManager
    {
        public MuntinBarCodeManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("MuntinBarCodeManager.GetBarCode", GetBarCode());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /**********************************************************************************
        *   Get Plant specific M-Codes
        **********************************************************************************/
        public iDB2Command GetBarCode()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
              TRIM(DRKY) AS MCODE,
              TRIM(DRDL01) AS DESCRIPTION,
              TRIM(DRDL02) AS UDC_COLOR,
              COALESCE(TRIM(C1.VAL), '') AS TYPE_CD,
              COALESCE(TRIM(C2.VAL), '0') AS THICKNESS,
              COALESCE(TRIM(C3.VAL), '0') AS WIDTH,
              COALESCE(TRIM(C4.VAL), '') AS EXT_COLOR,
              COALESCE(TRIM(C5.VAL), '') AS INT_COLOR,
              DRUPMJ,
              DRUPMT
            FROM
              {0}.F0005
              LEFT OUTER JOIN ( SELECT
                                  RIGHT('00000' || TRIM(RDTSV1), 5) AS MCODE ,
                                  RDTVAL AS VAL
                                FROM
                                  F3283
                                WHERE
                                  RDRTBT = 'C' AND
                                  RDMCU = '' AND
                                  RDTBLC = 'IGMCODEMST' AND
                                  RDBSEQ = 1
                              ) C1
              ON C1.MCODE = TRIM(DRKY)
              LEFT OUTER JOIN ( SELECT
                                  RIGHT('00000' || TRIM(RDTSV1), 5) AS MCODE ,
                                  RDTVAL AS VAL
                                FROM
                                  F3283
                                WHERE
                                  RDRTBT = 'C' AND
                                  RDMCU = '' AND
                                  RDTBLC = 'IGMCODEMST' AND
                                  RDBSEQ = 2
                              ) C2
              ON C2.MCODE = TRIM(DRKY)
              LEFT OUTER JOIN ( SELECT
                                  RIGHT('00000' || TRIM(RDTSV1), 5) AS MCODE ,
                                  RDTVAL AS VAL
                                FROM
                                  F3283
                                WHERE
                                  RDRTBT = 'C' AND
                                  RDMCU = '' AND
                                  RDTBLC = 'IGMCODEMST' AND
                                  RDBSEQ = 3
                              ) C3
              ON C3.MCODE = TRIM(DRKY)
              LEFT OUTER JOIN ( SELECT
                                  RIGHT('00000' || TRIM(RDTSV1), 5) AS MCODE ,
                                  RDTVAL AS VAL
                                FROM
                                  F3283
                                WHERE
                                  RDRTBT = 'C' AND
                                  RDMCU = '' AND
                                  RDTBLC = 'IGMCODEMST' AND
                                  RDBSEQ = 4
                              ) C4
              ON C4.MCODE = TRIM(DRKY)
              LEFT OUTER JOIN ( SELECT
                                  RIGHT('00000' || TRIM(RDTSV1), 5) AS MCODE ,
                                  RDTVAL AS VAL
                                FROM
                                  F3283
                                WHERE
                                  RDRTBT = 'C' AND
                                  RDMCU = '' AND
                                  RDTBLC = 'IGMCODEMST' AND
                                  RDBSEQ = 5
                              ) C5
              ON C5.MCODE = TRIM(DRKY)
            WHERE
              DRSY = '57' AND
              DRRT = 'BR' AND
              TRIM(DRKY) = ?
            ORDER BY 1", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("DRKY", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        public MuntinBarCode GetBarCode(BusinessSystemSession e1Session, string mCode)
        {
            var bar = new MuntinBarCode();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("MuntinBarCodeManager.GetBarCode"))
                          ? e1Session.DbCommands["MuntinBarCodeManager.GetBarCode"]
                          : GetBarCode();
                cmd.Parameters[0].Value = mCode.PadLeft(5, '0');
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    bar.MCode = CastDbNull.To<String>(reader["MCODE"], String.Empty).Trim();
                    bar.MCodeNum = Convert.ToInt16(bar.MCode);
                    bar.Description = CastDbNull.To<String>(reader["DESCRIPTION"], String.Empty).Trim();
                    var color = CastDbNull.To<String>(reader["UDC_COLOR"], String.Empty).Trim();
                    bar.ColorDesc = color;
                    var colors = color.Split('/');
                    if (colors.Length > 1)
                    {
                        bar.ExtColorDesc = colors[0];
                        bar.IntColorDesc = colors[1];
                        bar.IsTwoTone = true;
                    }
                    else
                    {
                        bar.ExtColorDesc = color;
                        bar.IntColorDesc = color;
                        bar.IsTwoTone = false;
                    }
                    bar.Type = CastDbNull.To<String>(reader["TYPE_CD"], String.Empty).Trim();
                    bar.Thickness = Convert.ToDecimal(CastDbNull.To<String>(reader["THICKNESS"], String.Empty).Trim());
                    bar.Width = Convert.ToDecimal(CastDbNull.To<String>(reader["WIDTH"], String.Empty).Trim());
                    bar.ExtColor= CastDbNull.To<String>(reader["EXT_COLOR"], String.Empty).Trim();
                    bar.IntColor = CastDbNull.To<String>(reader["INT_COLOR"], String.Empty).Trim();
                    var updateDateId = CastDbNull.To<Decimal>(reader["DRUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<Decimal>(reader["DRUPMT"], 0);
                    bar.UpdateDate = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return bar;
        }
    }
}
