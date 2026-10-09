using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackEventTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("RACK_UKID")).Max(); }
            set { }
        }

        public RackEventTable()
        {
            ExtendedProperties.Add("Target", "[INV].[RACK_EVENT_LEDGER]");
            Columns.Add("RACK_UKID", typeof(decimal));
            Columns.Add("RACK_ID", typeof(decimal));
            Columns.Add("RACK_TYPE_CD", typeof(string));
            Columns.Add("STATUS_CD", typeof(string));
            Columns.Add("LOCATION", typeof(string));
            Columns.Add("REASON_CD", typeof(string));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["RACK_UKID"] = CastDbNull.To<decimal>(reader["HLUKID"], 0);
                    dr["RACK_ID"] = CastDbNull.To<decimal>(reader["HLZ58RID"], 0);
                    dr["RACK_TYPE_CD"] = CastDbNull.To<string>(reader["HLZ58RT"], String.Empty).Trim();
                    dr["STATUS_CD"] = CastDbNull.To<string>(reader["HLZ58HST"], String.Empty).Trim();
                    dr["LOCATION"] = CastDbNull.To<string>(reader["HLLOCN"], String.Empty).Trim();
                    dr["REASON_CD"] = CastDbNull.To<string>(reader["HLURCD"], String.Empty).Trim();
                    dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["HLUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["HLTDAY"], 0));
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
