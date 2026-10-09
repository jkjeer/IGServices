using System;
using System.Data;
using System.Linq;
using IGGServices.Common.Types.Enterprise;
using IGServices.Common.Managers.E1;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ScheduleTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public ScheduleTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ScheduleLedger]");
            Columns.Add("ScheduleId", typeof(int));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("ScheduleType", typeof(string));
            Columns.Add("Lines", typeof(int));
            Columns.Add("Units", typeof(int));
            Columns.Add("BatchNum", typeof(string));
            Columns.Add("SystemCode", typeof(string));
            Columns.Add("SystemName", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                DataRow dr = this.NewRow();
                dr["ScheduleId"] = (int)schedule.ScheduleId;
                dr["ScheduleNum"] = schedule.f580020.S6Z58SCN;
                dr["ScheduleDate"] = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(schedule.f580020.S6Z58SCD, 0));
                dr["ScheduleType"] = schedule.f5834A01.S1Z58SCTYP;
                dr["Lines"] = schedule.ScheduleLines.Count;
                dr["Units"] = schedule.ScheduleLines.Sum(z => z.F580021.S7Z58QTY);
                dr["BatchNum"] = schedule.f580020.S6EDBT;
                dr["SystemCode"] = schedule.f580020.S6Z58SFS;
                dr["SystemName"] = schedule.f580020.S6Z58SFS_NAME;
                dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(schedule.f5834A01.S1UPMJ, 0),
                    Data.CastDbNull.To<decimal>(schedule.f5834A01.S1TDAY, 0));
                this.Rows.Add(dr);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }

}
