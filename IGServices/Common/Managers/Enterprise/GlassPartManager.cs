using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class GlassPartManager
    {

        public GlassPartManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("GlassPartManager.GetGlassPart", GetGlassPart());
            hash.Add("GlassPartManager.GetWorkOrderGlassParts", GetWorkOrderGlassParts());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public iDB2Command GetGlassPart()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
            @"SELECT 
                COALESCE(IBMCU, '00') AS IBMCU,
                COALESCE(IBITM, IMITM) AS IBITM,
                COALESCE(IBGLPT, IMGLPT) AS IBGLPT,
                COALESCE(IBSTKT, IMSTKT) AS IBSTKT,
                COALESCE(IBSRP4, IMSRP4) AS IBSRP4,
                COALESCE(IBANPL, 0) AS IBANPL,
                IMDSC1,
                IMDSC2               
            FROM 
                F4101
                LEFT OUTER JOIN F4102
                ON  IMITM = IBITM AND
                    IBMCU = ?
            WHERE 
                IMITM = ?", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public GlassPart GetGlassPart(BusinessSystemSession session, Decimal itemNum, Plant plant)
        {
            var item = new GlassPart();
            try
            {
                var plantNum = plant.PlantNum;
                iDB2Command cmd = (session.DbCommands.ContainsKey("GlassPartManager.GetGlassPart"))
                                      ? session.DbCommands["GlassPartManager.GetGlassPart"]
                                      : GetGlassPart();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    item.PlantNum = plantNum;
                    item.ItemNum = Data.CastDbNull.To<Decimal>(reader["IBITM"], 0);
                    item.StockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                    item.GLCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                    item.Description = CastDbNull.To<String>(reader["IMDSC1"], String.Empty).Trim();
                    item.Construction = CastDbNull.To<String>(reader["IMDSC2"], String.Empty).Trim();
                    item.UnitType = CastDbNull.To<String>(reader["IBSRP4"], String.Empty).Trim();
                    var customerNum = CastDbNull.To<Decimal>(reader["IBANPL"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            return item;
        }

        public GlassPart GetGlassPart(BusinessSystemSession session, ScheduleLine line)
        {
            var item = new GlassPart();
            try
            {
                iDB2Command cmd = (session.DbCommands.ContainsKey("GlassPartManager.GetGlassPart"))
                    ? session.DbCommands["GlassPartManager.GetGlassPart"]
                    : GetGlassPart();
                cmd.Parameters[0].Value = line.F580021.S7MCU.PadLeft(12);
                cmd.Parameters[1].Value = line.F580021.S7ITM;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    item.ItemNum = Data.CastDbNull.To<Decimal>(reader["IBITM"], 0);
                    item.StockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                    item.GLCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                    item.Description = CastDbNull.To<String>(reader["IMDSC1"], String.Empty).Trim();
                    item.Construction = CastDbNull.To<String>(reader["IMDSC2"], String.Empty).Trim();
                    item.UnitType = CastDbNull.To<String>(reader["IBSRP4"], String.Empty).Trim();
                    var customerNum = CastDbNull.To<Decimal>(reader["IBANPL"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            return item;
        }


        public iDB2Command GetWorkOrderGlassParts()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT * FROM (
                SELECT
                    WMDOCO,
                    WMOPSQ,
                    WMCPIT,
                    WMDSC1,
                    WMDSC2,
                    WMLOCN,
                    WMUORG,
                    WMTRQT,
                    WMUM,
                    WMCPNB,
                    WMMCU,
	                WMUSER,   
                    IBITM,
                    IBGLPT,
                    IBSTKT,
                    IBSRP4,
                    IBANPL,
	                ROW_NUMBER() OVER (PARTITION BY WMOPSQ ORDER BY WMCPNB) AS RN
                FROM
                    F3111
                    INNER JOIN F4102 
                    ON  IBITM = WMCPIT AND
                        IBMCU = WMMCU
                WHERE
                    WMDOCO = ? AND
                    IBGLPT IN ('GLAS','GLAF','GLAC','GLAX','GLAZ')) A
                WHERE A.RN = 1
                ORDER BY WMOPSQ"
            };
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            return cmd;
        }

        public GlassParts GetWorkOrderGlassParts(BusinessSystemSession session, decimal workOrderNum)
        {
            var parts = new GlassParts();
            try
            {
                iDB2Command cmd = session.DbCommands["GlassPartManager.GetWorkOrderGlassParts"];
                cmd.Parameters[0].Value = workOrderNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var part = new GlassPart();
                    part.ItemNum = Data.CastDbNull.To<Decimal>(reader["IBITM"], 0);
                    part.StockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                    part.GLCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                    part.UnitType = CastDbNull.To<String>(reader["IBSRP4"], String.Empty).Trim();
                    parts.Add(part);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            return parts;
        }
    }
}
