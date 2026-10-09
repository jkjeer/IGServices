using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class GlassItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public GlassItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[GlassItemLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("UnitType", typeof(string));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("StockType", typeof(string));
            Columns.Add("GlCategory", typeof(string));
            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("CustomerPartNum", typeof(string));
            Columns.Add("SupplierNum", typeof(decimal));
            Columns.Add("SupplierName", typeof(string));
            Columns.Add("PrimaryLocation", typeof(string));
            Columns.Add("PrimaryLocationFull", typeof(string));
            Columns.Add("PrimaryUom", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("CostPerEa", typeof(decimal));
            Columns.Add("LbsPerEa", typeof(decimal));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        //public void Add(IDataReader reader)
        //{
        //    try
        //    {
        //        while (reader.Read())
        //        {
        //            DataRow dr = this.NewRow();
        //            dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
        //            dr["UnitType"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
        //            dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
        //            dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
        //            dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
        //            dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
        //            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
        //            dr["CustomerName"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
        //            dr["SupplierNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
        //            dr["SupplierName"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();
        //            dr["CustomerPartNum"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
        //            dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
        //            dr["PrimaryLocationFull"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty);
        //            dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
        //            dr["LbsPerEa"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
        //            dr["QtyOnHand"] = (decimal)0;
        //            dr["CostPerEa"] = (decimal)0;
        //            dr["TransactionId"] = (decimal)0;
        //            dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IBUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["IBTDAY"], 0));
        //            this.Rows.Add(dr);
        //        }
        //        reader.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

    }
}
