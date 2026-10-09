using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class DiscreteGlassItemManager
    {
        private readonly GlassItemManager _glassItemManager;

        public DiscreteGlassItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();

            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemManager = new GlassItemManager(e1Session);
        }

        /*********************************************************************************
        * Prepared Commands
        ********************************************************************************/
        public iDB2Command GetItem()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMDSC1,
                IMDSC2               
            FROM 
                F4102
                INNER JOIN F4101
                ON  IMITM = IBITM AND
                    IBMCU = ?
            WHERE 
                IBGLPT IN ('GLAF','GLAS','GLAX','GLAZ') AND
                IBSTKT != 'C' AND
                IBMCU = ? AND
                IBITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetItemsByPlant()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMDSC1,
                IMDSC2               
            FROM 
                F4102
                INNER JOIN F4101
                ON  IMITM = IBITM AND
                    IBMCU = ?
            WHERE   
                IBGLPT IN ('GLAF','GLAS','GLAX','GLAZ') AND              
                IBSTKT != 'C' AND
                IBMCU = ?";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public GlassItems GetDiscreteGlassItems(BusinessSystemSession e1Session, string plantCode, string categoryCode = null, string stockingType = null, decimal itemNum = 0, TimeSpan? timeSpan = null)
        {
            var items = new GlassItems();
            try
            {
                var cmd = new iDB2Command();
                var ets = new E1TimeSpan();
                if (timeSpan != null && timeSpan.Value.TotalDays > 0)
                {
                    ets.SetTimeSpan(timeSpan.Value);
                }
                else
                {
                    timeSpan = null;
                }

                if (itemNum > 0)
                {
                    cmd = (e1Session.DbCommands.ContainsKey("DiscreteGlassItemManager.GetItem")) ? e1Session.DbCommands["DiscreteGlassItemManager.GetItem"] : GetItem();
                    cmd.Parameters[0].Value = plantCode;
                }
                else
                {
                    cmd = (e1Session.DbCommands.ContainsKey("DiscreteGlassItemManager.GetItemsByPlant")) ? e1Session.DbCommands["DiscreteGlassItemManager.GetItemsByPlant"] : GetItemsByPlant();
                    cmd.Parameters[0].Value = plantCode;
                    cmd.Parameters[1].Value = itemNum;    
                }

                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var itemVal = CastDbNull.To<Decimal>(reader["IBITM"], 0);
                    var item = _glassItemManager.GetGlassItem(e1Session, itemVal, plantCode);
                    items.Add(item);
                }
                reader.Close();
                return items;
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }
    }
}
