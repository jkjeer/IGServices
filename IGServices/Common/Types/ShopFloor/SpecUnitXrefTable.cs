using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitXrefTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public SpecUnitXrefTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_XREF_LEDGER]");
            Columns.Add("DOCUMENT_NUM", typeof(decimal));
            Columns.Add("DOCUMENT_TYPE_CD", typeof(string));
            Columns.Add("DOCUMENT_LINE_NUM", typeof(decimal));
            Columns.Add("PLANT_CD", typeof(string));
            Columns.Add("LOT_NUM", typeof(string));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                if (item.Id > 0)
                {
                    foreach (OrderId orderId in item.OrderIds)
                    {
                        DataRow dr = NewRow();
                        dr["DOCUMENT_NUM"] = orderId.DocumentNum;
                        dr["DOCUMENT_TYPE_CD"] = orderId.DocumentType;
                        dr["DOCUMENT_LINE_NUM"] = orderId.DocumentLineNum / 1000;
                        dr["PLANT_CD"] = orderId.Plant;
                        dr["LOT_NUM"] = orderId.LotNum;
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = item.Id;
                        dr["UPDATE_DATE"] = item.TransactionDate;
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        //public void Add(Schedule schedule)
        //{
        //    try
        //    {
        //        foreach (var line in schedule.ScheduleLines)
        //        {
        //            if (line.F580021.CMCFGID > 0)
        //            {
        //                if (line.F580021.S7RORN != "")
        //                {
        //                    DataRow dr = NewRow();
        //                    dr["DOCUMENT_NUM"] = Data.ParseStringToDecimal(line.F580021.S7RORN);
        //                    dr["DOCUMENT_TYPE_CD"] = "WO";
        //                    dr["DOCUMENT_LINE_NUM"] = 0;
        //                    dr["PLANT_CD"] = line.F580021.S7MCU;
        //                    dr["LOT_NUM"] = line.F580021.S7RORN;
        //                    dr["ITEM_NUM"] = line.F580021.S7ITM;
        //                    dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
        //                    dr["UPDATE_DATE"] = E1TimeManager.CastToDateTime(line.F580021.S7UPMJ, line.F580021.S7TDAY);
        //                    Rows.Add(dr);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new FaultException(e.Message);
        //    }
        //}
    }
}
