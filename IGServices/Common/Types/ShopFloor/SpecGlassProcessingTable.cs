using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecGlassProcessingTable : DataTable
    {

        public SpecGlassProcessingTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_LITE_PROCESSING_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("LITE_NO", typeof(int));
            Columns.Add("LITE_CD", typeof(string));
            Columns.Add("ROUTING_CD", typeof(string));
            Columns.Add("EDGEWORK_CD", typeof(string));
            Columns.Add("VGROOVE_CD", typeof(string));
            Columns.Add("VGROOVE_SURFACE_CD", typeof(string));
            Columns.Add("VGROOVE_STYLE_CD", typeof(string));
            Columns.Add("VGROOVE_HIGH", typeof(decimal));
            Columns.Add("VGROOVE_WIDE", typeof(decimal));
            Columns.Add("VGROOVE_VERT_OFFSET", typeof(decimal));
            Columns.Add("VGROOVE_HORZ_OFFSET", typeof(decimal));
            Columns.Add("VGROOVE_VERT_START", typeof(string));
            Columns.Add("VGROOVE_HORZ_START", typeof(string));
            Columns.Add("HOLES_DRAWING", typeof(string));
            Columns.Add("HOLES_NO", typeof(decimal));
            Columns.Add("NOTCH_LOCATION_CD", typeof(string));
            Columns.Add("NOTCH_WIDTH", typeof(decimal));
            Columns.Add("NOTCH_HEIGHT", typeof(decimal));
            Columns.Add("ANSI_LOGO_CD", typeof(string));
            Columns.Add("TEMP_LOGO", typeof(string));
            Columns.Add("LOGO_OFFSET_X", typeof(decimal));
            Columns.Add("LOGO_OFFSET_Y", typeof(decimal));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                foreach (Lite lt in item.GlassItem.Lites)
                {
                    DataRow dr = NewRow();
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.Id;
                    dr["LITE_NO"] = lt.Position;
                    dr["LITE_CD"] = lt.PositionName;
                    dr["ROUTING_CD"] = lt.RoutingCode;
                    dr["EDGEWORK_CD"] = lt.EdgeWork;
                    dr["VGROOVE_CD"] = lt.VGrooveStyle;
                    dr["VGROOVE_SURFACE_CD"] = lt.VGrooveSurface.ToString();
                    dr["VGROOVE_HIGH"] = lt.VGrooveHighOpen;
                    dr["VGROOVE_WIDE"] = lt.VGrooveWideOpen;
                    dr["VGROOVE_VERT_OFFSET"] = lt.VGrooveVertOffset;
                    dr["VGROOVE_HORZ_OFFSET"] = lt.VGrooveHorzOffset;
                    dr["VGROOVE_VERT_START"] = lt.VGrooveVertStartStop;
                    dr["VGROOVE_HORZ_START"] = lt.VGrooveHorzStartStop;
                    dr["HOLES_DRAWING"] = lt.HolesDrawing;
                    dr["HOLES_NO"] = lt.HolesNum;
                    dr["NOTCH_LOCATION_CD"] = lt.NotchLocation;
                    dr["NOTCH_HEIGHT"] = lt.NotchHeight;
                    dr["NOTCH_WIDTH"] = lt.NotchWidth;
                    dr["ANSI_LOGO_CD"] = item.GlassItem.Logo.AnsiLogo;
                    dr["LOGO_OFFSET_X"] = item.GlassItem.Logo.XOffset;
                    dr["LOGO_OFFSET_Y"] = item.GlassItem.Logo.YOffset;
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
