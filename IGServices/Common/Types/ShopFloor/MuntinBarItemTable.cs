using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IGServices.Common.Types.Enterprise;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class MuntinBarItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public MuntinBarItemTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[MUNTIN_BAR_ITEM_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(int));
            Columns.Add("DESCRIPTION", typeof(string));
            Columns.Add("STOCK_TYPE_CD", typeof(string));
            Columns.Add("MCODE1", typeof(string));
            Columns.Add("MCODE2", typeof(string));
            Columns.Add("BAR_TYPE_CD", typeof(string));
            Columns.Add("WIDTH", typeof(decimal));
            Columns.Add("THICKNESS", typeof(decimal));
            Columns.Add("IS_TWO_TONE", typeof(bool));
            Columns.Add("PRIMARY_LOCATION_CD", typeof(string));
            Columns.Add("PRIMARY_UOM_CD", typeof(string));
            Columns.Add("QTY_ON_HAND", typeof(decimal));
            Columns.Add("COST_PER_LINEAL_FOOT", typeof(decimal));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(MuntinBarItems barItems)
        {
            try
            {
                foreach (MuntinBarItem item in barItems)
                {
                    DataRow dr = this.NewRow();
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["DESCRIPTION"] = item.Description;
                    dr["STOCK_TYPE_CD"] = item.StockingType;
                    if (item.MuntinBarCodes.Any())
                    {
                        dr["MCODE1"] = item.MuntinBarCodes[0].MCode;
                        dr["BAR_TYPE_CD"] = item.MuntinBarCodes[0].Type;
                        dr["WIDTH"] = item.MuntinBarCodes[0].Width;
                        dr["THICKNESS"] = item.MuntinBarCodes[0].Thickness;
                        dr["IS_TWO_TONE"] = item.MuntinBarCodes[0].IsTwoTone;
                    }
                    dr["MCODE2"] = (item.MuntinBarCodes.Count > 1) ? item.MuntinBarCodes[1].MCode : "";
                    dr["PRIMARY_LOCATION_CD"] = item.PrimaryLocation;
                    dr["PRIMARY_UOM_CD"] = item.PrimaryUom;
                    dr["QTY_ON_HAND"] = item.QtyOnHand;
                    dr["COST_PER_LINEAL_FOOT"] = item.CostPerLinealFoot;
                    dr["UPDATE_DATE"] = item.UpdateDate;
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
