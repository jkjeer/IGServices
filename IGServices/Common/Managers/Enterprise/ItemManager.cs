using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class ItemManager
    {
        //private GlassItemManager _glassItemMgr;
        //private SpacerItemManager _spacerItemMgr;

        public ItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("ItemManager.GetItem", GetItem());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            //_glassItemMgr = new GlassItemManager(e1Session);
            //_spacerItemMgr = new SpacerItemManager(e1Session);
        }

        /*********************************************************************************
        * Base Item
        *******************************************************************************/
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
                        ON IMITM = IBITM
                    WHERE 
                        IBMCU = ? AND
                        IBITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public Item GetItem(BusinessSystemSession e1Session, string plantNum, decimal itemNum)
        {
            var item = new Item();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetItem")) ? e1Session.DbCommands["GetItem"] : GetItem();
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
                    //item.UnitType = CastDbNull.To<String>(reader["IBSRP4"], String.Empty).Trim();
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return item;
        }

        /*********************************************************************************
        * Strongly Typed Item
        *******************************************************************************/
        public Item GetTypedItem(BusinessSystemSession e1Session, string plantNum, decimal itemNum, string glCategory)
        {
            try
            {
                var plantCode = PlantManager.GetPlant(plantNum).PlantCode;
                switch (glCategory)
                {
                    case "GLAS":
                    case "GLAZ":
                    case "GLAX":
                    case "GLAF":
                        var _glassItemMgr = new GlassItemManager(e1Session);
                        var glassItem = _glassItemMgr.GetGlassItem(e1Session, itemNum, plantCode);
                        return glassItem;
                    case "CHAN":
                        var _spacerItemMgr = new SpacerItemManager(e1Session);
                        var spacerItem = _spacerItemMgr.GetSpacerItem(e1Session, plantNum, itemNum);
                        return spacerItem;
                    default:
                        var baseItem = GetItem(e1Session, plantNum, itemNum);
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
