using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ScheduleLineTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public ScheduleLineTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[SCHEDULE_LINE_LEDGER]");
            Columns.Add("SCHEDULE_NUM", typeof(decimal));
            Columns.Add("SCHEDULE_DATE", typeof(DateTime));
            Columns.Add("WORK_ORDER_NUM", typeof(decimal));
            Columns.Add("TRANSACTION_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["SCHEDULE_NUM"] = CastDbNull.To<decimal>(reader["S5Z58SCN"], 0);
                    dr["SCHEDULE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["S5Z58SCD"], 0));
                    dr["SCHEDULE_NUM"] = CastDbNull.To<decimal>(reader["S5DOCO"], 0);
                    dr["TRANSACTION_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["S5UPMJ"], 0), Data.CastDbNull.To<decimal>(reader["S5TDAY"], 0));
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
