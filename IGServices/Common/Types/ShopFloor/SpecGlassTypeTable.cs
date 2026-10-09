using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecGlassTypeTable : DataTable
    {

        public SpecGlassTypeTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_LITE_GLASS_TYPE_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("LITE_NO", typeof(decimal));
            Columns.Add("LITE_CD", typeof(string));
            Columns.Add("THICKNESS_NO", typeof(decimal));
            Columns.Add("HEAT_TREATMENT_CD", typeof(string));
            Columns.Add("COLOR_CD", typeof(string));
            Columns.Add("SURFACE1_COAT_CD", typeof(string));
            Columns.Add("SURFACE2_COAT_CD", typeof(string));
            Columns.Add("PATTERN_TEXTURE_CD", typeof(string));
            Columns.Add("PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_TYPE_CD", typeof(string));
            Columns.Add("LAMI_COLOR_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_PATTERN_SURFACE_CD", typeof(string));
            Columns.Add("LAMI_COATING_SURFACE_CD", typeof(string));
            Columns.Add("QUALITY_CD", typeof(string));
            Columns.Add("IS_CUSTOMER_SUPPLIED", typeof(bool));
            Columns.Add("CUSTOMER_LITE_NUM", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                foreach (Lite lt in item.GlassItem.Lites)
                {
                    DataRow dr = NewRow();
                    var egt = lt.GlassType;
                    dr["ITEM_NUM"] = item.ItemNum;
                    dr["CONFIGURATION_ID"] = item.Id;
                    dr["LITE_NO"] = lt.Position;
                    dr["LITE_CD"] = lt.PositionName;
                    dr["THICKNESS_NO"] = egt.Thickness;
                    dr["HEAT_TREATMENT_CD"] = egt.HeatTreatment;
                    dr["COLOR_CD"] = egt.ColorCode;
                    dr["SURFACE1_COAT_CD"] = egt.Surface1Coating;
                    dr["SURFACE2_COAT_CD"] = egt.Surface2Coating;
                    dr["PATTERN_TEXTURE_CD"] = egt.PatternTexture;
                    dr["PATTERN_SURFACE_CD"] = egt.PatternTextureSrf;
                    dr["LAMI_TYPE_CD"] = egt.LamiProductType;
                    dr["LAMI_COLOR_SURFACE_CD"] = egt.LamiColorSurface;
                    dr["LAMI_PATTERN_SURFACE_CD"] = egt.LamiPatternSurface;
                    dr["LAMI_COATING_SURFACE_CD"] = egt.LamiCoatingSurface;
                    dr["QUALITY_CD"] = lt.QualityLevel;
                    dr["IS_CUSTOMER_SUPPLIED"] = egt.CustomerSupplied;
                    dr["CUSTOMER_LITE_NUM"] = egt.CustomerLiteNum;
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public void Add(SpecUnits items)
        {
            try
            {
                foreach (SpecUnit item in items)
                {
                    Add(item);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }



        public void Add2(SpecUnits items)
        {
            try
            {
                foreach (SpecUnit item in items)
                {
                    var attributes = item.ItemAttributes;

                    if (attributes.GetDecimalAttributeValue("Z58L1TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIG_ID"] = (int) item.Id;
                        dr["LITE_NO"] = 1;
                        dr["LITE_CD"] = "EXT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L1TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L1HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L11C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L12C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L1PX");
                       // dr["PATTERN_SURFACE_CD"] = egt.PatternTextureSrf;
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L1LPT");
                       // dr["LAMI_COLOR_SURFACE_CD"] = egt.LamiColorSurface;
                       // dr["LAMI_PATTERN_SURFACE_CD"] = egt.LamiPatternSurface;
                       // dr["LAMI_COATING_SURFACE_CD"] = egt.LamiCoatingSurface;
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("1");
                       // dr["CUSTOMER_LITE_NUM"] = egt.CustomerLiteNum;
                        Rows.Add(dr);
                    }
                    if (attributes.GetDecimalAttributeValue("Z58L2TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIG_ID"] = (int) item.Id;
                        dr["LITE_NO"] = 2;
                        dr["LITE_CD"] = (attributes.GetDecimalAttributeValue("Z58L3TK") > 0) ? "MID" : "INT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L2TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L2HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L14C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L13C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L2PX");
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L2LPT");
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("2");
                        Rows.Add(dr);
                    }
                    if (attributes.GetDecimalAttributeValue("Z58L3TK") > 0)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIG_ID"] = (int) item.Id;
                        dr["LITE_NO"] = 3;
                        dr["LITE_CD"] = "INT";
                        dr["THICKNESS_NO"] = attributes.GetDecimalAttributeValue("Z58L3TK");
                        dr["HEAT_TREATMENT_CD"] = attributes.GetStringAttributeValue("Z58L3HT");
                        dr["COLOR_CD"] = attributes.GetStringAttributeDesc3("Z58L3CL", "CL");
                        dr["SURFACE1_COAT_CD"] = attributes.GetStringAttributeValue("Z58L15C");
                        dr["SURFACE2_COAT_CD"] = attributes.GetStringAttributeValue("Z58L16C");
                        dr["PATTERN_TEXTURE_CD"] = attributes.GetStringAttributeValue("Z58L3PX");
                        dr["LAMI_TYPE_CD"] = attributes.GetStringAttributeValue("Z58L3LPT");
                        dr["QUALITY_CD"] = attributes.GetStringAttributeValue("Z58L1PQL");
                        dr["IS_CUSTOMER_SUPPLIED"] = (attributes.GetStringAttributeValue("Z5840450") != null) &&
                                                     attributes.GetStringAttributeValue("Z5840450").Contains("3");
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
