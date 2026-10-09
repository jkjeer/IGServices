using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class BarItemManager
    {
        public BarItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"BarItemManager.GetBarItem", GetBarItem()},
                {"BarItemManager.GetBarTypeFromC", GetBarTypeFromC()},
                {"BarItemManager.GetBarTypeFromUdc", GetBarTypeFromUdc()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public iDB2Command GetBarItem()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    IBMCU,
                        IBITM,
                        IBGLPT,
                        IBSTKT,
                        IBSRP4,
                        IMUOM1,
                        IMDSC1,
                        IMDSC2               
                    FROM 
	                    F4102
                        INNER JOIN F4101
                        ON IMITM = IBITM
                    WHERE 
                        IBGLPT = 'MUNT' AND
                        IBMCU = ? AND
                        IBITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public BarItem GetBarItem(BusinessSystemSession e1Session, string plantNum, decimal itemNum)
        {
            var item = new BarItem();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("BarItemManager.GetBarItem")) ? e1Session.DbCommands["BarItemManager.GetBarItem"] : GetBarItem();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    item.ItemNum = CastDbNull.To<decimal>(reader["IBITM"], 0);
                    item.StockingType = CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                    item.GLCategory = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                    item.Description = CastDbNull.To(reader["IMDSC1"], string.Empty).Trim();
                    item.Construction = CastDbNull.To(reader["IMDSC2"], string.Empty).Trim();
                    item.PrimaryUom = CastDbNull.To(reader["IMUOM1"], string.Empty).Trim();
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return item;
        }

        public iDB2Command GetBarTypeFromC()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    RDBSEQ,
                        RDTVAL               
                    FROM 
	                    F3283
                    WHERE 
                        RDRTBT = 'C' AND
                        RDTBLC = 'IGMCODEMST' AND
                        RDTSV1 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCode", iDB2DbType.iDB2Graphic, 40));
            return cmd;
        }

        public iDB2Command GetBarTypeFromUdc()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
	                    DRDL01,
                        DRDL02,
                        DRSPHD              
                    FROM 
	                    {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                    WHERE 
                        DRSY = '57' AND
                        DRRT = 'BR' AND
                        DRSPHD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCode", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        public BarItem GetBarAttributes(BusinessSystemSession e1Session, string mCode, string orientation)
        {
            var barItem = new BarItem();
            try
            {
                barItem.MCode = mCode;
                barItem.Orientation = orientation;
                var dict1 = GetBarTypeProperties(e1Session, mCode);
                barItem.Type = dict1[1];
                barItem.Thickness = Convert.ToDecimal(dict1[2]);
                barItem.Width = Convert.ToDecimal(dict1[3]);
                var dict2 = GetBarTypeUdcProperties(e1Session, mCode);
                barItem.Description = dict2["DL01"];
                barItem.Color = dict2["DL02"];
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return barItem;
        }

        private Dictionary<decimal, string> GetBarTypeProperties(BusinessSystemSession e1Session, string mCode)
        {
            var ret = new Dictionary<decimal, string>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("BarItemManager.GetBarTypeFromC")) ? e1Session.DbCommands["BarItemManager.GetBarTypeFromC"] : GetBarTypeFromC();
                cmd.Parameters[0].Value = mCode;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    ret.Add(CastDbNull.To<decimal>(reader["RDBSEQ"], 0), CastDbNull.To(reader["RDTVAL"], string.Empty).Trim());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return ret;
        }

        private Dictionary<string, string> GetBarTypeUdcProperties(BusinessSystemSession e1Session, string mCode)
        {
            var ret = new Dictionary<string, string>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("BarItemManager.GetBarTypeFromUdc")) ? e1Session.DbCommands["BarItemManager.GetBarTypeFromUdc"] : GetBarTypeFromUdc();
                cmd.Parameters[0].Value = mCode;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    ret.Add("DL01", CastDbNull.To(reader["DRDL01"], string.Empty).Trim());
                    ret.Add("DL02", CastDbNull.To(reader["DRDL02"], string.Empty).Trim());
                    ret.Add("SPHD", CastDbNull.To(reader["DRSPHD"], string.Empty).Trim());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return ret;
        }

    }
}
