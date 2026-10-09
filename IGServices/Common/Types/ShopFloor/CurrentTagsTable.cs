using System;
using System.Data;
using System.Globalization;
using System.Linq;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class CurrentTagsTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TagDate")).Max(); }
            set { }
        }

        public CurrentTagsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[CurrentTagsLedger]");
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            //Columns.Add("PlantID", typeof(string));
            Columns.Add("TagDate", typeof(DateTime));
            Columns.Add("DownLoadTime", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = NewRow();
                    dr["SerialNum"] = CastDbNull.To(reader["TILOTN"], string.Empty).Trim();
                    dr["WOrderNum"] = CastDbNull.To<decimal>(reader["TIDOCO"], 0).ToString(CultureInfo.InvariantCulture);
                    dr["OrderType"] = CastDbNull.To(reader["TIDCTO"], string.Empty).Trim();
                    //dr["PlantID"] = CastDbNull.To(reader["TIMCU"], string.Empty).Trim();
                    var updateDateId = CastDbNull.To<decimal>(reader["TIUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["TITDAY"], 0);
                    dr["TagDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId);
                    dr["DownLoadTime"] = DateTime.Now;
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
