using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class MuntinBarCodeTable : DataTable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public MuntinBarCodeTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[MUNTIN_BAR_CODE_LEDGER]");
            Columns.Add("MCODE_NUM", typeof(int));
            Columns.Add("MCODE", typeof(string));
            Columns.Add("DESCRIPTION", typeof(string));
            Columns.Add("BAR_TYPE_CD", typeof(string));
            Columns.Add("WIDTH", typeof(decimal));
            Columns.Add("THICKNESS", typeof(decimal));
            Columns.Add("EXT_COLOR_CD", typeof(string));
            Columns.Add("EXT_COLOR_DESC", typeof(string));
            Columns.Add("INT_COLOR_CD", typeof(string));
            Columns.Add("INT_COLOR_DESC", typeof(string));
            Columns.Add("IS_TWO_TONE", typeof(bool));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(List<MuntinBarCode> codes)
        {
            try
            {
                foreach (var code in codes)
                {
                    DataRow dr = this.NewRow();
                    dr["MCODE"] = code.MCode;
                    dr["MCODE_NUM"] = code.MCodeNum;
                    dr["DESCRIPTION"] = code.Description;
                    dr["BAR_TYPE_CD"] = code.Type;
                    dr["EXT_COLOR_CD"] = code.ExtColor;
                    dr["EXT_COLOR_DESC"] = code.ExtColorDesc;
                    dr["INT_COLOR_CD"] = code.IntColor;
                    dr["INT_COLOR_DESC"] = code.IntColorDesc;
                    dr["IS_TWO_TONE"] = code.IsTwoTone;
                    dr["THICKNESS"] = code.Thickness;
                    dr["WIDTH"] = code.Width;
                    dr["UPDATE_DATE"] = code.UpdateDate;
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(MuntinBarItems barItems)
        {
            try
            {
                var codes = barItems.SelectMany(x => x.MuntinBarCodes).GroupBy(x => x.MCodeNum).Select(x=>x.First());
                foreach (MuntinBarCode code in codes)
                {
                    DataRow dr = this.NewRow();
                    dr["MCODE"] = code.MCode;
                    dr["MCODE_NUM"] = code.MCodeNum;
                    dr["DESCRIPTION"] = code.Description;
                    dr["BAR_TYPE_CD"] = code.Type;
                    dr["EXT_COLOR_CD"] = code.ExtColor;
                    dr["EXT_COLOR_DESC"] = code.ExtColorDesc;
                    dr["INT_COLOR_CD"] = code.IntColor;
                    dr["INT_COLOR_DESC"] = code.IntColorDesc;
                    dr["IS_TWO_TONE"] = code.IsTwoTone;
                    dr["THICKNESS"] = code.Thickness;
                    dr["WIDTH"] = code.Width;
                    dr["UPDATE_DATE"] = code.UpdateDate;
                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add2(MuntinBarItems barItems)
        {
            try
            {
                foreach (MuntinBarItem item in barItems)
                {
                    var itemDate = item.UpdateDate;
                    foreach (Types.Enterprise.MuntinBarCode code in item.MuntinBarCodes)
                    {
                        DataRow dr = this.NewRow();
                        dr["MCODE"] = code.MCode;
                        dr["MCODE_NUM"] = code.MCodeNum;
                        dr["DESCRIPTION"] = code.Description;
                        dr["BAR_TYPE_CD"] = code.Type;
                        dr["EXT_COLOR_CD"] = code.ExtColor;
                        dr["EXT_COLOR_DESC"] = code.ExtColorDesc;
                        dr["INT_COLOR_CD"] = code.IntColor;
                        dr["INT_COLOR_DESC"] = code.IntColorDesc;
                        dr["IS_TWO_TONE"] = code.IsTwoTone;
                        dr["THICKNESS"] = code.Thickness;
                        dr["WIDTH"] = code.Width;
                        dr["UPDATE_DATE"] = code.UpdateDate;
                        this.Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
