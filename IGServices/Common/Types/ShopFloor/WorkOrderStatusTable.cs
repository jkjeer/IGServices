using System;
using System.Data;
using System.Linq;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class WorkOrderStatusTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TRANSACTION_DATE")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("UNIQUE_ID")).Max(); }
            set { }
        }

        public WorkOrderStatusTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[WORK_ORDER_STATUS_LEDGER]");
            Columns.Add("UNIQUE_ID", typeof(decimal));
            Columns.Add("WORK_ORDER_NO", typeof(decimal));
            Columns.Add("STATUS_CD", typeof(string));
            Columns.Add("TRANSACTION_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["UNIQUE_ID"] = Data.CastDbNull.To<decimal>(reader["WAUKID"], 0);
                    dr["WORK_ORDER_NO"] = CastDbNull.To<decimal>(reader["WADOCO"], 0);
                    dr["STATUS_CD"] = CastDbNull.To(reader["WASRST"], string.Empty);
                    var updateDateId = CastDbNull.To<decimal>(reader["WAMUPM"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["WAUPMT"], 0);
                    dr["TRANSACTION_DATE"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    Rows.Add(dr);
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
