using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchaseLineTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }
        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("ORDER_LINE_UKID")).Max(); }
            set { }
        }

        public PurchaseLineTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[PURCHASE_ORDER_EVENT_LEDGER]");
            Columns.Add("ORDER_NUM", typeof(decimal));
            Columns.Add("ORDER_TYPE_CD", typeof(string));
            Columns.Add("ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("ORDER_LINE_UKID", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("LOT_NUM", typeof(string));
            Columns.Add("QUANTITY_ORDERED", typeof(decimal));
            Columns.Add("QUANTITY_RECEIVED", typeof(decimal));
            Columns.Add("ORDER_DATE", typeof(DateTime));
            Columns.Add("REQUESTED_DATE", typeof(DateTime));
            Columns.Add("RECEIVED_DATE", typeof(DateTime));
            Columns.Add("CANCEL_DATE", typeof(DateTime));
            Columns.Add("SUPPLIER_NUM", typeof(decimal));
            Columns.Add("STOCK_CD", typeof(string));
            Columns.Add("LINE_TYPE_CD", typeof(string));
            Columns.Add("GL_CAT_CD", typeof(string));
            Columns.Add("STATUS_LAST_CD", typeof(string));
            Columns.Add("STATUS_NEXT_CD", typeof(string));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["ORDER_NUM"] = CastDbNull.To<decimal>(reader["OLDOCO"], 0);
                    dr["ORDER_TYPE_CD"] = CastDbNull.To<string>(reader["OLDCTO"], String.Empty).Trim();
                    dr["ORDER_LINE_NUM"] = CastDbNull.To<decimal>(reader["OLLNID"], 0) / 1000;
                    dr["ORDER_LINE_UKID"] = CastDbNull.To<decimal>(reader["OLUKID"], 0);
                    dr["CONFIGURATION_ID"] = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["ITEM_NUM"] = CastDbNull.To<decimal>(reader["OLITM"], 0);
                    dr["LOT_NUM"] = CastDbNull.To<string>(reader["OLLOTN"], String.Empty).Trim();
                    dr["QUANTITY_ORDERED"] = CastDbNull.To<decimal>(reader["OLUORG"], 0) / 1000;
                    dr["QUANTITY_RECEIVED"] = CastDbNull.To<decimal>(reader["OLUREC"], 0) / 1000;
                    dr["ORDER_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLTRDJ"], 0));
                    dr["REQUESTED_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLDRQJ"], 0));
                    dr["RECEIVED_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLADDJ"], 0));
                    dr["CANCEL_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLCNDJ"], 0));
                    dr["SUPPLIER_NUM"] = CastDbNull.To<decimal>(reader["OLAN8"], 0);
                    dr["STOCK_CD"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    dr["LINE_TYPE_CD"] = CastDbNull.To<string>(reader["OLLNTY"], String.Empty).Trim();
                    dr["GL_CAT_CD"] = CastDbNull.To<string>(reader["OLGLC"], String.Empty).Trim();
                    dr["STATUS_LAST_CD"] = CastDbNull.To<string>(reader["OLLTTR"], String.Empty).Trim();
                    dr["STATUS_NEXT_CD"] = CastDbNull.To<string>(reader["OLNXTR"], String.Empty).Trim();
                    dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["OLUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["OLTDAY"], 0));
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
