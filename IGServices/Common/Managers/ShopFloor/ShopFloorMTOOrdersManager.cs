using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.ShopFloor
{
    public class ShopFloorMTOOrdersManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public SqlCommand GetProdLitesBySchedule(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT  COUNT(DISTINCT [WorkOrderNum]) AS 'RECS'
                    FROM    [ClearPathReporting].[ProdLites] cpl WITH (NOLOCK)
                    {filterString}"
            };
            return cmd;
        }

        //public bool Validate(MTOScheduleDownloadResponse response, BusinessSystemSession e1Session, SqlConnection cnCP)
        //{
        //    var isValid = true;
        //    if (ValidateSFIsProduction(response, cnCP))
        //    {
        //        isValid = false;
        //        response.Errors = ErrorManager.AddError(response.Errors, response.TransferMessage);
        //    }
        //    return isValid;
        //}

        //public bool ValidateSFIsProduction(MTOScheduleDownloadResponse response, SqlConnection cnCP)
        //{
        //    var retValue = false;
        //    var responseTimesMgr = new ResponseTimesManager();
        //    responseTimesMgr.OpenResponseTime(response.ResponseTimes, "ValidateSFIsProduction", "cnCP", cnCP.ConnectionString);
        //    var sqlResponse = new SqlDmlResponse();
        //    var prodLites = GetProdLitesCountShopFloorMTOOrdersDynamic(cnCP, response.ScheduleNum, response.MTOSchedule.ShopFloorMTOOrders, sqlResponse);
        //    if (prodLites > 0)
        //    {
        //        retValue = true;
        //        response.MTOSchedule.ValidationStatus = prodLites + " workorders on this MTO Schedule already have production records in SF";
        //        response.TransferMessage = prodLites + " workorders on this MTO Schedule already have production records in SF";
        //    }
        //    responseTimesMgr.CloseResponseTime(response.ResponseTimes, "ValidateSFIsProduction", "cnCP", cnCP.ConnectionString);
        //    return retValue;
        //}

        public int GetProdLitesCountShopFloorMTOOrdersDynamic(SqlConnection cnCP, decimal scheduleNum, ShopFloorMTOOrders shopFloorMTOOrders, SqlDmlResponse sqlResponse = null)
        {
            var prodLites = 0;
            var filters = new List<string>();
            Logger.Trace("Reader Open " + cnCP.ConnectionString);
            try
            {
                var inString = new StringBuilder();
                foreach (var line in shopFloorMTOOrders)
                {
                    inString.Append(line.WOrderNum + ",");
                }
                inString.Length -= 1;
                // Work Orders
                filters.Add("[WorkOrderNum] IN (" + inString + ") \r\n");
                var cmd = GetProdLitesBySchedule(filters);
                cmd.Connection = cnCP;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    prodLites = CastDbNull.To(reader["RECS"], 0);
                    counter++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
                sqlResponse.RowsAffected = prodLites;
                sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                sqlResponse.Name = "GetProdLitesCountShopFloorMTOOrdersDynamic";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            Logger.Trace("Reader Close " + cnCP.ConnectionString);
            return prodLites;
        }


        /******************************************************************
        * GET ORDERS
        ******************************************************************/
        public ShopFloorMTOOrders GetShopFloorMTOOrders(MTOSchedule mtoSchedule)
        {
            var orders = new ShopFloorMTOOrders();
            foreach (var line in mtoSchedule.MTOScheduleLines)
            {
                var order = new ShopFloorMTOOrder();
                try
                {
                    order.ScheduleNum = mtoSchedule.ScheduleNum;
                    order.ScheduleSeq = line.ScheduleSeqNum;
                    order.WOrderNum = line.WorkOrder.WorkOrderNum;
                    order.WOrderType = "WO";
                    order.SOrderNum = line.WorkOrder.RelatedSalesOrderLine.OrderNum;
                    order.SOrderType = line.WorkOrder.RelatedSalesOrderLine.OrderType ?? "";
                    order.SOrderLine = line.WorkOrder.RelatedSalesOrderLine.OrderLineNum;
                    order.CustNum = Convert.ToInt32(line.ShipToCustomer.CustomerNum);
                    order.CustName = line.ShipToCustomer.CustomerName ?? "";
                    order.CustPO = line.WorkOrder.RelatedSalesOrderLine.CustomerPO ?? "";
                    //order.CustPrtNum1 = ""
                    order.CustPrtNum2 = line.WorkOrder.RelatedSalesOrderLine.CustomerPart2 ?? "";
                    order.CustPrtNum3 = line.WorkOrder.RelatedSalesOrderLine.CustomerPart3 ?? "";
                    order.CustPrtNum4 = line.WorkOrder.RelatedSalesOrderLine.CustomerPart4 ?? "";
                    order.CustIGLine = line.WorkOrder.RelatedSalesOrderLine.CustomerLiteNum.ToString(CultureInfo.CurrentCulture);
                    //order.CustSchedNum = "";
                    //order.CustPL = "";
                    order.DueDate = mtoSchedule.DueDate;
                    order.ScheduleDate = mtoSchedule.ScheduleDate;
                    order.ScheduleType = mtoSchedule.ScheduleType;
                    order.Quantity = line.WorkOrder.Quantity;
                    order.Picked = (line.WorkOrder.RelatedSalesOrderLine.OrderNum == 0);
                    order.PriorityCode = line.WorkOrder.RelatedSalesOrderLine.PriorityCode ?? "";
                    //order.DockLocal = "";
                    order.CustSeq1 = line.WorkOrder.RelatedSalesOrderLine.CustomerSequenceNum;
                    //order.CustSeq2 = "";
                    order.Department = line.WorkOrder.RelatedSalesOrderLine.Reference2 ?? "";
                    //order.ShipZone = "";
                    short igLineNum;
                    short.TryParse(line.IgLine, out igLineNum);
                    order.LineID = igLineNum;
                    //order.SalesMsg1 = "";
                    //order.SalesMsg2 = "";
                    //order.SalesMsg3 = "";
                    //order.SalesMsg4 = "";
                    //order.SalesMsg5 = "";
                    //order.MateLite = 0;
                    order.ProductType = line.WorkOrder.RelatedSalesOrderLine.ProductType;
                    order.FlipGlass = (line.WorkOrder.RelatedSalesOrderLine.Flippable);
                    order.AddTubes = (line.WorkOrder.RelatedSalesOrderLine.TubesAdded);
                    order.AddPreserve = (line.WorkOrder.RelatedSalesOrderLine.PreserveAdded);
                    order.AddScoring = (line.WorkOrder.RelatedSalesOrderLine.ScoringAdded);
                    order.CrateGlass = (line.WorkOrder.RelatedSalesOrderLine.CrateAdded);

                    var glassItem = line.WorkOrder.GlassItem;

                    order.ShapeNum = glassItem.Shape.ShapeNum;
                    order.Lite1_Base = glassItem.Shape.BaseDimension.DecimalValue;
                    order.Lite1_Base_Leg = glassItem.Shape.BaseDimension.Base;
                    order.Lite1_Base_Num = glassItem.Shape.BaseDimension.Numerator;
                    order.Lite1_Base_Den = glassItem.Shape.BaseDimension.Denominator;
                    order.Lite1_Left = glassItem.Shape.LeftDimension.DecimalValue;
                    order.Lite1_Left_Leg = glassItem.Shape.LeftDimension.Base;
                    order.Lite1_Left_Num = glassItem.Shape.LeftDimension.Numerator;
                    order.Lite1_Left_Den = glassItem.Shape.LeftDimension.Denominator;
                    order.Lite1_Right = glassItem.Shape.RightDimension.DecimalValue;
                    order.Lite1_Right_Leg = glassItem.Shape.RightDimension.Base;
                    order.Lite1_Right_Num = glassItem.Shape.RightDimension.Numerator;
                    order.Lite1_Right_Den = glassItem.Shape.RightDimension.Denominator;
                    order.Lite1_Top = glassItem.Shape.TopDimension.DecimalValue;
                    order.Lite1_Top_Leg = glassItem.Shape.TopDimension.Base;
                    order.Lite1_Top_Num = glassItem.Shape.TopDimension.Numerator;
                    order.Lite1_Top_Den = glassItem.Shape.TopDimension.Denominator;
                    order.Lite1_s1 = glassItem.Shape.Side1Dimension.DecimalValue;
                    order.Lite1_S1_Leg = glassItem.Shape.Side1Dimension.Base;
                    order.Lite1_S1_Num = glassItem.Shape.Side1Dimension.Numerator;
                    order.Lite1_S1_Den = glassItem.Shape.Side1Dimension.Denominator;
                    order.Lite1_s2 = glassItem.Shape.Side2Dimension.DecimalValue;
                    order.Lite1_S2_Leg = glassItem.Shape.Side2Dimension.Base;
                    order.Lite1_S2_Num = glassItem.Shape.Side2Dimension.Numerator;
                    order.Lite1_S2_Den = glassItem.Shape.Side2Dimension.Denominator;

                    if (glassItem.Lites.Count > 0)
                    {
                        order.Lite1_Thick = glassItem.Lites[0].GlassType.Thickness;
                        order.Lite1_Heat = glassItem.Lites[0].GlassType.HeatTreatment;
                        order.Lite1_Color = glassItem.Lites[0].GlassType.Color;
                        order.Lite1_1stSrfCoat = glassItem.Lites[0].GlassType.Surface1Coating;
                        order.Lite1_2ndSrfCoat = glassItem.Lites[0].GlassType.Surface2Coating;
                        order.Lite1_Pattern = glassItem.Lites[0].GlassType.PatternTexture;
                        order.Lite1_PatTextSrf = glassItem.Lites[0].GlassType.PatternTextureSrf.ToString(CultureInfo.CurrentCulture);
                        order.Lite1_Lami = glassItem.Lites[0].GlassType.LamiProductType;
                        order.Lite1_LamiColorSrf = glassItem.Lites[0].GlassType.LamiColorSrf.ToString(CultureInfo.CurrentCulture);
                        order.Lite1_TempLogo = (glassItem.Lites[0].GlassType.TempLogo == "Y");
                        order.Lite1_ProdQuaLvl = glassItem.Lites[0].QualityLevel ?? "";
                        order.Lite1_AnsiLogo = glassItem.Logo.AnsiLogo ?? "";
                        order.Lite1_offsetX = glassItem.Logo.XOffset;
                        order.Lite1_OffsetY = glassItem.Logo.YOffset;
                        // Processing Options
                        order.Lite1_EdgeWrk = glassItem.Lites[0].EdgeWork;
                        order.Lite1_GrvType = glassItem.Lites[0].VGrooveType ?? "";
                        order.Lite1_GrvSurface = Convert.ToInt16(glassItem.Lites[0].VGrooveSurface);
                        order.Lite1_GrvStyle = glassItem.Lites[0].VGrooveStyle ?? "";
                        order.Lite1_GrvWide = Convert.ToInt16(glassItem.Lites[0].VGrooveWideOpen);
                        order.Lite1_GrvHigh = Convert.ToInt16(glassItem.Lites[0].VGrooveHighOpen);
                        order.Lite1_GrvOpen = Convert.ToInt16(glassItem.Lites[0].VGrooveTotalOpen);
                        order.Lite1_GrvDrawing = glassItem.Lites[0].VGrooveDrawing ?? "";
                        order.L1VGrooveVerticalOffset = glassItem.Lites[0].VGrooveVertOffset;
                        order.L1VGrooveHorizontalOffset = glassItem.Lites[0].VGrooveHorzOffset;
                        order.L1VGrooveVerticalStartStop = glassItem.Lites[0].VGrooveVertStartStop ?? "";
                        order.L1VGrooveHorizontalStartStop = glassItem.Lites[0].VGrooveHorzStartStop ?? "";
                        order.Lite1_Bevel = glassItem.Lites[0].VGrooveBevel ?? "";
                        order.Lite1_Holes = glassItem.Lites[0].HolesDrawing ?? "";
                        order.LiteNumHoles = Convert.ToInt16(glassItem.Lites[0].HolesNum);
                        order.Lite1_SilkSrf = Convert.ToInt16(glassItem.Lites[0].SilkScreenSurface);
                        order.Lite1_SilkDraw = glassItem.Lites[0].SilkScreenDrawing ?? "";
                        order.NotchLocation = glassItem.Lites[0].NotchLocation ?? "";
                        order.NotchHeight = glassItem.Lites[0].NotchHeight;
                        order.NotchWidth = glassItem.Lites[0].NotchWidth;
                        // Routing
                        var sbRoute1 = new StringBuilder();
                        foreach (var route in line.WorkOrder.Routings.Where(x => x.SequenceNum < 100))
                        {
                            sbRoute1.Append(route.ShopFloorCode);
                        }
                        order.Lite1_Routing = sbRoute1.ToString();
                    }

                    if (glassItem.Lites.Count > 1)
                    {
                        order.Lite2_Thick = glassItem.Lites[1].GlassType.Thickness;
                        order.Lite2_Heat = glassItem.Lites[1].GlassType.HeatTreatment;
                        order.Lite2_Color = glassItem.Lites[1].GlassType.Color;
                        order.Lite2_4thSrfCoat = glassItem.Lites[1].GlassType.Surface1Coating;
                        order.Lite2_3rdSrfCoat = glassItem.Lites[1].GlassType.Surface2Coating;
                        order.Lite2_Pattern = glassItem.Lites[1].GlassType.PatternTexture;
                        order.Lite2_PatTextSrf = Convert.ToString(glassItem.Lites[1].GlassType.PatternTextureSrf, CultureInfo.InvariantCulture);
                        order.Lite2_Lami = glassItem.Lites[1].GlassType.LamiProductType;
                        order.Lite2_LamiColorSrf = Convert.ToString(glassItem.Lites[1].GlassType.LamiColorSrf, CultureInfo.InvariantCulture);
                        // Processing Options
                        order.Lite2_EdgeWrk = glassItem.Lites[1].EdgeWork;
                        order.Lite2_GrvType = glassItem.Lites[1].VGrooveType ?? "";
                        order.Lite2_GrvSurface = Convert.ToInt16(glassItem.Lites[1].VGrooveSurface);
                        order.Lite2_GrvStyle = glassItem.Lites[1].VGrooveStyle ?? "";
                        order.Lite2_GrvWide = Convert.ToInt16(glassItem.Lites[1].VGrooveWideOpen);
                        order.Lite2_GrvHigh = Convert.ToInt16(glassItem.Lites[1].VGrooveHighOpen);
                        order.Lite2_GrvOpen = Convert.ToInt16(glassItem.Lites[1].VGrooveTotalOpen);
                        order.Lite2_GrvDrawing = glassItem.Lites[1].VGrooveDrawing ?? "";
                        order.L2VGrooveVerticalOffset = glassItem.Lites[1].VGrooveVertOffset;
                        order.L2VGrooveHorizontalOffset = glassItem.Lites[1].VGrooveHorzOffset;
                        order.L2VGrooveVerticalStartStop = glassItem.Lites[1].VGrooveVertStartStop ?? "";
                        order.L2VGrooveHorizontalStartStop = glassItem.Lites[1].VGrooveHorzStartStop ?? "";
                        order.Lite2_Bevel = glassItem.Lites[1].VGrooveBevel ?? "";
                        order.Lite2_Holes = glassItem.Lites[1].HolesDrawing ?? "";
                        order.Lite2_SilkSrf = Convert.ToInt16(glassItem.Lites[1].SilkScreenSurface);
                        order.Lite2_SilkDraw = glassItem.Lites[1].SilkScreenDrawing ?? "";

                        // Routing
                        var sbRoute2 = new StringBuilder();
                        foreach (var route in line.WorkOrder.Routings.Where(x => x.SequenceNum > 100 && x.SequenceNum < 200))
                        {
                            sbRoute2.Append(route.ShopFloorCode);
                        }
                        order.Lite2_Routing = sbRoute2.ToString();

                        // Spacer
                        //order.Spacer_Size1 = glassItem.Airspaces[0].SpacerItem.Width;
                        //order.Spacer_Type1 = glassItem.Airspaces[0].SpacerItem.SpacerType;
                        order.Spacer_GasMix1 = glassItem.Airspaces[0].GasMixture ?? "";

                        // Bars
                        order.Shades_Air1 = glassItem.Airspaces[0].Shades ?? "";
                        order.BreatherTube_Air1 = (glassItem.Airspaces[0].Tubes == "Y");
                        if (glassItem.Airspaces[0].BarAssembly != null)
                        {
                            //order.MCode1_Air1 = Convert.ToDecimal(glassItem.Airspaces[0].BarAssembly.BarItems[0].MCode ?? "");
                            //order.MCode1_Air1Ort = glassItem.Airspaces[0].BarAssembly.BarItems[0].Orientation ?? "";
                            //order.CCode_Air1 = Convert.ToDecimal(glassItem.Airspaces[0].BarAssembly.CCode ?? "");
                            //order.BumpOnType_Air1 = glassItem.Airspaces[0].BarAssembly.BumperType ?? "";
                            //order.BumpOns_Air1 = glassItem.Airspaces[0].BarAssembly.BumperQty;
                            //order.IGBumpOnConfigAir01 = glassItem.Airspaces[0].BarAssembly.BumperConfig ?? "";
                            //if (glassItem.Airspaces[0].BarAssembly.BarItems.Count > 1)
                            //{
                            //    order.MCode2_Air1 = Convert.ToDecimal(glassItem.Airspaces[0].BarAssembly.BarItems[1].MCode ?? "");
                            //    order.MCode2_Air1Ort = glassItem.Airspaces[0].BarAssembly.BarItems[1].Orientation ?? "";
                            //}
                        }
                        if (glassItem.Airspaces[0].GridPattern != null)
                        {
                            order.Grilles_HzAir1 = glassItem.Airspaces[0].GridPattern.NumHorizontal;
                            order.Grilles_VtAir1 = glassItem.Airspaces[0].GridPattern.NumVertical;
                            order.OpenWide_Air1 = glassItem.Airspaces[0].GridPattern.NumOpeningsWide;
                            order.OpenHigh_Air1 = glassItem.Airspaces[0].GridPattern.NumOpeningsHigh;
                            order.OddOpen_Air1 = glassItem.Airspaces[0].GridPattern.NumOpeningsOdd;
                        }
                    }

                    if (glassItem.Lites.Count > 2)
                    {
                        order.Lite3_Thick = glassItem.Lites[2].GlassType.Thickness;
                        order.Lite3_Heat = glassItem.Lites[2].GlassType.HeatTreatment;
                        order.Lite3_Color = glassItem.Lites[2].GlassType.Color;
                        order.Lite3_6thSrfCoat = glassItem.Lites[2].GlassType.Surface1Coating;
                        order.Lite3_5thtSrfCoat = glassItem.Lites[2].GlassType.Surface2Coating;
                        order.Lite3_Pattern = glassItem.Lites[2].GlassType.PatternTexture;
                        order.Lite3_PatTextSrf = glassItem.Lites[2].GlassType.PatternTextureSrf.ToString(CultureInfo.InvariantCulture);
                        order.Lite3_Lami = glassItem.Lites[2].GlassType.LamiProductType;
                        order.Lite3_LamiColorSrf = glassItem.Lites[2].GlassType.LamiColorSrf.ToString(CultureInfo.CurrentCulture);
                        // Processing Options
                        order.Lite3_EdgeWrk = glassItem.Lites[2].EdgeWork;
                        order.Lite3_GrvType = glassItem.Lites[2].VGrooveType ?? "";
                        order.Lite3_GrvSurface = Convert.ToInt16(glassItem.Lites[2].VGrooveSurface);
                        order.Lite3_GrvStyle = glassItem.Lites[2].VGrooveStyle ?? "";
                        order.Lite3_GrvWide = Convert.ToInt16(glassItem.Lites[2].VGrooveWideOpen);
                        order.Lite3_GrvHigh = Convert.ToInt16(glassItem.Lites[2].VGrooveHighOpen);
                        order.Lite3_GrvOpen = Convert.ToInt16(glassItem.Lites[2].VGrooveTotalOpen);
                        order.Lite3_GrvDrawing = glassItem.Lites[2].VGrooveDrawing ?? "";
                        order.L3VGrooveVerticalOffset = glassItem.Lites[2].VGrooveVertOffset;
                        order.L3VGrooveHorizontalOffset = glassItem.Lites[2].VGrooveHorzOffset;
                        order.L3VGrooveVerticalStartStop = glassItem.Lites[2].VGrooveVertStartStop ?? "";
                        order.L3VGrooveHorizontalStartStop = glassItem.Lites[2].VGrooveHorzStartStop ?? "";
                        order.Lite3_Bevel = glassItem.Lites[2].VGrooveBevel ?? "";
                        order.Lite3_Holes = glassItem.Lites[2].HolesDrawing ?? "";
                        order.Lite3_SilkSrf = Convert.ToInt16(glassItem.Lites[2].SilkScreenSurface);
                        order.Lite3_SilkDraw = glassItem.Lites[2].SilkScreenDrawing ?? "";

                        // Routing
                        var sbRoute3 = new StringBuilder();
                        foreach (var route in line.WorkOrder.Routings.Where(x => x.SequenceNum > 200 && x.SequenceNum < 300))
                        {
                            sbRoute3.Append(route.ShopFloorCode);
                        }
                        order.Lite3_Routing = sbRoute3.ToString();

                        // Spacer
                        //order.Spacer_Size2 = glassItem.Airspaces[1].SpacerItem.Width;
                        //order.Spacer_Type2 = glassItem.Airspaces[1].SpacerItem.SpacerType;

                        // Bars
                        order.Shades_Air2 = glassItem.Airspaces[1].Shades ?? "";
                        order.BreatherTube_Air2 = (glassItem.Airspaces[1].Tubes == "Y");
                        if (glassItem.Airspaces[1].BarAssembly != null)
                        {
                            //order.MCode1_Air2 = Convert.ToDecimal(glassItem.Airspaces[1].BarAssembly.BarItems[0].MCode ?? "");
                            //order.MCode1_Air2Ort = glassItem.Airspaces[1].BarAssembly.BarItems[0].Orientation ?? "";
                            //order.CCode_Air2 = Convert.ToDecimal(glassItem.Airspaces[1].BarAssembly.CCode ?? "");
                            //order.BumpOnType_Air2 = glassItem.Airspaces[1].BarAssembly.BumperType ?? "";
                            //order.BumpOns_Air2 = glassItem.Airspaces[1].BarAssembly.BumperQty;
                            //order.IGBumpOnConfigAir02 = glassItem.Airspaces[1].BarAssembly.BumperConfig ?? "";
                            //if (glassItem.Airspaces[1].BarAssembly.BarItems.Count > 1)
                            //{
                            //    order.MCode2_Air2 = Convert.ToDecimal(glassItem.Airspaces[1].BarAssembly.BarItems[1].MCode ?? "");
                            //    order.MCode2_Air2Ort = glassItem.Airspaces[1].BarAssembly.BarItems[1].Orientation ?? "";
                            //}
                        }
                        if (glassItem.Airspaces[1].GridPattern != null)
                        {
                            order.Grilles_HzAir2 = glassItem.Airspaces[1].GridPattern.NumHorizontal;
                            order.Grilles_VtAir2 = glassItem.Airspaces[1].GridPattern.NumVertical;
                            order.OpenWide_Air2 = glassItem.Airspaces[1].GridPattern.NumOpeningsWide;
                            order.OpenHigh_Air2 = glassItem.Airspaces[1].GridPattern.NumOpeningsHigh;
                            order.OddOpen_Air2 = glassItem.Airspaces[1].GridPattern.NumOpeningsOdd;
                        }
                    }

                    order.NFRC = glassItem.Logo.NfrcLogo1 ?? "";
                    order.NFRC_HztOffset = glassItem.Logo.NfrcHorizontalOffset;
                    order.NFRC_VrtOffset = glassItem.Logo.NfrcVerticalOffset;
                    order.NarwMargCode = glassItem.NarrowMarginCode ?? "";
                    order.VertSashIntru = glassItem.VerticalSashIntrusion;
                    order.HorzSashIntru = glassItem.HorizontalSashIntrusion;

                    order.ExtPrsvCB = glassItem.ExteriorSurface.PreserveCutback ?? "";
                    order.ExtPrsvColor = glassItem.ExteriorSurface.PreserveColor ?? "";
                    order.ExtPrsvScoring = glassItem.ExteriorSurface.ScoringWidth;
                    order.ExtOpacifier = glassItem.ExteriorSurface.Opacifier ?? "";
                    order.IntPrsvCB = glassItem.InteriorSurface.PreserveCutback ?? "";
                    order.IntPrsvColor = glassItem.InteriorSurface.PreserveColor ?? "";
                    order.IntPrsvScoring = glassItem.InteriorSurface.ScoringWidth;
                    order.IntOpacifier = glassItem.InteriorSurface.Opacifier ?? "";

                    // Grid Pattern attributes
                    var haveGrid = false;
                    var gridPattern = new GridPattern();
                    if (glassItem.Airspaces.Count > 0 && glassItem.Airspaces[0].GridPattern != null)
                    {
                        gridPattern = glassItem.Airspaces[0].GridPattern;
                        haveGrid = true;
                    }
                    if (glassItem.ExteriorSurface.GridPattern != null)
                    {
                        gridPattern = glassItem.ExteriorSurface.GridPattern;
                        haveGrid = true;
                    }
                    if (glassItem.InteriorSurface.GridPattern != null)
                    {
                        gridPattern = glassItem.InteriorSurface.GridPattern;
                        haveGrid = true;
                    }

                    if (haveGrid)
                    {
                        order.GridStyle_Spacer1 = gridPattern.GridStyle ?? "";
                        order.IGSubGridStyle = gridPattern.SubGridStyle ?? "";
                        order.DrawingID = gridPattern.Drawing ?? "";
                        order.CustomerOverlayBar = gridPattern.OverlayBar ?? "";
                        order.PlacementCode = gridPattern.PlacementType ?? "";
                        order.PrairieBarHOffset = gridPattern.HorizontalPrairieOffset;
                        order.PrairieBarVOffset = gridPattern.VerticalPrairieOffset;

                        foreach (var placement in gridPattern.VerticalBarPlacements)
                        {
                            switch (placement.OrdinalNum)
                            {
                                case 1:
                                    order.VertPos1 = placement.Position;
                                    break;
                                case 2:
                                    order.VertPos2 = placement.Position;
                                    break;
                                case 3:
                                    order.VertPos3 = placement.Position;
                                    break;
                                case 4:
                                    order.VertPos4 = placement.Position;
                                    break;
                                case 5:
                                    order.VertPos5 = placement.Position;
                                    break;
                                case 6:
                                    order.VertPos6 = placement.Position;
                                    break;
                                case 7:
                                    order.VertPos7 = placement.Position;
                                    break;
                                case 8:
                                    order.VertPos8 = placement.Position;
                                    break;
                                case 9:
                                    order.VertPos9 = placement.Position;
                                    break;
                                case 10:
                                    order.VertPos10 = placement.Position;
                                    break;
                                case 11:
                                    order.VertPos11 = placement.Position;
                                    break;
                                case 12:
                                    order.VertPos12 = placement.Position;
                                    break;
                                case 13:
                                    order.VertPos13 = placement.Position;
                                    break;
                                case 14:
                                    order.VertPos14 = placement.Position;
                                    break;
                                case 15:
                                    order.VertPos15 = placement.Position;
                                    break;
                                case 16:
                                    order.VertPos16 = placement.Position;
                                    break;
                                case 17:
                                    order.VertPos17 = placement.Position;
                                    break;
                                case 18:
                                    order.VertPos18 = placement.Position;
                                    break;
                                case 19:
                                    order.VertPos19 = placement.Position;
                                    break;
                                case 20:
                                    order.VertPos20 = placement.Position;
                                    break;
                            }
                        }

                        foreach (var placement in gridPattern.HorizontalBarPlacements)
                        {
                            switch (placement.OrdinalNum)
                            {
                                case 1:
                                    order.HorzPos1 = placement.Position;
                                    break;
                                case 2:
                                    order.HorzPos2 = placement.Position;
                                    break;
                                case 3:
                                    order.HorzPos3 = placement.Position;
                                    break;
                                case 4:
                                    order.HorzPos4 = placement.Position;
                                    break;
                                case 5:
                                    order.HorzPos5 = placement.Position;
                                    break;
                                case 6:
                                    order.HorzPos6 = placement.Position;
                                    break;
                                case 7:
                                    order.HorzPos7 = placement.Position;
                                    break;
                                case 8:
                                    order.HorzPos8 = placement.Position;
                                    break;
                                case 9:
                                    order.HorzPos9 = placement.Position;
                                    break;
                                case 10:
                                    order.HorzPos10 = placement.Position;
                                    break;
                                case 11:
                                    order.HorzPos11 = placement.Position;
                                    break;
                                case 12:
                                    order.HorzPos12 = placement.Position;
                                    break;
                                case 13:
                                    order.HorzPos13 = placement.Position;
                                    break;
                                case 14:
                                    order.HorzPos14 = placement.Position;
                                    break;
                                case 15:
                                    order.HorzPos15 = placement.Position;
                                    break;
                                case 16:
                                    order.HorzPos16 = placement.Position;
                                    break;
                                case 17:
                                    order.HorzPos17 = placement.Position;
                                    break;
                                case 18:
                                    order.HorzPos18 = placement.Position;
                                    break;
                                case 19:
                                    order.HorzPos19 = placement.Position;
                                    break;
                                case 20:
                                    order.HorzPos20 = placement.Position;
                                    break;
                            }
                        }
                    }

                    order.LogoNum = glassItem.Logo.IgLogo;
                    order.SchedulerMsg1 = line.SchedulerMsg1;
                    order.SchedulerMsg2 = line.SchedulerMsg2;
                    order.Comment1 = line.Comment1;
                    order.Comment2 = line.Comment2;

                    short boxPriority;
                    short.TryParse(line.BoxPriority, out boxPriority);
                    order.BoxPriority = boxPriority;

                    order.Bungee = line.Bungee;
                    order.Bungee_Size = line.BungeeSize;
                    order.MakeupLegacy = "";
                    order.OddDate = "";
                    order.CardinalProductType = glassItem.CardinalProductType ?? "";
                    order.CustomerDate = line.ReleaseDate;
                    order.OriginalBranchPlant = mtoSchedule.PlantNum;
                    order.TotalWeight = glassItem.TotalWeight;

                    if (line.WorkOrder.RelatedSalesOrderLine.EdiMessages != null)
                    {
                        var labelMessages = line.WorkOrder.RelatedSalesOrderLine.EdiMessages.Where(msg => msg.EdiType == "L").ToList();
                        var index = 1;
                        foreach (var v in labelMessages)
                        {
                            switch (Convert.ToInt16(index))
                            {
                                case 1:
                                    order.CustMsg1 = v.Message;
                                    break;
                                case 2:
                                    order.CustMsg2 = v.Message;
                                    break;
                                case 3:
                                    order.CustMsg3 = v.Message;
                                    break;
                                case 4:
                                    order.CustMsg4 = v.Message;
                                    break;
                                case 5:
                                    order.CustMsg5 = v.Message;
                                    break;
                            }
                            index += 1;
                        }
                        var etchMessages = line.WorkOrder.RelatedSalesOrderLine.EdiMessages.Where(msg => msg.EdiType == "E").ToList();
                        index = 1;
                        foreach (var v in etchMessages)
                        {
                            switch (Convert.ToInt16(index))
                            {
                                case 1:
                                    order.CustEtchingMsg1 = v.Message;
                                    break;
                                case 2:
                                    order.CustEtchingMsg2 = v.Message;
                                    break;
                                case 3:
                                    order.CustEtchingMsg3 = v.Message;
                                    break;
                                case 4:
                                    order.CustEtchingMsg4 = v.Message;
                                    break;
                                case 5:
                                    order.CustEtchingMsg5 = v.Message;
                                    break;
                            }
                            index += 1;
                        }
                    }
                    order.QCInspect = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801840");
                    order.PickingItemNumber = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5899033"); // S8ITM
                    order.BasePickingLocation = ""; // S8PPIK
                    order.Airspace1Tape = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5840470");
                    order.EnergyPanel = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801630");
                    order.ShipBoxNumber = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5851965");
                    order.ShipMethod = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5851966");
                    order.Lite1LamiMixedTreatment = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801865");
                    order.Lite1LamiColorSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801890");
                    order.Lite1LamiPatternSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801870");
                    order.Lite1LamiCoatingSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801880");
                    order.Lite2LamiMixedTreatment = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5803485");
                    order.Lite2LamiColorSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5803510");
                    order.Lite2LamiPatternSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5803490");
                    order.Lite2LamiCoatingSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5803500");
                    order.Lite3LamiMixedTreatment = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5805075");
                    order.Lite3LamiColorSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5805100");
                    order.Lite3LamiPatternSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5805080");
                    order.Lite3LamiCoatingSurface = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5805090");
                    order.ButylColor = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801990");
                    order.SiliconeColor = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5802000");
                    order.IGCCCertificationFlag = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5840435");
                    order.IGCaps = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5801635", "");
                    order.CornerClipping = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5800550");
                    order.EndItemPickItem = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5880230"); // S8Z5880230
                    order.IQLite1DiscretePart = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5851551"); // S8Z5851551
                    order.IQLite2DiscretePart = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5851552"); // S8Z5851552
                    order.IQLite3DiscretePart = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5851553"); // S8Z5851553
                    order.Lite1RawPickLocation = ""; // S8Z58L1RLO
                    order.Lite2RawPickLocation = ""; // S8Z58L2RLO
                    order.Lite3RawPickLocation = ""; // S8Z58L3RLO

                    var ediLabelMessages = line.WorkOrder.RelatedSalesOrderLine.EdiMessages.Where(msg => msg.EdiType == "L").ToList();
                    foreach (var v in ediLabelMessages)
                    {
                        switch (Convert.ToInt16(v.EdiSeqNum))
                        {
                            case 1:
                                order.EdiLabelMsg1 = v.Message;
                                break;
                            case 2:
                                order.EdiLabelMsg2 = v.Message;
                                break;
                            case 3:
                                order.EdiLabelMsg3 = v.Message;
                                break;
                            case 4:
                                order.EdiLabelMsg4 = v.Message;
                                break;
                            case 5:
                                order.EdiLabelMsg5 = v.Message;
                                break;
                            case 6:
                                order.EdiLabelMsg6 = v.Message;
                                break;
                            case 7:
                                order.EdiLabelMsg7 = v.Message;
                                break;
                            case 8:
                                order.EdiLabelMsg8 = v.Message;
                                break;
                            case 9:
                                order.EdiLabelMsg9 = v.Message;
                                break;
                            case 10:
                                order.EdiLabelMsg10 = v.Message;
                                break;
                        }
                    }

                    var ediEtchMessages = line.WorkOrder.RelatedSalesOrderLine.EdiMessages.Where(msg => msg.EdiType == "E").ToList();
                    foreach (var v in ediEtchMessages)
                    {
                        switch (Convert.ToInt16(v.EdiSeqNum))
                        {
                            case 1:
                                order.EdiEtchMsg1 = v.Message;
                                break;
                            case 2:
                                order.EdiEtchMsg2 = v.Message;
                                break;
                            case 3:
                                order.EdiEtchMsg3 = v.Message;
                                break;
                            case 4:
                                order.EdiEtchMsg4 = v.Message;
                                break;
                            case 5:
                                order.EdiEtchMsg5 = v.Message;
                                break;
                            case 6:
                                order.EdiEtchMsg6 = v.Message;
                                break;
                            case 7:
                                order.EdiEtchMsg7 = v.Message;
                                break;
                            case 8:
                                order.EdiEtchMsg8 = v.Message;
                                break;
                            case 9:
                                order.EdiEtchMsg9 = v.Message;
                                break;
                            case 10:
                                order.EdiEtchMsg10 = v.Message;
                                break;
                        }
                    }

                    order.CrateId = line.WorkOrder.RelatedSalesOrderLine.Crate?.CrateId ?? 0; // CRZ58CRID
                    order.CrateType = line.WorkOrder.RelatedSalesOrderLine.Crate?.CrateType ?? ""; // CRZ58CRTY
                    order.CustBillToNum = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("AN8DR"); // S7AN8DR
                    order.GenericSeqNum = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z58GSQNO"); // S7Z58GSQNO
                    order.RequestedDeliveryTime = 0; // S8DRQT
                    order.BatchName = ""; // S8Z58BDES
                    order.BoxingOption = ""; // S8Z58CCV01
                    order.RunOption = ""; // S8Z58CCV02
                    order.CustomerShipTo = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z5850785"); // S7SHAN
                    order.Crate = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z58CRATE"); // S7Z58CRATE
                    order.MailingName = ""; // S8MLNM
                    order.OrigCustNumSoldTo = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z58CTL");
                    order.NFRC2 = ""; // S8Z58NFRC2
                    order.PickingNotes = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5899011"); // S8Z5899011
                    order.PackingNotes = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5899012"); // S8Z5899012
                    order.SCHED_VALUE03 = ""; // S8Z58CCV03
                    order.SCHED_VALUE04 = ""; // S8Z58CCV04
                    order.QCInspectFlag = ""; //
                    order.EdgeDeleteWidth = line.WorkOrder.GlassItem.ItemAttributes.GetStringAttributeValue("Z5805680", ""); // S8Z5805680
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.ToString());
                }
                orders.Add(order);
            }
            return orders;
        }

        /******************************************************************
        * BULK ORDERS
        ******************************************************************/
        public SqlBulkCopyResponse BulkMTOOrders(SqlConnection cn, ShopFloorMTOOrders mtoOrders)
        {
            var bcMgr = new SqlBulkCopyManager
            {
                DbConnectionString = cn.ConnectionString
            };
            return bcMgr.BulkReader("ShopFloorMTOOrdersLedger", mtoOrders.GetDataReader());
        }

        //public MTOScheduleDownloadResponse DownloadSchedule(MTOScheduleDownloadResponse response, SqlConnection cnSFInt)
        //{
        //    var bcMgr = new SqlBulkCopyManager { DbConnectionString = cnSFInt.ConnectionString };
        //    var incomingOrderLines = response.MTOSchedule.ShopFloorMTOOrders;
        //    IDataReader dr = incomingOrderLines.GetDataReader();
        //    if (response.Write)
        //    {
        //        var sqlBulkCopyResponse = bcMgr.BulkReader("ShopFloorMTOOrdersLedger", dr);
        //        if (response.SqlBulkCopyResponses == null) response.SqlBulkCopyResponses = new SqlBulkCopyResponses();
        //        response.SqlBulkCopyResponses.Add(sqlBulkCopyResponse);
        //        if (sqlBulkCopyResponse.HasErrors)
        //        {
        //            response.Transferred = false;
        //            response.TransferStatus = "E";
        //            response.TransferMessage = sqlBulkCopyResponse.Message;
        //            response.Errors = ErrorManager.AddError(response.Errors, sqlBulkCopyResponse.Message);
        //        }
        //        else
        //        {
        //            response.Transferred = true;
        //            response.TransferStatus = "Y";
        //        }
        //        response.TransferDate = DateTime.Now.ToLocalTime();
        //    }
        //    return response;
        //}
    }
}

