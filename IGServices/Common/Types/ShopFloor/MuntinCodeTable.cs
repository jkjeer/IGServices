using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class MuntinCodeTable : DataTable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public MuntinCodeTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[MuntinBarCodeLedger]");
            Columns.Add("MCodeNum", typeof(int));
            Columns.Add("MCode", typeof(string));
            Columns.Add("Description", typeof(string));
            Columns.Add("Type", typeof(string));
            Columns.Add("Width", typeof(decimal));
            Columns.Add("Thickness", typeof(decimal));
            Columns.Add("ExtColorCode", typeof(string));
            Columns.Add("ExtColorDesc", typeof(string));
            Columns.Add("IntColorCode", typeof(string));
            Columns.Add("IntColorDesc", typeof(string));
            Columns.Add("IsTwoTone", typeof(bool));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["MCode"] = CastDbNull.To<string>(reader["DRKY"], String.Empty).Trim();
                    dr["MCodeNum"] = CastDbNull.To<int>(reader["DRKY_INT"], 0);
                    dr["Description"] = CastDbNull.To<string>(reader["DRDL01"], String.Empty).Trim();
                    dr["Type"] = CastDbNull.To<string>(reader["RDTVAL1"], String.Empty).Trim();
                    dr["Width"] = Convert.ToDecimal(CastDbNull.To<string>(reader["RDTVAL3"], String.Empty).Trim());
                    dr["Thickness"] = Convert.ToDecimal(CastDbNull.To<string>(reader["RDTVAL2"], String.Empty).Trim());
                    dr["ExtColorCode"] = CastDbNull.To<string>(reader["RDTVAL4"], String.Empty).Trim();
                    dr["IntColorCode"] = CastDbNull.To<string>(reader["RDTVAL5"], String.Empty).Trim();
                    var udcColor = CastDbNull.To<String>(reader["DRDL02"], String.Empty).Trim();
                    var colors = udcColor.Split('/');
                    if (colors.Length > 1)
                    {
                        dr["ExtColorDesc"] = colors[0];
                        dr["IntColorDesc"] = colors[1];
                    }
                    else
                    {
                        dr["ExtColorDesc"] = udcColor;
                        dr["IntColorDesc"] = udcColor;
                    }
                    dr["IsTwoTone"] = ((string)dr["ExtColorCode"] != (string)dr["IntColorCode"]);
                    dr["ItemNum"] = Data.CastDbNull.To<decimal>(reader["RDITM"], 0);
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["DRUPMJ"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["DRUPMT"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
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
