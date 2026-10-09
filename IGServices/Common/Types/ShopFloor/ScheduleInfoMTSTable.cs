using System;
using System.Data;
using System.ServiceModel;
using IGGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Enterprise;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class ScheduleInfoMTSTable : DataTable
    {
        public ScheduleInfoMTSTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[ScheduleInfoMTSLedger]");
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("ScheduleSeq", typeof(decimal));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("ScheduledQty", typeof(int));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("OrderLine", typeof(decimal));
            Columns.Add("WOrderType", typeof(string));
            Columns.Add("LineID", typeof(int));
            Columns.Add("ItemNumber", typeof(string));
            Columns.Add("Dimensions", typeof(string));
            Columns.Add("Construction", typeof(string));
            Columns.Add("Customer", typeof(string));
            Columns.Add("Lite1_Inventory", typeof(int));
            Columns.Add("Lite2_Inventory", typeof(int));
            Columns.Add("Lite3_Inventory", typeof(int));
            Columns.Add("Finished_Inventory", typeof(int));
            Columns.Add("Lite1_ReleaseNote", typeof(string));
            Columns.Add("Lite1_ReleaseTimeNote", typeof(string));
            Columns.Add("Lite2_ReleaseNote", typeof(string));
            Columns.Add("Lite2_ReleaseTimeNote", typeof(string));
            Columns.Add("Lite3_ReleaseNote", typeof(string));
            Columns.Add("Lite3_ReleaseTimeNote", typeof(string));
            Columns.Add("Completed", typeof(bool));
            Columns.Add("MuntinComplete", typeof(bool));
            Columns.Add("ChannelComplete", typeof(bool));
            Columns.Add("GlassComplete", typeof(bool));
            Columns.Add("PackingComplete", typeof(bool));
            Columns.Add("InterLayerComplete", typeof(bool));
            Columns.Add("AutoClaveComplete", typeof(bool));
            Columns.Add("ChangedNote", typeof(string));
            Columns.Add("SalesMsg1", typeof(string));
            Columns.Add("SalesMsg2", typeof(string));
            Columns.Add("SalesMsg3", typeof(string));
            Columns.Add("SalesMsg4", typeof(string));
            Columns.Add("ProcessMsg1", typeof(string));
            Columns.Add("ProcessMsg2", typeof(string));
            Columns.Add("ShapeNum", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("E1BatchID", typeof(string));
            Columns.Add("PlantID", typeof(string));
            Columns.Add("WrittenDate", typeof(DateTime));
            Columns.Add("Lite1Weight", typeof(decimal));
            Columns.Add("Lite2Weight", typeof(decimal));
            Columns.Add("Lite3Weight", typeof(decimal));
        }

        public void Add(MTSIncomingOrderLines mtsIncomingOrderLines)
        {
            try
            {
                foreach (var mtsIncomingOrderLine in mtsIncomingOrderLines)
                {
                    var dr = NewRow();
                    dr["ScheduleNum"] = mtsIncomingOrderLine.ScheduleNum;
                    dr["ScheduleSeq"] = mtsIncomingOrderLine.ScheduleSeq;
                    dr["ScheduleDate"] = mtsIncomingOrderLine.ScheduleDate;
                    dr["ScheduledQty"] = mtsIncomingOrderLine.ScheduledQty;
                    dr["WOrderNum"] = mtsIncomingOrderLine.WOrderNum;
                    dr["OrderLine"] = mtsIncomingOrderLine.OrderLine;
                    dr["WOrderType"] = mtsIncomingOrderLine.WOrderType;
                    dr["LineID"] = mtsIncomingOrderLine.LineID;
                    dr["ItemNumber"] = mtsIncomingOrderLine.ItemNumber;
                    dr["Dimensions"] = mtsIncomingOrderLine.Dimensions;
                    dr["Construction"] = mtsIncomingOrderLine.Construction;
                    dr["Customer"] = mtsIncomingOrderLine.Customer;
                    dr["Lite1_Inventory"] = mtsIncomingOrderLine.Lite1_Inventory;
                    dr["Lite2_Inventory"] = mtsIncomingOrderLine.Lite2_Inventory;
                    dr["Lite3_Inventory"] = mtsIncomingOrderLine.Lite3_Inventory;
                    dr["Finished_Inventory"] = mtsIncomingOrderLine.Finished_Inventory;
                    dr["Lite1_ReleaseNote"] = mtsIncomingOrderLine.Lite1_ReleaseNote;
                    dr["Lite1_ReleaseTimeNote"] = mtsIncomingOrderLine.Lite1_ReleaseTimeNote;
                    dr["Lite2_ReleaseNote"] = mtsIncomingOrderLine.Lite2_ReleaseNote;
                    dr["Lite2_ReleaseTimeNote"] = mtsIncomingOrderLine.Lite2_ReleaseTimeNote;
                    dr["Lite3_ReleaseNote"] = mtsIncomingOrderLine.Lite3_ReleaseNote;
                    dr["Lite3_ReleaseTimeNote"] = mtsIncomingOrderLine.Lite3_ReleaseTimeNote;
                    dr["Completed"] = mtsIncomingOrderLine.Completed;
                    dr["MuntinComplete"] = mtsIncomingOrderLine.MuntinComplete;
                    dr["ChannelComplete"] = mtsIncomingOrderLine.ChannelComplete;
                    dr["GlassComplete"] = mtsIncomingOrderLine.GlassComplete;
                    dr["PackingComplete"] = mtsIncomingOrderLine.PackingComplete;
                    dr["InterLayerComplete"] = mtsIncomingOrderLine.InterLayerComplete;
                    dr["AutoClaveComplete"] = mtsIncomingOrderLine.AutoClaveComplete;
                    dr["ChangedNote"] = mtsIncomingOrderLine.ChangedNote;
                    dr["SalesMsg1"] = mtsIncomingOrderLine.SalesMsg1;
                    dr["SalesMsg2"] = mtsIncomingOrderLine.SalesMsg2;
                    dr["SalesMsg3"] = mtsIncomingOrderLine.SalesMsg3;
                    dr["SalesMsg4"] = mtsIncomingOrderLine.SalesMsg4;
                    dr["ProcessMsg1"] = mtsIncomingOrderLine.ProcessMsg1;
                    dr["ProcessMsg2"] = mtsIncomingOrderLine.ProcessMsg2;
                    dr["ShapeNum"] = mtsIncomingOrderLine.ShapeNum;
                    dr["RackType"] = mtsIncomingOrderLine.RackType;
                    dr["LitesPerRack"] = mtsIncomingOrderLine.LitesPerRack;
                    dr["E1BatchID"] = mtsIncomingOrderLine.E1BatchID;
                    dr["PlantID"] = mtsIncomingOrderLine.PlantNum;
                    dr["WrittenDate"] = mtsIncomingOrderLine.WrittenDate;
                    dr["Lite1Weight"] = mtsIncomingOrderLine.Lite1Weight;
                    dr["Lite2Weight"] = mtsIncomingOrderLine.Lite2Weight;
                    dr["Lite3Weight"] = mtsIncomingOrderLine.Lite3Weight;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (ScheduleLine line in schedule.ScheduleLines)
                {
                    var dr = NewRow();
                    dr["ScheduleNum"] = line.F580021.S7Z58SCN;
                    dr["ScheduleSeq"] = line.F580021.S7Z58SCSN;
                    dr["ScheduleDate"] = Utilities.Common.CastToDateTime(line.F580021.S7Z58SCDDJ);
                    dr["ScheduledQty"] = line.F580021.S7Z58QTY;
                    dr["WOrderNum"] = line.F580021.S7DOCO;
                    dr["OrderLine"] = 0;
                    dr["WOrderType"] = line.F580021.S7DCTO;
                    dr["LineID"] = int.TryParse(line.F580021.S7Z58IGLN, out var value) ? value : 0;
                    dr["ItemNumber"] = line.F580021.S7LITM;
                    dr["Dimensions"] = line.F580021.S7DSC2;
                    dr["Construction"] = line.F580021.S7DSC1;
                    dr["Customer"] = line.F580021.S7ALPH;
                    dr["Lite1_Inventory"] = Convert.ToInt32(line.F580021.S7Z58L1QOH);
                    dr["Lite2_Inventory"] = Convert.ToInt32(line.F580021.S7Z58L1QOH);
                    dr["Lite3_Inventory"] = Convert.ToInt32(line.F580021.S7Z58L1QOH);
                    dr["Finished_Inventory"] = Convert.ToInt32(line.F580021.S7Z58L4QOH);
                    dr["Lite1_ReleaseNote"] = line.F580021.S7Z58FLRN;
                    dr["Lite1_ReleaseTimeNote"] = line.F580021.S7Z58FLTN;
                    dr["Lite2_ReleaseNote"] = line.F580021.S7Z58SLRN;
                    dr["Lite2_ReleaseTimeNote"] = line.F580021.S7Z58SLTN;
                    dr["Lite3_ReleaseNote"] = line.F580021.S7Z58TLRN;
                    dr["Lite3_ReleaseTimeNote"] = line.F580021.S7Z58TLTN;
                    dr["Completed"] = false;
                    dr["MuntinComplete"] = false;
                    dr["ChannelComplete"] = false;
                    dr["GlassComplete"] = false;
                    dr["PackingComplete"] = false;
                    dr["InterLayerComplete"] = false;
                    dr["AutoClaveComplete"] = false;
                    dr["ChangedNote"] = line.F580021.S7Z58CHGN;
                    dr["SalesMsg1"] = line.F580021.S7Z58GAL1;
                    dr["SalesMsg2"] = line.F580021.S7Z58GAL2;
                    dr["SalesMsg3"] = "";
                    dr["SalesMsg4"] = "";
                    dr["ProcessMsg1"] = "";
                    dr["ProcessMsg2"] = "";
                    dr["ShapeNum"] = line.F580021.S7Z58SHAPE;
                    dr["RackType"] = line.F580021.S7Z58RT;
                    dr["LitesPerRack"] = line.F580021.S7Z58LPR;
                    dr["E1BatchID"] = line.F580021.S7EDBT;
                    dr["PlantID"] = line.F580021.S7MCU;
                    dr["WrittenDate"] = DateTime.Now;
                    dr["Lite1Weight"] = line.F580024.S8Z5887004 / 1000;
                    dr["Lite2Weight"] = line.F580024.S8Z5887005 / 1000;
                    dr["Lite3Weight"] = line.F580024.S8Z5887006 / 1000;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
