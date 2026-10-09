using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitGridTable : DataTable
    {

        public SpecUnitGridTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_GRID_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("GRID_STYLE_CD", typeof(string));
            Columns.Add("SUB_GRID_STYLE_CD", typeof(string));
            Columns.Add("HORZ_BARS_QTY", typeof(decimal));
            Columns.Add("VERT_BARS_QTY", typeof(decimal));
            Columns.Add("HIGH_OPEN_QTY", typeof(decimal));
            Columns.Add("WIDE_OPEN_QTY", typeof(decimal));
            Columns.Add("ODD_OPEN_QTY", typeof(decimal));
            Columns.Add("RECT_OPEN_QTY", typeof(decimal));
            Columns.Add("DRAWING_CD", typeof(string));
            Columns.Add("HORZ_SASH_INTRUSION", typeof(decimal));
            Columns.Add("VERT_SASH_INTRUSION", typeof(decimal));
            Columns.Add("HORZ_PRAIRIE_OFFSET", typeof(decimal));
            Columns.Add("VERT_PRAIRIE_OFFSET", typeof(decimal));
            Columns.Add("PLACEMENT_TYPE_CD", typeof(string));
            Columns.Add("HORZ_POS_01", typeof(decimal));
            Columns.Add("HORZ_POS_02", typeof(decimal));
            Columns.Add("HORZ_POS_03", typeof(decimal));
            Columns.Add("HORZ_POS_04", typeof(decimal));
            Columns.Add("HORZ_POS_05", typeof(decimal));
            Columns.Add("HORZ_POS_06", typeof(decimal));
            Columns.Add("HORZ_POS_07", typeof(decimal));
            Columns.Add("HORZ_POS_08", typeof(decimal));
            Columns.Add("HORZ_POS_09", typeof(decimal));
            Columns.Add("HORZ_POS_10", typeof(decimal));
            Columns.Add("HORZ_POS_11", typeof(decimal));
            Columns.Add("HORZ_POS_12", typeof(decimal));
            Columns.Add("HORZ_POS_13", typeof(decimal));
            Columns.Add("HORZ_POS_14", typeof(decimal));
            Columns.Add("HORZ_POS_15", typeof(decimal));
            Columns.Add("HORZ_POS_16", typeof(decimal));
            Columns.Add("HORZ_POS_17", typeof(decimal));
            Columns.Add("HORZ_POS_18", typeof(decimal));
            Columns.Add("HORZ_POS_19", typeof(decimal));
            Columns.Add("HORZ_POS_20", typeof(decimal));
            Columns.Add("VERT_POS_01", typeof(decimal));
            Columns.Add("VERT_POS_02", typeof(decimal));
            Columns.Add("VERT_POS_03", typeof(decimal));
            Columns.Add("VERT_POS_04", typeof(decimal));
            Columns.Add("VERT_POS_05", typeof(decimal));
            Columns.Add("VERT_POS_06", typeof(decimal));
            Columns.Add("VERT_POS_07", typeof(decimal));
            Columns.Add("VERT_POS_08", typeof(decimal));
            Columns.Add("VERT_POS_09", typeof(decimal));
            Columns.Add("VERT_POS_10", typeof(decimal));
            Columns.Add("VERT_POS_11", typeof(decimal));
            Columns.Add("VERT_POS_12", typeof(decimal));
            Columns.Add("VERT_POS_13", typeof(decimal));
            Columns.Add("VERT_POS_14", typeof(decimal));
            Columns.Add("VERT_POS_15", typeof(decimal));
            Columns.Add("VERT_POS_16", typeof(decimal));
            Columns.Add("VERT_POS_17", typeof(decimal));
            Columns.Add("VERT_POS_18", typeof(decimal));
            Columns.Add("VERT_POS_19", typeof(decimal));
            Columns.Add("VERT_POS_20", typeof(decimal));
            Columns.Add("HORZ_ANGLE_01", typeof(decimal));
            Columns.Add("HORZ_ANGLE_02", typeof(decimal));
            Columns.Add("HORZ_ANGLE_03", typeof(decimal));
            Columns.Add("HORZ_ANGLE_04", typeof(decimal));
            Columns.Add("HORZ_ANGLE_05", typeof(decimal));
            Columns.Add("HORZ_ANGLE_06", typeof(decimal));
            Columns.Add("HORZ_ANGLE_07", typeof(decimal));
            Columns.Add("HORZ_ANGLE_08", typeof(decimal));
            Columns.Add("HORZ_ANGLE_09", typeof(decimal));
            Columns.Add("HORZ_ANGLE_10", typeof(decimal));
            Columns.Add("HORZ_ANGLE_11", typeof(decimal));
            Columns.Add("HORZ_ANGLE_12", typeof(decimal));
            Columns.Add("HORZ_ANGLE_13", typeof(decimal));
            Columns.Add("HORZ_ANGLE_14", typeof(decimal));
            Columns.Add("HORZ_ANGLE_15", typeof(decimal));
            Columns.Add("HORZ_ANGLE_16", typeof(decimal));
            Columns.Add("HORZ_ANGLE_17", typeof(decimal));
            Columns.Add("HORZ_ANGLE_18", typeof(decimal));
            Columns.Add("HORZ_ANGLE_19", typeof(decimal));
            Columns.Add("HORZ_ANGLE_20", typeof(decimal));
            Columns.Add("VERT_ANGLE_01", typeof(decimal));
            Columns.Add("VERT_ANGLE_02", typeof(decimal));
            Columns.Add("VERT_ANGLE_03", typeof(decimal));
            Columns.Add("VERT_ANGLE_04", typeof(decimal));
            Columns.Add("VERT_ANGLE_05", typeof(decimal));
            Columns.Add("VERT_ANGLE_06", typeof(decimal));
            Columns.Add("VERT_ANGLE_07", typeof(decimal));
            Columns.Add("VERT_ANGLE_08", typeof(decimal));
            Columns.Add("VERT_ANGLE_09", typeof(decimal));
            Columns.Add("VERT_ANGLE_10", typeof(decimal));
            Columns.Add("VERT_ANGLE_11", typeof(decimal));
            Columns.Add("VERT_ANGLE_12", typeof(decimal));
            Columns.Add("VERT_ANGLE_13", typeof(decimal));
            Columns.Add("VERT_ANGLE_14", typeof(decimal));
            Columns.Add("VERT_ANGLE_15", typeof(decimal));
            Columns.Add("VERT_ANGLE_16", typeof(decimal));
            Columns.Add("VERT_ANGLE_17", typeof(decimal));
            Columns.Add("VERT_ANGLE_18", typeof(decimal));
            Columns.Add("VERT_ANGLE_19", typeof(decimal));
            Columns.Add("VERT_ANGLE_20", typeof(decimal));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                var g = item.GlassItem.Grid;
                if (!String.IsNullOrEmpty(g.GridStyle))
                {
                    DataRow dr = NewRow();
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.Id;
                    dr["GRID_STYLE_CD"] = (string)g.GridStyle;
                    dr["SUB_GRID_STYLE_CD"] = g.SubGridStyle;
                    dr["HORZ_BARS_QTY"] = g.NumHorizontal;
                    dr["VERT_BARS_QTY"] = g.NumVertical;
                    dr["HIGH_OPEN_QTY"] = g.NumOpeningsHigh;
                    dr["WIDE_OPEN_QTY"] = g.NumOpeningsWide;
                    dr["ODD_OPEN_QTY"] = g.NumOpeningsOdd;
                    dr["RECT_OPEN_QTY"] = g.NumOpeningsRectangle;
                    dr["DRAWING_CD"] = g.Drawing;
                    dr["HORZ_SASH_INTRUSION"] = item.GlassItem.HorizontalSashIntrusion;
                    dr["VERT_SASH_INTRUSION"] = item.GlassItem.VerticalSashIntrusion;
                    dr["HORZ_PRAIRIE_OFFSET"] = g.HorizontalPrairieOffset;
                    dr["VERT_PRAIRIE_OFFSET"] = g.VerticalPrairieOffset;
                    dr["PLACEMENT_TYPE_CD"] = g.PlacementType;
                    var hpcount = g.HorizontalBarPlacements.Count;
                    var vpcount = g.VerticalBarPlacements.Count;

                    // HORIZONTAL
                    if (hpcount > 0)
                    {
                        dr["HORZ_POS_01"] = g.HorizontalBarPlacements[0].Position;
                        dr["HORZ_ANGLE_01"] = g.HorizontalBarPlacements[0].Angle;
                    }
                    if (hpcount > 1)
                    {
                        dr["HORZ_POS_02"] = g.HorizontalBarPlacements[1].Position;
                        dr["HORZ_ANGLE_02"] = g.HorizontalBarPlacements[1].Angle;
                    }
                    if (hpcount > 2)
                    {
                        dr["HORZ_POS_03"] = g.HorizontalBarPlacements[2].Position;
                        dr["HORZ_ANGLE_03"] = g.HorizontalBarPlacements[2].Angle;
                    }
                    if (hpcount > 3)
                    {
                        dr["HORZ_POS_04"] = g.HorizontalBarPlacements[3].Position;
                        dr["HORZ_ANGLE_04"] = g.HorizontalBarPlacements[3].Angle;
                    }
                    if (hpcount > 4)
                    {
                        dr["HORZ_POS_05"] = g.HorizontalBarPlacements[4].Position;
                        dr["HORZ_ANGLE_05"] = g.HorizontalBarPlacements[4].Angle;
                    }
                    if (hpcount > 5)
                    {
                        dr["HORZ_POS_06"] = g.HorizontalBarPlacements[5].Position;
                        dr["HORZ_ANGLE_06"] = g.HorizontalBarPlacements[5].Angle;
                    }
                    if (hpcount > 6)
                    {
                        dr["HORZ_POS_07"] = g.HorizontalBarPlacements[6].Position;
                        dr["HORZ_ANGLE_07"] = g.HorizontalBarPlacements[6].Angle;
                    }
                    if (hpcount > 7)
                    {
                        dr["HORZ_POS_08"] = g.HorizontalBarPlacements[7].Position;
                        dr["HORZ_ANGLE_08"] = g.HorizontalBarPlacements[7].Angle;
                    }
                    if (hpcount > 8)
                    {
                        dr["HORZ_POS_09"] = g.HorizontalBarPlacements[8].Position;
                        dr["HORZ_ANGLE_09"] = g.HorizontalBarPlacements[8].Angle;
                    }
                    if (hpcount > 9)
                    {
                        dr["HORZ_POS_10"] = g.HorizontalBarPlacements[9].Position;
                        dr["HORZ_ANGLE_10"] = g.HorizontalBarPlacements[9].Angle;
                    }
                    if (hpcount > 10)
                    {
                        dr["HORZ_POS_11"] = g.HorizontalBarPlacements[10].Position;
                        dr["HORZ_ANGLE_11"] = g.HorizontalBarPlacements[10].Angle;
                    }
                    if (hpcount > 11)
                    {
                        dr["HORZ_POS_12"] = g.HorizontalBarPlacements[11].Position;
                        dr["HORZ_ANGLE_12"] = g.HorizontalBarPlacements[11].Angle;
                    }
                    if (hpcount > 12)
                    {
                        dr["HORZ_POS_13"] = g.HorizontalBarPlacements[12].Position;
                        dr["HORZ_ANGLE_13"] = g.HorizontalBarPlacements[12].Angle;
                    }
                    if (hpcount > 13)
                    {
                        dr["HORZ_POS_14"] = g.HorizontalBarPlacements[13].Position;
                        dr["HORZ_ANGLE_14"] = g.HorizontalBarPlacements[13].Angle;
                    }
                    if (hpcount > 14)
                    {
                        dr["HORZ_POS_15"] = g.HorizontalBarPlacements[14].Position;
                        dr["HORZ_ANGLE_15"] = g.HorizontalBarPlacements[14].Angle;
                    }
                    if (hpcount > 15)
                    {
                        dr["HORZ_POS_16"] = g.HorizontalBarPlacements[15].Position;
                        dr["HORZ_ANGLE_16"] = g.HorizontalBarPlacements[15].Angle;
                    }
                    if (hpcount > 16)
                    {
                        dr["HORZ_POS_17"] = g.HorizontalBarPlacements[16].Position;
                        dr["HORZ_ANGLE_17"] = g.HorizontalBarPlacements[16].Angle;
                    }
                    if (hpcount > 17)
                    {
                        dr["HORZ_POS_18"] = g.HorizontalBarPlacements[17].Position;
                        dr["HORZ_ANGLE_18"] = g.HorizontalBarPlacements[17].Angle;
                    }
                    if (hpcount > 18)
                    {
                        dr["HORZ_POS_19"] = g.HorizontalBarPlacements[18].Position;
                        dr["HORZ_ANGLE_19"] = g.HorizontalBarPlacements[18].Angle;
                    }
                    if (hpcount > 19)
                    {
                        dr["HORZ_POS_20"] = g.HorizontalBarPlacements[19].Position;
                        dr["HORZ_ANGLE_20"] = g.HorizontalBarPlacements[19].Angle;
                    }

                    // VERTICAL
                    if (vpcount > 0)
                    {
                        dr["VERT_POS_01"] = g.VerticalBarPlacements[0].Position;
                        dr["VERT_ANGLE_01"] = g.VerticalBarPlacements[0].Angle;
                    }
                    if (vpcount > 1)
                    {
                        dr["VERT_POS_02"] = g.VerticalBarPlacements[1].Position;
                        dr["VERT_ANGLE_02"] = g.VerticalBarPlacements[1].Angle;
                    }
                    if (vpcount > 2)
                    {
                        dr["VERT_POS_03"] = g.VerticalBarPlacements[2].Position;
                        dr["VERT_ANGLE_03"] = g.VerticalBarPlacements[2].Angle;
                    }
                    if (vpcount > 3)
                    {
                        dr["VERT_POS_04"] = g.VerticalBarPlacements[3].Position;
                        dr["VERT_ANGLE_04"] = g.VerticalBarPlacements[3].Angle;
                    }
                    if (vpcount > 4)
                    {
                        dr["VERT_POS_05"] = g.VerticalBarPlacements[4].Position;
                        dr["VERT_ANGLE_05"] = g.VerticalBarPlacements[4].Angle;
                    }
                    if (vpcount > 5)
                    {
                        dr["VERT_POS_06"] = g.VerticalBarPlacements[5].Position;
                        dr["VERT_ANGLE_06"] = g.VerticalBarPlacements[5].Angle;
                    }
                    if (vpcount > 6)
                    {
                        dr["VERT_POS_07"] = g.VerticalBarPlacements[6].Position;
                        dr["VERT_ANGLE_07"] = g.VerticalBarPlacements[6].Angle;
                    }
                    if (vpcount > 7)
                    {
                        dr["VERT_POS_08"] = g.VerticalBarPlacements[7].Position;
                        dr["VERT_ANGLE_08"] = g.VerticalBarPlacements[7].Angle;
                    }
                    if (vpcount > 8)
                    {
                        dr["VERT_POS_09"] = g.VerticalBarPlacements[8].Position;
                        dr["VERT_ANGLE_09"] = g.VerticalBarPlacements[8].Angle;
                    }
                    if (vpcount > 9)
                    {
                        dr["VERT_POS_10"] = g.VerticalBarPlacements[9].Position;
                        dr["VERT_ANGLE_10"] = g.VerticalBarPlacements[9].Angle;
                    }
                    if (vpcount > 10)
                    {
                        dr["VERT_POS_11"] = g.VerticalBarPlacements[10].Position;
                        dr["VERT_ANGLE_11"] = g.VerticalBarPlacements[10].Angle;
                    }
                    if (vpcount > 11)
                    {
                        dr["VERT_POS_12"] = g.VerticalBarPlacements[11].Position;
                        dr["VERT_ANGLE_12"] = g.VerticalBarPlacements[11].Angle;
                    }
                    if (vpcount > 12)
                    {
                        dr["VERT_POS_13"] = g.VerticalBarPlacements[12].Position;
                        dr["VERT_ANGLE_13"] = g.VerticalBarPlacements[12].Angle;
                    }
                    if (vpcount > 13)
                    {
                        dr["VERT_POS_14"] = g.VerticalBarPlacements[13].Position;
                        dr["VERT_ANGLE_14"] = g.VerticalBarPlacements[13].Angle;
                    }
                    if (vpcount > 14)
                    {
                        dr["VERT_POS_15"] = g.VerticalBarPlacements[14].Position;
                        dr["VERT_ANGLE_15"] = g.VerticalBarPlacements[14].Angle;
                    }
                    if (vpcount > 15)
                    {
                        dr["VERT_POS_16"] = g.VerticalBarPlacements[15].Position;
                        dr["VERT_ANGLE_16"] = g.VerticalBarPlacements[15].Angle;
                    }
                    if (vpcount > 16)
                    {
                        dr["VERT_POS_17"] = g.VerticalBarPlacements[16].Position;
                        dr["VERT_ANGLE_17"] = g.VerticalBarPlacements[16].Angle;
                    }
                    if (vpcount > 17)
                    {
                        dr["VERT_POS_18"] = g.VerticalBarPlacements[17].Position;
                        dr["VERT_ANGLE_18"] = g.VerticalBarPlacements[17].Angle;
                    }
                    if (vpcount > 18)
                    {
                        dr["VERT_POS_19"] = g.VerticalBarPlacements[18].Position;
                        dr["VERT_ANGLE_19"] = g.VerticalBarPlacements[18].Angle;
                    }
                    if (vpcount > 19)
                    {
                        dr["VERT_POS_20"] = g.VerticalBarPlacements[19].Position;
                        dr["VERT_ANGLE_20"] = g.VerticalBarPlacements[19].Angle;
                    }

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

