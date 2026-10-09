using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ShippableRackTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public ShippableRackTable()
        {
            ExtendedProperties.Add("Target", "[INV].[SHIPPABLE_RACK_LEDGER]");
            Columns.Add("PLANT_NO", typeof(string));
            Columns.Add("RACK_ID", typeof(decimal));
            Columns.Add("SCHEDULED_RACK_ID", typeof(decimal));
            Columns.Add("RACK_TYPE", typeof(string));
            Columns.Add("SCHEDULE_NO", typeof(decimal));
            Columns.Add("BOX_NO", typeof(decimal));
            Columns.Add("RACK_HEADER_STATUS_CD", typeof(string));
            Columns.Add("SCHEDULED_RACK_HEADER_STATUS_CD", typeof(string));
            Columns.Add("RACK_DETAIL_STATUS_CD", typeof(string));
            Columns.Add("WORK_ORDER_NO", typeof(decimal));
            Columns.Add("WORK_ORDER_STATUS_CD", typeof(string));
            Columns.Add("LOT_NO", typeof(string));
            Columns.Add("SERIAL_NO", typeof(string));
            Columns.Add("UNITS", typeof(int));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["PLANT_NO"] = CastDbNull.To<string>(reader["RHMCU"], String.Empty).Trim();
                    dr["RACK_ID"] = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                    dr["SCHEDULED_RACK_ID"] = CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                    dr["RACK_TYPE"] = CastDbNull.To<string>(reader["RHZ58RT"], String.Empty).Trim();
                    dr["SCHEDULE_NO"] = CastDbNull.To<decimal>(reader["RHZ58SCN"], 0);
                    dr["BOX_NO"] = CastDbNull.To<decimal>(reader["RHZ58BOX"], 0);
                    dr["RACK_HEADER_STATUS_CD"] = CastDbNull.To<string>(reader["RHZ58HST"], String.Empty).Trim();
                    dr["SCHEDULED_RACK_HEADER_STATUS_CD"] = CastDbNull.To<string>(reader["RHZ58HST_SCHED"], String.Empty).Trim();
                    dr["RACK_DETAIL_STATUS_CD"] = CastDbNull.To<string>(reader["RDZ58DST"], String.Empty).Trim();
                    dr["WORK_ORDER_NO"] = CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                    dr["WORK_ORDER_STATUS_CD"] = CastDbNull.To<string>(reader["WASRST"], String.Empty).Trim();
                    dr["LOT_NO"] = CastDbNull.To<string>(reader["RDLOTN"], String.Empty).Trim();
                    dr["SERIAL_NO"] = CastDbNull.To<string>(reader["RDZ58SN"], String.Empty).Trim();
                    dr["UNITS"] = CastDbNull.To<int>(reader["UNITS"], 0);
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
