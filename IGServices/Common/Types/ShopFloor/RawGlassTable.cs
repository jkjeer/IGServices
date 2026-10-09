using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.E1;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class RawGlassTable : DataTable
    {
        public RawGlassTable()
        {
            //ExtendedProperties.Add("Target", "[EBS].[RawGlassLedger]");
            ExtendedProperties.Add("Target", "[dbo].[Raw_Glass_Ledger]");
            Columns.Add("RawGlassID", typeof(decimal));
            Columns.Add("BusinessUnit", typeof(string));
            Columns.Add("PartType", typeof(string));
            Columns.Add("DocType", typeof(string));
            Columns.Add("WorderNum", typeof(decimal));
            Columns.Add("ItemNumber", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("OrderQty", typeof(decimal));
            Columns.Add("QtyAvail", typeof(decimal));
            Columns.Add("Desc1", typeof(string));
            Columns.Add("Desc2", typeof(string));
            Columns.Add("LiteThickness", typeof(decimal));
            Columns.Add("LiteHeat", typeof(string));
            Columns.Add("LiteColor", typeof(string));
            Columns.Add("FirstSrfCoating", typeof(string));
            Columns.Add("SecondSrfCoating", typeof(string));
            Columns.Add("LitePattern", typeof(string));
            Columns.Add("BaseDim", typeof(decimal));
            Columns.Add("BaseN", typeof(decimal));
            Columns.Add("BaseD", typeof(decimal));
            Columns.Add("LeftDim", typeof(decimal));
            Columns.Add("LeftN", typeof(decimal));
            Columns.Add("LeftD", typeof(decimal));
            Columns.Add("RightDim", typeof(decimal));
            Columns.Add("RightN", typeof(decimal));
            Columns.Add("RightD", typeof(decimal));
            Columns.Add("TopDim", typeof(decimal));
            Columns.Add("TopN", typeof(decimal));
            Columns.Add("TopD", typeof(decimal));
            Columns.Add("ShapeNum", typeof(string));
            Columns.Add("E1BatchID", typeof(string));
            Columns.Add("S7ITM", typeof(decimal));
            Columns.Add("S7Z58SCN", typeof(decimal));
            Columns.Add("WMOPSQ", typeof(decimal));
            Columns.Add("TimeStamp", typeof(DateTime));
        }

        public void Add(RawGlassItems rawGlassItems)
        {
            try
            {
                foreach (var item in rawGlassItems)
                {
                    var dr = NewRow();
                    dr["BusinessUnit"] = item.WMCMCU;
                    dr["PartType"] = item.IBGLPT;
                    dr["DocType"] = item.WMDCTO;
                    dr["WorderNum"] = item.WMDOCO;
                    dr["ItemNumber"] = item.WMCPIT;
                    dr["Location"] = item.WMLOCN;
                    dr["OrderQty"] = item.WMUORG / 1000;
                    dr["QtyAvail"] = item.WMTRQT / 1000;
                    dr["Desc1"] = item.WMDSC1;
                    dr["Desc2"] = item.WMDSC2;
                    dr["LiteThickness"] = item.CRZ58L1TK;
                    dr["LiteHeat"] = item.CRZ58L1HT;
                    dr["LiteColor"] = item.CRZ58L1CL;
                    dr["FirstSrfCoating"] = item.CRZ58L11C;
                    dr["SecondSrfCoating"] = item.CRZ58L12C;
                    dr["LitePattern"] = item.CRZ58L1PX;
                    dr["BaseDim"] = item.CRZ58L1BDM;
                    dr["BaseN"] = item.CRZ58L1BNM;
                    dr["BaseD"] = item.CRZ58L1BDN;
                    dr["LeftDim"] = item.CRZ58L1LDM;
                    dr["LeftN"] = item.CRZ58L1LNM;
                    dr["LeftD"] = item.CRZ58L1LDN;
                    dr["RightDim"] = item.CRZ58L1RDM;
                    dr["RightN"] = item.CRZ58L1RNM;
                    dr["RightD"] = item.CRZ58L1RDN;
                    dr["TopDim"] = item.CRZ58L1TDM;
                    dr["TopN"] = item.CRZ58L1TNM;
                    dr["TopD"] = item.CRZ58L1TDN;
                    dr["ShapeNum"] = item.CRZ58SHAPE;
                    dr["E1BatchID"] = item.S7EDBT;
                    dr["S7ITM"] = item.S7ITM;
                    dr["S7Z58SCN"] = item.S7Z58SCN;
                    dr["WMOPSQ"] = item.WMOPSQ;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
