using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitSurfaceTable : DataTable
    {

        public SpecUnitSurfaceTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_SURFACE_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("EXT_PRESERVE_COLOR", typeof(string));
            Columns.Add("EXT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("EXT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("INT_PRESERVE_COLOR", typeof(string));
            Columns.Add("INT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("INT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("OPACIFIER", typeof(string));
            Columns.Add("OVERLAY_BAR", typeof(string));
            Columns.Add("PROTECTIVE_FILM_CD", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                DataRow dr = NewRow();
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.Id;
                dr["EXT_PRESERVE_COLOR"] = item.GlassItem.ExteriorSurface.PreserveColor;
                dr["EXT_PRESERVE_CUTBACK"] = item.GlassItem.ExteriorSurface.PreserveCutback;
                dr["EXT_SCORING_WIDTH"] = item.GlassItem.ExteriorSurface.ScoringWidth;
                dr["INT_PRESERVE_COLOR"] = item.GlassItem.InteriorSurface.PreserveColor;
                dr["INT_PRESERVE_CUTBACK"] = item.GlassItem.InteriorSurface.PreserveCutback;
                dr["INT_SCORING_WIDTH"] = item.GlassItem.InteriorSurface.ScoringWidth;
                dr["OPACIFIER"] = item.GlassItem.ExteriorSurface.Opacifier;
                dr["OVERLAY_BAR"] = item.GlassItem.ExteriorSurface.CustomerOverlayBar;
                dr["PROTECTIVE_FILM_CD"] = item.GlassItem.ExteriorSurface.PreserveProcessing;
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}
