using System;
using System.Data;
using System.Linq;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ItemLedger]");
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("Description1", typeof(string));
            Columns.Add("Description2", typeof(string));
            Columns.Add("StockType", typeof(string));
            Columns.Add("GlCategory", typeof(string));
            Columns.Add("PrimaryLocation", typeof(string));
            Columns.Add("PrimaryUom", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("CostPerEa", typeof(decimal));
            Columns.Add("LbsPerEa", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["IBITM"], 0);
                    dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                    dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                    dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
                    dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
                    dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                    dr["LbsPerEa"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IBUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
