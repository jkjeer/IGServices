using System;
using System.Data;
using System.Linq;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class EmployeeTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
            set { }
        }

        public EmployeeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[EmployeeLedger]");
            Columns.Add("AddressNum", typeof(decimal));
            Columns.Add("AlternateNum", typeof(string));
            Columns.Add("Name", typeof(string));
            Columns.Add("HomeCompany", typeof(string));
            Columns.Add("HomeCompanyDesc", typeof(string));
            Columns.Add("Company", typeof(string));
            Columns.Add("HomeCostCenter", typeof(string));
            Columns.Add("CostCenter", typeof(string));
            Columns.Add("UnionCode", typeof(string));
            Columns.Add("PayStatus", typeof(string));
            Columns.Add("UpdateDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["AddressNum"] = CastDbNull.To<decimal>(reader["YAAN8"], 0);
                    dr["AlternateNum"] = CastDbNull.To(reader["YAOEMP"], string.Empty).Trim();
                    dr["Name"] = CastDbNull.To(reader["YAALPH"], string.Empty).Trim();
                    dr["HomeCompany"] = CastDbNull.To(reader["YAHMCO"], string.Empty).Trim();
                    dr["HomeCompanyDesc"] = CastDbNull.To(reader["MCDL01"], string.Empty).Trim();
                    dr["Company"] = "";
                    dr["HomeCostCenter"] = CastDbNull.To(reader["YAHMCU"], string.Empty).Trim();
                    dr["CostCenter"] = CastDbNull.To(reader["YAMCU"], string.Empty).Trim();
                    dr["UnionCode"] = CastDbNull.To(reader["YAUN"], string.Empty).Trim();
                    dr["PayStatus"] = CastDbNull.To(reader["YAPAST"], string.Empty).Trim();
                    var updateDateId = CastDbNull.To<decimal>(reader["YAUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["YADTSP"], 0);
                    dr["UpdateDate"] = Utilities.Common.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}
