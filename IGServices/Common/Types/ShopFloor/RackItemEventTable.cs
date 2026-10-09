using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemEventTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("RACK_ITEM_UKID")).Max(); }
            set { }
        }

        public RackItemEventTable()
        {
            ExtendedProperties.Add("Target", "[INV].[RACK_ITEM_EVENT_LEDGER]");
            Columns.Add("RACK_ITEM_UKID", typeof(decimal));
            Columns.Add("RACK_ID", typeof(decimal));
            Columns.Add("SERIAL_NO", typeof(string));
            Columns.Add("ITEM_NO", typeof(decimal));
            Columns.Add("LOT_NO", typeof(string));
            Columns.Add("STATUS_CD", typeof(string));
            Columns.Add("ON_HAND_QTY", typeof(decimal));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["RACK_ITEM_UKID"] = CastDbNull.To<decimal>(reader["DLUKID"], 0);
                    dr["RACK_ID"] = CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                    dr["SERIAL_NO"] = CastDbNull.To<string>(reader["DLZ58SN"], String.Empty).Trim();
                    dr["ITEM_NO"] = CastDbNull.To<decimal>(reader["DLITM"], 0);
                    dr["LOT_NO"] = CastDbNull.To<string>(reader["DLLOTN"], String.Empty).Trim();
                    dr["STATUS_CD"] = CastDbNull.To<string>(reader["DLZ58DST"], String.Empty).Trim();
                    dr["ON_HAND_QTY"] = CastDbNull.To<decimal>(reader["DLPQOH"], 0) / 1000;
                    dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["DLUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["DLTDAY"], 0));
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
