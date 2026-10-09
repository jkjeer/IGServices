using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpacerItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public SpacerItemTable()
        {
            //ExtendedProperties.Add("Target", "[ITM].[SPACER_ITEM_LEDGER]");
            //Columns.Add("ITEM_NUM", typeof(decimal));
            //Columns.Add("DESCRIPTION", typeof(string));
            //Columns.Add("CONSTRUCTION", typeof(string));
            //Columns.Add("STOCK_TYPE_CD", typeof(string));
            //Columns.Add("SPACER_TYPE_CD", typeof(string));
            //Columns.Add("MATERIAL", typeof(string));
            //Columns.Add("PROFILE", typeof(string));
            //Columns.Add("COLOR", typeof(string));
            //Columns.Add("PAINTED", typeof(string));
            //Columns.Add("GAUGE", typeof(string));
            //Columns.Add("PRE_KEYED", typeof(string));
            //Columns.Add("WIDTH", typeof(decimal));
            //Columns.Add("LENGTH", typeof(decimal));
            //Columns.Add("PRIMARY_LOCATION_CD", typeof(string));
            //Columns.Add("PRIMARY_UOM_CD", typeof(string));
            //Columns.Add("QTY_ON_HAND", typeof(decimal));
            //Columns.Add("COST_PER_LINEAL_FOOT", typeof(decimal));
            //Columns.Add("UPDATE_DATE", typeof(DateTime));

            ExtendedProperties.Add("Target", "[EBS].[SpacerItemLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Description", typeof(string));
            Columns.Add("Construction", typeof(string));
            Columns.Add("StockTypeCode", typeof(string));
            Columns.Add("SpacerTypeCode", typeof(string));
            Columns.Add("Material", typeof(string));
            Columns.Add("Profile", typeof(string));
            Columns.Add("Color", typeof(string));
            Columns.Add("Painted", typeof(string));
            Columns.Add("Gauge", typeof(string));
            Columns.Add("PreKeyed", typeof(string));
            Columns.Add("Width", typeof(decimal));
            Columns.Add("Length", typeof(decimal));
            Columns.Add("PrimaryLocationCode", typeof(string));
            Columns.Add("PrimaryUomCode", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("CostPerLinealFoot", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Enterprise.SpacerItems items)
        {
            try
            {
                foreach (var item in items)
                {
                    DataRow dr = this.NewRow();
                    dr["ItemNum"] = item.ItemNum;
                    dr["Description"] = item.Description;
                    dr["Construction"] = item.Construction;
                    dr["StockTypeCode"] = item.StockingType;
                    if (item.SpacerType != null)
                    {
                        dr["SpacerTypeCode"] = item.SpacerType.Name;
                        dr["Material"] = item.SpacerType.Material;
                        dr["Color"] = item.SpacerType.Color;
                        dr["Painted"] = item.SpacerType.Painted;
                    }
                    dr["Profile"] = item.Profile;
                    dr["Gauge"] = item.Gauge;
                    dr["PreKeyed"] = item.PreKeyed;
                    dr["Width"] = item.Width;
                    dr["Length"] = item.Length;
                    dr["PrimaryUomCode"] = item.PrimaryUom;
                    dr["PrimaryLocationCode"] = item.PrimaryLocation;
                    dr["CostPerLinealFoot"] = item.CostPerLinealFoot;
                    dr["QtyOnHand"] = item.QtyOnHand;
                    dr["TransactionDate"] = item.UpdateDate;

                    //dr["ITEM_NUM"] = item.ItemNum;
                    //dr["DESCRIPTION"] = item.Description;
                    //dr["CONSTRUCTION"] = item.Construction;
                    //dr["STOCK_TYPE_CD"] = item.StockingType;
                    //if (item.SpacerType != null)
                    //{
                    //    dr["SPACER_TYPE_CD"] = item.SpacerType.Name;
                    //    dr["MATERIAL"] = item.SpacerType.Material;
                    //    dr["COLOR"] = item.SpacerType.Color;
                    //    dr["PAINTED"] = item.SpacerType.Painted;
                    //}
                    //dr["PROFILE"] = item.Profile;
                    //dr["GAUGE"] = item.Gauge;
                    //dr["PRE_KEYED"] = item.PreKeyed;
                    //dr["WIDTH"] = item.Width;
                    //dr["LENGTH"] = item.Length;
                    //dr["PRIMARY_UOM_CD"] = item.PrimaryUom;
                    //dr["PRIMARY_LOCATION_CD"] = item.PrimaryLocation;
                    //dr["COST_PER_LINEAL_FOOT"] = item.CostPerLinealFoot;
                    //dr["QTY_ON_HAND"] = item.QtyOnHand;
                    //dr["UPDATE_DATE"] = item.UpdateDate;

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
