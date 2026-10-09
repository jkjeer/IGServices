using System;
using Microsoft.Data.SqlClient;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class DiscreteGlassItemManager
    {
        public DiscreteGlassItemManager()
        {
        }

        /******************************************************************
        * GET ShopFloor Address Book Items
        ******************************************************************/
        public DiscreteGlassItems GetSfDiscreteGlassItems(GlassItems glassItems)
        {
            var sfItems = new DiscreteGlassItems();
            foreach (var item in glassItems)
            {
                var sfItem = new DiscreteGlassItem();
                sfItem.ItemNum = item.ItemNum.ToString();
                sfItem.PlantID = item.PlantNum;
                sfItem.StockingType = item.StockingType;
                sfItem.CategoryCode = item.GLCategory;

                sfItem.SoldCustomerNum = item.ItemAttributes.GetDecimalAttributeValue("AN8DR");
                sfItem.ShipCustomerNum = item.ItemAttributes.GetDecimalAttributeValue("SHAN");
                sfItem.OrigSoldCustomerNum = item.ItemAttributes.GetDecimalAttributeValue("Z58CTL");
                sfItem.PlannerNum = item.PlannerNum;
                sfItem.SupplierNum = item.VendorNum;

                sfItem.UnitType = item.UnitType;
                sfItem.ProductType = item.ProductType;
                sfItem.CardinalProductType = item.CardinalProductType;
                sfItem.VolumeIndicator = item.VolumeIndicator;
                sfItem.OverallThickness = item.OverallThickness;

                sfItem.Dimensions = item.Description;
                sfItem.ShapeNum = item.Shape.ShapeNum;
                sfItem.BaseL = item.Shape.BaseDimension.Base;
                sfItem.BaseN = item.Shape.BaseDimension.Numerator;
                sfItem.BaseD = item.Shape.BaseDimension.Denominator;
                sfItem.BaseMM = item.Shape.BaseDimension.MMValue;
                sfItem.BaseDec = item.Shape.BaseDimension.DecimalValue;
                sfItem.LeftL = item.Shape.LeftDimension.Base;
                sfItem.LeftN = item.Shape.LeftDimension.Numerator;
                sfItem.LeftD = item.Shape.LeftDimension.Denominator;
                sfItem.LeftMM = item.Shape.LeftDimension.MMValue;
                sfItem.LeftDec = item.Shape.LeftDimension.DecimalValue;
                sfItem.RightL = item.Shape.RightDimension.Base;
                sfItem.RightN = item.Shape.RightDimension.Numerator;
                sfItem.RightD = item.Shape.RightDimension.Denominator;
                sfItem.RightMM = item.Shape.RightDimension.MMValue;
                sfItem.RightDec = item.Shape.RightDimension.DecimalValue;
                sfItem.TopL = item.Shape.TopDimension.Base;
                sfItem.TopN = item.Shape.TopDimension.Numerator;
                sfItem.TopD = item.Shape.TopDimension.Denominator;
                sfItem.TopMM = item.Shape.TopDimension.MMValue;
                sfItem.TopDec = item.Shape.TopDimension.DecimalValue;
                sfItem.Side1L = item.Shape.Side1Dimension.Base;
                sfItem.Side1N = item.Shape.Side1Dimension.Numerator;
                sfItem.Side1D = item.Shape.Side1Dimension.Denominator;
                sfItem.Side1MM = item.Shape.Side1Dimension.MMValue;
                sfItem.Side1Dec = item.Shape.Side1Dimension.DecimalValue;
                sfItem.Side2L = item.Shape.Side2Dimension.Base;
                sfItem.Side2N = item.Shape.Side2Dimension.Numerator;
                sfItem.Side2D = item.Shape.Side2Dimension.Denominator;
                sfItem.Side2MM = item.Shape.Side2Dimension.MMValue;
                sfItem.Side2Dec = item.Shape.Side2Dimension.DecimalValue;
                sfItem.ActualSqft = item.Shape.ActualSqft;
                sfItem.BillableSqft = item.Shape.BillableSqft;

                sfItem.Construction = item.Construction;
                if (item.Lites.Count > 0)
                {
                    sfItem.Lite1_Thick = item.Lites[0].GlassType.Thickness;
                    sfItem.Lite1_Color = item.Lites[0].GlassType.Color;
                    sfItem.Lite1_Heat = item.Lites[0].GlassType.HeatTreatment;
                    sfItem.Lite1_1stSrfCoat = item.Lites[0].GlassType.Surface1Coating;
                    sfItem.Lite1_2ndSrfCoat = item.Lites[0].GlassType.Surface2Coating;
                    sfItem.Lite1_Pattern = item.Lites[0].GlassType.PatternTexture;
                    sfItem.Lite1_Lami = item.Lites[0].GlassType.LamiProductType;
                    sfItem.Lite1_Quality = item.Lites[0].QualityLevel;
                    sfItem.Lite1_AnsiiLogo = item.Logo.AnsiLogo;
                    sfItem.Lite1_TempLogo = item.Lites[0].GlassType.TempLogo;
                }

                if (item.Lites.Count > 1)
                {
                    sfItem.Lite2_Thick = item.Lites[1].GlassType.Thickness;
                    sfItem.Lite2_Color = item.Lites[1].GlassType.Color;
                    sfItem.Lite2_Heat = item.Lites[1].GlassType.HeatTreatment;
                    sfItem.Lite2_3rdSrfCoat = item.Lites[1].GlassType.Surface2Coating;
                    sfItem.Lite2_4thSrfCoat = item.Lites[1].GlassType.Surface1Coating;
                    sfItem.Lite2_Pattern = item.Lites[1].GlassType.PatternTexture;
                    sfItem.Lite2_Lami = item.Lites[1].GlassType.LamiProductType;

                    sfItem.Spacer1_Size = item.Airspaces[0].ChannelSize;
                    sfItem.Spacer1_Type = item.Airspaces[0].ChannelType;
                    sfItem.MCode1_Air1 = item.Airspaces[0].BarAssembly.MuntinBars[0].MCode;
                    sfItem.MCode1_Air1Ort = item.Airspaces[0].BarAssembly.MuntinBars[0].Orientation;
                    sfItem.MCode2_Air1 = item.Airspaces[0].BarAssembly.MuntinBars[1].MCode;
                    sfItem.MCode2_Air1Ort = item.Airspaces[0].BarAssembly.MuntinBars[1].Orientation;
                    sfItem.CCode_Air1 = item.Airspaces[0].BarAssembly.CCode;
                    sfItem.Spacer1_GasMix = item.Airspaces[0].GasMixture;
                    sfItem.Tubes = item.Airspaces[0].Tubes;
                }

                if (item.Lites.Count > 2)
                {
                    sfItem.Lite3_Thick = item.Lites[2].GlassType.Thickness;
                    sfItem.Lite3_Color = item.Lites[2].GlassType.Color;
                    sfItem.Lite3_Heat = item.Lites[2].GlassType.HeatTreatment;
                    sfItem.Lite3_5thSrfCoat = item.Lites[2].GlassType.Surface2Coating;
                    sfItem.Lite3_6thSrfCoat = item.Lites[2].GlassType.Surface1Coating;
                    sfItem.Lite3_Pattern = item.Lites[2].GlassType.PatternTexture;
                    sfItem.Lite3_Lami = item.Lites[2].GlassType.LamiProductType;

                    sfItem.Spacer2_Size = item.Airspaces[1].ChannelSize;
                    sfItem.Spacer2_Type = item.Airspaces[1].ChannelType;
                    sfItem.MCode1_Air2 = item.Airspaces[1].BarAssembly.MuntinBars[0].MCode;
                    sfItem.MCode1_Air2Ort = item.Airspaces[1].BarAssembly.MuntinBars[0].Orientation;
                    sfItem.MCode2_Air2 = item.Airspaces[1].BarAssembly.MuntinBars[1].MCode;
                    sfItem.MCode2_Air2Ort = item.Airspaces[1].BarAssembly.MuntinBars[1].Orientation;
                    sfItem.CCode_Air2 = item.Airspaces[1].BarAssembly.CCode;

                }

                sfItem.IntPrsvCB = item.InteriorSurface.PreserveCutback;
                sfItem.IntPrsvColor = item.InteriorSurface.PreserveColor;
                sfItem.ExtPrsvCB = item.ExteriorSurface.PreserveCutback;
                sfItem.ExtPrsvColor = item.ExteriorSurface.PreserveColor;
                sfItem.OverlayBar = item.InteriorSurface.CustomerOverlayBar;

                GridPattern grid = new GridPattern();
                if (item.ExteriorSurface.GridPattern != null)
                {
                    grid = item.ExteriorSurface.GridPattern;
                }
                else
                {
                    if (item.InteriorSurface.GridPattern != null)
                    {
                        grid = item.InteriorSurface.GridPattern;
                    }
                    else
                    {
                        if (item.Airspaces[0]?.GridPattern != null)
                        {
                            grid = item.Airspaces[0].GridPattern;
                        }
                        else
                        {
                            if (item.Airspaces[1]?.GridPattern != null)
                            {
                                grid = item.Airspaces[1].GridPattern;
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(grid.GridStyle))
                {
                    sfItem.GridStyle = grid.GridStyle;
                    sfItem.Grilles_Horz = grid.NumHorizontal;
                    sfItem.Grilles_Vert = grid.NumVertical;
                    sfItem.OpenHigh = grid.NumOpeningsHigh;
                    sfItem.OpenWide = grid.NumOpeningsWide;
                    sfItem.OpenOdd = grid.NumOpeningsOdd;
                }

                sfItem.RackType = item.RackType;
                sfItem.LitesPerRack = item.LitesPerRack;
                sfItem.RackRows = item.RackRows;
                sfItem.RackTiers = item.RackTiers;
                sfItem.RackOrientIO = item.RackOrientationIO;
                sfItem.RackOrientVH = item.RackOrientationVH;

                sfItem.UpdateDate = DateTime.Now;

                sfItems.Add(sfItem);
            }
            return sfItems;
        }

        /******************************************************************
        * Download 
        ******************************************************************/
        public SqlBulkCopyResponse DownloadItems(SqlConnection cn, DiscreteGlassItems items)
        {
            try
            {
                var bcMgr = new SqlBulkCopyManager();
                bcMgr.DbConnectionString = cn.ConnectionString;
                return bcMgr.BulkReader("E1DiscreteGlassItemLedger", items.GetDataReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
