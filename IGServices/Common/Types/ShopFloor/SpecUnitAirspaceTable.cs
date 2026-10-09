using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitAirspaceTable : DataTable
    {

        public SpecUnitAirspaceTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_AIRSPACE_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("AIRSPACE_NO", typeof(int));
            Columns.Add("CHANNEL_TYPE_CD", typeof(string));
            Columns.Add("CHANNEL_SIZE", typeof(decimal));
            Columns.Add("NARROW_MARGIN_CD", typeof(string));
            Columns.Add("GAS_MIXTURE_CD", typeof(string));
            Columns.Add("BUTYL_COLOR", typeof(string));
            Columns.Add("SILICON_COLOR", typeof(string));
            Columns.Add("MCODE1", typeof(decimal));
            Columns.Add("MCODE1_ORIENTATION", typeof(string));
            Columns.Add("MCODE2", typeof(decimal));
            Columns.Add("MCODE2_ORIENTATION", typeof(string));
            Columns.Add("CCODE", typeof(string));
            Columns.Add("BUMP_ON_TYPE", typeof(string));
            Columns.Add("BUMP_ON_QTY", typeof(decimal));
            Columns.Add("BUMP_ON_CFG", typeof(string));
            Columns.Add("SHADES", typeof(string));
            Columns.Add("BREATHER_TUBE", typeof(string));
            Columns.Add("TAPE", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                if (item.GlassItem.UnitType != "1")
                {
                    foreach (Airspace a in item.GlassItem.Airspaces)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = item.Id;
                        dr["AIRSPACE_NO"] = a.Position;
                        dr["CHANNEL_TYPE_CD"] = a.Spacer.SpacerType;
                        dr["CHANNEL_SIZE"] = a.Spacer.Width;
                        dr["NARROW_MARGIN_CD"] = item.GlassItem.NarrowMarginCode;
                        dr["GAS_MIXTURE_CD"] = a.GasMixture;
                        dr["BUTYL_COLOR"] = a.ButylColor;
                        dr["SILICON_COLOR"] = a.SiliconeColor;
                        dr["SHADES"] = a.Shades;
                        dr["BREATHER_TUBE"] = a.Tubes;
                        dr["TAPE"] = a.Tape;
                        // BARS
                        if (a.BarAssembly != null)
                        {
                            dr["CCODE"] = a.BarAssembly.CCode;
                            dr["BUMP_ON_TYPE"] = a.BarAssembly.BumperType;
                            dr["BUMP_ON_QTY"] = a.BarAssembly.BumperQty;
                            dr["BUMP_ON_CFG"] = a.BarAssembly.BumperConfig;
                            dr["MCODE1"] = (decimal)a.BarAssembly.MuntinBars[0].MCodeNum;
                            dr["MCODE1_ORIENTATION"] = a.BarAssembly.MuntinBars[0].Orientation;
                            if (a.BarAssembly.MuntinBars.Count == 2)
                            {
                                dr["MCODE2"] = (decimal)a.BarAssembly.MuntinBars[1].MCodeNum;
                                dr["MCODE2_ORIENTATION"] = a.BarAssembly.MuntinBars[1].Orientation;
                            }
                        }
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}
