using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class WorkOrderEventTable : DataTable
    {
        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("ORDER_UKID")).Max(); }
            set { }
        }

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public WorkOrderEventTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[WORK_ORDER_EVENT_LEDGER]");
            Columns.Add("ORDER_UKID", typeof(decimal));
            Columns.Add("ORDER_NUM", typeof(decimal));
            Columns.Add("ORDER_TYPE_CD", typeof(string));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("LOT_NUM", typeof(string));
            Columns.Add("QUANTITY", typeof(decimal));
            Columns.Add("ORDER_DATE", typeof(DateTime));
            Columns.Add("REQUESTED_DATE", typeof(DateTime));
            Columns.Add("CUSTOMER_SOLD_NUM", typeof(decimal));
            Columns.Add("STOCK_CD", typeof(string));
            Columns.Add("STATUS_CD", typeof(string));
            Columns.Add("RELATED_ORDER_NUM", typeof(decimal));
            Columns.Add("RELATED_ORDER_TYPE_CD", typeof(string));
            Columns.Add("RELATED_ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["ORDER_UKID"] = Data.CastDbNull.To<decimal>(reader["WAUKID"], 0);
                    dr["ORDER_NUM"] = Data.CastDbNull.To<decimal>(reader["WADOCO"], 0);
                    dr["ORDER_TYPE_CD"] = Data.CastDbNull.To<string>(reader["WADCTO"], String.Empty).Trim();
                    dr["LOT_NUM"] = Data.CastDbNull.To<string>(reader["WALOTN"], String.Empty).Trim();
                    dr["STATUS_CD"] = Data.CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim();
                    dr["CUSTOMER_SOLD_NUM"] = Data.CastDbNull.To<decimal>(reader["WAAN8"], 0);
                    dr["ITEM_NUM"] = Data.CastDbNull.To<decimal>(reader["WAITM"], 0);
                    dr["CONFIGURATION_ID"] = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["STOCK_CD"] = Data.CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    dr["ORDER_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WATRDJ"], 0));
                    dr["REQUESTED_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WADRQJ"], 0));
                    dr["QUANTITY"] = Data.CastDbNull.To<decimal>(reader["WAUORG"], 0) / 1000;
                    dr["RELATED_ORDER_NUM"] = Data.ParseStringToDecimal(Data.CastDbNull.To<string>(reader["WARORN"], String.Empty).Trim());
                    dr["RELATED_ORDER_TYPE_CD"] = Data.CastDbNull.To<string>(reader["WARCTO"], String.Empty).Trim();
                    dr["RELATED_ORDER_LINE_NUM"] = Data.CastDbNull.To<decimal>(reader["WALNID"], 0) / 1000;
                    dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["WAMUPM"], 0),
                        Data.CastDbNull.To<decimal>(reader["WAUPMT"], 0));
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
