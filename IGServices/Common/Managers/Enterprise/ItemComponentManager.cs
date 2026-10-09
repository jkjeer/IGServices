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
    public class ItemComponentManager
    {
        public ItemComponentManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("ItemComponentManager.GetItemComponents", GetItemComponents());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public iDB2Command GetItemComponents()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
            @"SELECT 
	                IXITM,
                    IMDSC1,
                    IXQNTY,
                    IXUM,
                    IXUOM,
                    IXOPSQ,
                    IBGLPT          
                FROM 
	                F3002
                    INNER JOIN F4102
                    ON  IBITM = IXITM AND
                        IBMCU = IXMMCU
                    INNER JOIN F4101
                    ON  IMITM = IXITM
                WHERE 
                    IXMMCU = ? AND
                    IXKIT = ? AND
                    IXEFFF <= ? AND
                    IXEFFT >= ?
                ORDER BY IXOPSQ", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("StartDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["StartDateId"].Precision = 6;
            cmd.Parameters["StartDateId"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("EndDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EndDateId"].Precision = 6;
            cmd.Parameters["EndDateId"].Scale = 0;
            return cmd;
        }

        public ItemComponents GetItemComponents(BusinessSystemSession e1Session, String plantNum, Decimal itemNum)
        {
            var items = new ItemComponents();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetItemComponents")) ? e1Session.DbCommands["GetItemComponents"] : GetItemComponents();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Parameters[2].Value = Utilities.Common.CastToE1Date(System.DateTime.Now);
                cmd.Parameters[3].Value = Utilities.Common.CastToE1Date(System.DateTime.Now);
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var item = new ItemComponent();
                    item.Item = GetTypedItem(e1Session, plantNum, CastDbNull.To<Decimal>(reader["IXITM"], 0), CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim());
                    //item.ItemNum = CastDbNull.To<Decimal>(reader["IXITM"], 0);
                    //item.ItemDesc = CastDbNull.To<String>(reader["IMDSC1"], String.Empty).Trim();
                    //item.ItemCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                    item.Quantity = CastDbNull.To<Decimal>(reader["IXQNTY"], 0) / 1000;
                    item.UnitMeasure = CastDbNull.To<String>(reader["IXUM"], String.Empty).Trim();
                    item.UnitMeasureInput = CastDbNull.To<String>(reader["IXUOM"], String.Empty).Trim();
                    item.OperationSeqNum = CastDbNull.To<Decimal>(reader["IXOPSQ"], 0) / 100;
                    
                    items.Add(item);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return items;
        }

        /*********************************************************************************
        * Strongly Typed Item
        *******************************************************************************/
        public Item GetTypedItem(BusinessSystemSession e1Session, String plantNum, Decimal itemNum, String glCategory)
        {
            try
            {
                switch (glCategory)
                {
                    case "GLAS":
                    case "GLAZ":
                    case "GLAX":
                    case "GLAF":
                        var _glassItemMgr = new GlassItemManager(e1Session);
                        var glassItem = _glassItemMgr.GetGlassItem(e1Session, itemNum, plantNum);
                        return glassItem;
                    case "CHAN":
                        var _spacerItemMgr = new SpacerItemManager(e1Session);
                        //var spacerItem = _spacerItemMgr.GetSpacerItems(e1Session, plantNum, itemNum);
                        var spacerItem = _spacerItemMgr.GetSpacerItem(e1Session, plantNum, itemNum);
                        return spacerItem;
                    default:
                        var _itemMgr = new ItemManager(e1Session);
                        var baseItem = _itemMgr.GetItem(e1Session, plantNum, itemNum);
                        return baseItem;
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }
    }
}
