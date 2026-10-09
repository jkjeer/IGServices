using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;


namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class RawGlassEbsTable : DataTable
    {
        public RawGlassEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RawGlassLedger]");
            Columns.Add("BusinessUnit", typeof(string));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("WorderNum", typeof(decimal));
            Columns.Add("DocType", typeof(string));
            Columns.Add("WorderItemNum", typeof(decimal));
            Columns.Add("WorderUnitType", typeof(string));

            Columns.Add("PartType", typeof(string));
            Columns.Add("PartName", typeof(string));
            Columns.Add("PartIndex", typeof(int));
            Columns.Add("OpSeqNum", typeof(decimal));
            Columns.Add("OpSeqIndex", typeof(int));
            Columns.Add("ItemNum", typeof(string));
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

            Columns.Add("TransactionDate", typeof(DateTime));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("E1BatchID", typeof(string));

        }

        public void Add(RawGlassItems rawGlassItems)
        {
            try
            {
                foreach (var item in rawGlassItems)
                {
                    var dr = this.NewRow();
                    dr["BusinessUnit"] = item.WMCMCU;
                    dr["ScheduleNum"] = item.S7Z58SCN;
                    dr["WorderNum"] = item.WMDOCO;
                    dr["DocType"] = item.WMDCTO;
                    dr["WorderItemNum"] = item.S7ITM;
                    dr["WorderUnitType"] = item.IBSRP4;

                    dr["PartType"] = item.IBGLPT;
                    dr["OpSeqNum"] = item.WMOPSQ;
                    dr["OpSeqIndex"] = item.WMOPSQ_RN;
                    dr["ItemNum"] = item.WMCPIT;
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

                    dr["TransactionId"] = item.WMUKID;
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(item.WMUPMJ, item.WMTDAY);
                    dr["E1BatchID"] = item.S7EDBT;

                    //Lite Name
                    string partName = "";
                    int partIndex = 0;
                    switch (item.IBSRP4)
                    {
                        case "1":
                            partName = "EXT";
                            partIndex = 1;
                            break;
                        case "L":
                            partName = "EXT";
                            partIndex = 1;
                            break;
                        case "2":
                            switch (item.WMOPSQ_RN)
                            {
                                case 1:
                                    partName = "EXT";
                                    partIndex = 1;
                                    break;
                                case 2:
                                    partName = "INT";
                                    partIndex = 2;
                                    break;
                                case 3:
                                    partName = "EXT";
                                    partIndex = 1;
                                    break;
                                case 4:
                                    partName = "INT";
                                    partIndex = 2;
                                    break;
                            }
                            break;
                        case "3":
                            switch (item.WMOPSQ_RN)
                            {
                                case 1:
                                    partName = "EXT";
                                    partIndex = 1;
                                    break;
                                case 2:
                                    partName = "MID";
                                    partIndex = 2;
                                    break;
                                case 3:
                                    partName = "INT";
                                    partIndex = 3;
                                    break;
                                case 4:
                                    partName = "EXT";
                                    partIndex = 1;
                                    break;
                                case 5:
                                    partName = "MID";
                                    partIndex = 2;
                                    break;
                                case 6:
                                    partName = "INT";
                                    partIndex = 3;
                                    break;
                            }
                            break;
                        default:
                            partName = "EXT";
                            partIndex = 1;
                            break;
                    }
                    dr["PartName"] = partName;
                    dr["PartIndex"] = partIndex;

                    this.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
