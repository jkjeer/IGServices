using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class MTOIncomingOrderManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        //private readonly E1.SalesLineAttributeManager _salesLineAttributeManager;

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public static MTOIncomingOrderLines BuildMTOIncomingOrderLines(MTOSchedule mtoSchedule, BusinessSystemSession e1Session)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"Plant: {mtoSchedule.E1ScheduleData.F580020.S6MCU}  Sched: {mtoSchedule.E1ScheduleData.F580020.S6Z58SCN}  BEGIN: {DateTime.Now.ToLocalTime()}");
            var incomingOrderLines = new MTOIncomingOrderLines();
            var _salesLineAttributeManager = new E1.SalesLineAttributeManager(e1Session);
            var _itemAttributesManager = new E1.ItemAttributeManager(e1Session);
            try
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var f580021 in mtoSchedule.E1ScheduleData.F580021s)
                {
                    var f580024 = mtoSchedule.E1ScheduleData.F580024s?.Find(item => item.S8EDBT == f580021.S7EDBT && item.S8EDLN == f580021.S7EDLN && item.S8Z58SCN == f580021.S7Z58SCN && item.S8Z58SCSN == f580021.S7Z58SCSN);
                    Logger.Info($"Line {f580021.S7Z58SCSN} F580024 found: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                    if (f580024 != null)
                    {
                        var f584715L = mtoSchedule.E1ScheduleData.F584715Ls?.Find(item => item.ZDEDBT == f580021.S7EDBT && item.ZDEDLN == f580021.S7EDLN && item.ZDZ58SCN == f580021.S7Z58SCN && item.ZDZ58SCSN == f580021.S7Z58SCSN) ?? new F584715();
                        Logger.Debug($"Line {f580021.S7Z58SCSN} F584715L found: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        var f584715E = mtoSchedule.E1ScheduleData.F584715Es?.Find(item => item.ZDEDBT == f580021.S7EDBT && item.ZDEDLN == f580021.S7EDLN && item.ZDZ58SCN == f580021.S7Z58SCN && item.ZDZ58SCSN == f580021.S7Z58SCSN) ?? new F584715();
                        Logger.Debug($"Line {f580021.S7Z58SCSN} F584715E found: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        var s7Rorn = int.TryParse(f580021.S7RORN, out Tmp<int>.Value) ? Tmp<int>.Value : 0;
                        var f580220 = mtoSchedule.E1ScheduleData.F580220s?.Find(item => (int)item.CRDOCO == s7Rorn && item.CRDCTO == f580021.S7RCTO && item.CRLNID == f580021.S7LNID) ?? new F580220();
                        Logger.Debug($"Line {f580021.S7Z58SCSN} F580220 found: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        var f4715Pivot = mtoSchedule.E1ScheduleData.F4715Pivots?.Find(item => (int)item.ZDDOCO == s7Rorn && item.ZDDCTO == f580021.S7RCTO && item.ZDLNID == f580021.S7LNID) ?? new F4715Pivot();
                        Logger.Debug($"Line {f580021.S7Z58SCSN} F4715Pivot found: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        var incomingOrderLine = new MTOIncomingOrderLine
                        {
                            PlantNum = f580021.S7MCU,
                            ScheduleNum = f580021.S7Z58SCN,
                            ScheduleSeq = f580021.S7Z58SCSN,
                            WOrderNum = f580021.S7DOCO,
                            WOrderType = f580021.S7DCTO,
                            SOrderNum = Convert.ToDecimal(f580021.S7RORN),
                            SOrderLine = f580021.S7LNID / 1000,
                            SOrderType = f580021.S7RCTO,
                            CustNum = Convert.ToInt32(f580021.S7SHAN),
                            CustName = f580021.S7ALPH,
                            CustPO = f580021.S7VR01,
                            CustPrtNum1 = f580021.S7CITM,
                            CustPrtNum2 = f580021.S7Z58CP2,
                            CustPrtNum3 = f580021.S7Z58CP3,
                            CustPrtNum4 = f580021.S7Z58CP4,
                            CustIGLine = Convert.ToString(f580021.S7Z58IGCLN, CultureInfo.CurrentCulture),
                            CustSchedNum = f580021.S7Z58CSCNO,
                            CustPL = Convert.ToString(f580021.S7LINN, CultureInfo.CurrentCulture),
                            DueDate = Utilities.Common.CastToDateTime(f580021.S7Z58SCDDJ),
                            ScheduleDate = Utilities.Common.CastToDateTime(f580021.S7Z58SCD),
                            ScheduleType = f580021.S7Z58SCTYP,
                            Quantity = f580021.S7Z58QTY,
                            Picked = f580021.S7RORN.Equals("0"),
                            PriorityCode = f580024.S8URCD,
                            Department = f580021.S7VR02,
                            LineID = int.TryParse(f580021.S7Z58IGLN, out Tmp<int>.Value) ? Tmp<int>.Value : 0,
                            SalesMsg1 = f580021.S7Z58GAL1,
                            SalesMsg2 = f580021.S7Z58GAL2,
                            SalesMsg3 = "",
                            SalesMsg4 = "",
                            SalesMsg5 = "",
                            FlipGlass = false,
                            AddTubes = f580021.S7Z58TBAD.Equals("Y"),
                            AddPreserve = false,
                            AddScoring = false,
                            CrateGlass = !f580021.S7Z58CRATE.Equals("N"),
                            Lite1_Routing = f580021.S7Z58L1RG,
                            Lite1_Thick = f580021.S7Z58L1TK / 10,
                            Lite1_Color = f580021.S7Z58L1CL,
                            Lite1_Heat = f580021.S7Z58L1HT,
                            Lite1_1stSrfCoat = f580021.S7Z58L11C,
                            Lite1_2ndSrfCoat = f580021.S7Z58L12C,
                            Lite1_Pattern = f580021.S7Z58L1PX,
                            Lite1_Lami = f580021.S7Z58L1LPT,
                            Lite1_Base = f580021.S7Z581BDEC / 1000,
                            Lite1_Left = f580021.S7Z581LDEC / 1000,
                            Lite1_Right = f580021.S7Z581RDEC / 1000,
                            Lite1_Top = f580021.S7Z581TDEC / 1000,
                            Lite1_s1 = f580021.S7Z5811DEC / 1000,
                            Lite1_s2 = f580021.S7Z5812DEC / 1000,
                            Lite1_GrvType = f580021.S7Z58L1VG,
                            Lite1_GrvSurface = Convert.ToInt32(f580021.S7Z58L1VGS),
                            Lite1_GrvStyle = f580021.S7Z58L1VGT,
                            Lite1_GrvWide = Convert.ToInt32(f580021.S7Z58L1VGW),
                            Lite1_GrvHigh = Convert.ToInt32(f580021.S7Z58L1VGH),
                            Lite1_GrvOpen = Convert.ToInt32(f580021.S7Z58L1VGO),
                            Lite1_GrvDrawing = f580021.S7Z58L1VGD,
                            Lite1_Bevel = f580021.S7Z58L1BL.Length > 2 ? f580021.S7Z58L1BL.Substring(0, 2) : f580021.S7Z58L1BL,
                            Lite1_Holes = f580021.S7Z58L1HL,
                            LiteNumHoles = 0,
                            Lite1_Silk = "",
                            Lite1_SilkSrf = Convert.ToInt32(f580021.S7Z58L1SF),
                            Lite1_SilkDraw = f580021.S7Z58L1SD,
                            Lite1_EdgeWrk = f580021.S7Z58L1EW,
                            Lite1_ProdQuaLvl = f580021.S7Z58L1PQL,
                            Lite1_TempLogo = f580021.S7Z58L1TL.Equals("Y"),
                            Lite1_AnsiLogo = f580021.S7Z58L1ALT,
                            Lite1_offsetX = f580021.S7Z58L1LX / 1000,
                            Lite1_OffsetY = f580021.S7Z58L1LY / 1000,
                            Lite1_CusLiteNum = Convert.ToInt32(f580021.S7Z58L1CLN),
                            Lite1_PatTextSrf = Convert.ToString(f580021.S7Z58L1PTS, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580021.S7Z58L1PTS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580021.S7Z58L1PTS, CultureInfo.CurrentCulture),
                            Lite1_LamiColorSrf = Convert.ToString(f580021.S7Z58L1LCS, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580021.S7Z58L1LCS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580021.S7Z58L1LCS, CultureInfo.CurrentCulture),
                            Spacer_Size1 = f580024.S8Z58L1SSZ / 10,
                            Spacer_Type1 = f580024.S8Z58L1STP,
                            Spacer_GasMix1 = f580024.S8Z58S1GM,
                            Lite2_Thick = f580021.S7Z58L2TK / 10,
                            Lite2_Color = f580021.S7Z58L2CL,
                            Lite2_Heat = f580021.S7Z58L2HT,
                            Lite2_3rdSrfCoat = f580021.S7Z58L23C,
                            Lite2_4thSrfCoat = f580021.S7Z58L24C,
                            Lite2_Pattern = f580021.S7Z58L2PX,
                            Lite2_Lami = f580021.S7Z58L2LPT,
                            Lite2_Base = f580021.S7Z582BDEC / 1000,
                            Lite2_Left = f580021.S7Z582LDEC / 1000,
                            Lite2_Right = f580021.S7Z582RDEC / 1000,
                            Lite2_Top = f580021.S7Z582TDEC / 1000,
                            Lite2_s1 = f580021.S7Z5821DEC / 1000,
                            Lite2_s2 = f580021.S7Z5822DEC / 1000,
                            Lite2_GrvType = f580021.S7Z58L2VG,
                            Lite2_GrvSurface = Convert.ToInt32(f580021.S7Z58L2VGS),
                            Lite2_GrvStyle = f580021.S7Z58L2VGT,
                            Lite2_GrvWide = Convert.ToInt32(f580021.S7Z58L2VGW),
                            Lite2_GrvHigh = Convert.ToInt32(f580021.S7Z58L2VGH),
                            Lite2_GrvOpen = Convert.ToInt32(f580021.S7Z58L2VGO),
                            Lite2_GrvDrawing = f580021.S7Z58L2VGD,
                            Lite2_Bevel = f580021.S7Z58L2BL.Length > 2 ? f580021.S7Z58L2BL.Substring(0, 2) : f580021.S7Z58L2BL,
                            Lite2_Holes = f580021.S7Z58L2HL,
                            Lite2_Silk = "",
                            Lite2_SilkSrf = Convert.ToInt32(f580021.S7Z58L2SF),
                            Lite2_SilkDraw = f580021.S7Z58L2SD,
                            Lite2_EdgeWrk = f580021.S7Z58L2EW,
                            Lite2_TempLogo = f580021.S7Z58L2TL.Equals("Y"),
                            Lite2_PatTextSrf = Convert.ToString(f580021.S7Z58L2PTS, CultureInfo.CurrentCulture).Length > 1 ? Convert.ToString(f580021.S7Z58L2PTS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580021.S7Z58L2PTS, CultureInfo.CurrentCulture),
                            Lite2_LamiColorSrf = Convert.ToString(f580021.S7Z58L2LCS, CultureInfo.CurrentCulture).Length > 1 ? Convert.ToString(f580021.S7Z58L2LCS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580021.S7Z58L2LCS, CultureInfo.CurrentCulture),
                            Spacer_Size2 = f580024.S8Z58L2SSZ / 10,
                            Spacer_Type2 = f580024.S8Z58L2STP,
                            Lite2_Routing = f580021.S7Z58L2RG,
                            Lite3_Thick = f580021.S7Z58L3TK / 10,
                            Lite3_Color = f580021.S7Z58L3CL,
                            Lite3_Heat = f580021.S7Z58L3HT,
                            Lite3_5thtSrfCoat = f580021.S7Z58L35C,
                            Lite3_6thSrfCoat = f580021.S7Z58L36C,
                            Lite3_Pattern = f580021.S7Z58L3PX,
                            Lite3_Lami = f580021.S7Z58L3LPT,
                            Lite3_Base = f580021.S7Z583BDEC / 1000,
                            Lite3_Left = f580021.S7Z583LDEC / 1000,
                            Lite3_Right = f580021.S7Z583RDEC / 1000,
                            Lite3_Top = f580021.S7Z583TDEC / 1000,
                            Lite3_s1 = f580021.S7Z5831DEC / 1000,
                            Lite3_s2 = f580021.S7Z5832DEC / 1000,
                            Lite3_GrvType = f580021.S7Z58L3VG,
                            Lite3_GrvSurface = Convert.ToInt32(f580021.S7Z58L3VGS),
                            Lite3_GrvStyle = f580021.S7Z58L3VGT,
                            Lite3_GrvWide = Convert.ToInt32(f580021.S7Z58L3VGW),
                            Lite3_GrvHigh = Convert.ToInt32(f580021.S7Z58L3VGH),
                            Lite3_GrvOpen = Convert.ToInt32(f580021.S7Z58L3VGO),
                            Lite3_GrvDrawing = f580021.S7Z58L3VGD,
                            Lite3_Bevel = Convert.ToString(f580021.S7Z58L3BL, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580021.S7Z58L3BL, CultureInfo.CurrentCulture).Substring(0, 2) : Convert.ToString(f580021.S7Z58L3BL, CultureInfo.CurrentCulture),
                            Lite3_Holes = f580021.S7Z58L3HL,
                            Lite3_Silk = "",
                            Lite3_SilkSrf = Convert.ToInt32(f580021.S7Z58L3SF),
                            Lite3_SilkDraw = f580021.S7Z58L3SD,
                            Lite3_EdgeWrk = f580024.S8Z58L3EW,
                            Lite3_TempLogo = f580024.S8Z58L3TL.Equals("Y"),
                            Lite3_PatTextSrf = Convert.ToString(f580024.S8Z58L3PTS, CultureInfo.CurrentCulture).Length > 1 ? Convert.ToString(f580024.S8Z58L3PTS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580024.S8Z58L3PTS, CultureInfo.CurrentCulture),
                            Lite3_LamiColorSrf = Convert.ToString(f580024.S8Z58L3LCS, CultureInfo.CurrentCulture).Length > 1 ? Convert.ToString(f580024.S8Z58L3LCS, CultureInfo.CurrentCulture).Substring(0, 1) : Convert.ToString(f580024.S8Z58L3LCS, CultureInfo.CurrentCulture),
                            Lite3_Routing = f580021.S7Z58L3RG,
                            MCode1_Air1 = f580024.S8Z58M1A1,
                            MCode1_Air1Ort = f580024.S8Z58M1A1O,
                            MCode2_Air1 = f580024.S8Z58M2A1,
                            MCode2_Air1Ort = Convert.ToString(f580024.S8Z58M2A1O, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580024.S8Z58M2A1O, CultureInfo.CurrentCulture).Substring(0, 2) : Convert.ToString(f580024.S8Z58M2A1O, CultureInfo.CurrentCulture),
                            Grilles_HzAir1 = f580024.S8Z58GHA1,
                            Grilles_VtAir1 = f580024.S8Z58GVA1,
                            OpenWide_Air1 = f580024.S8Z58OWA1,
                            OpenHigh_Air1 = f580024.S8Z58OHA1,
                            OddOpen_Air1 = f580024.S8Z58OOA1,
                            CCode_Air1 = f580024.S8Z58CCA1,
                            BumpOnType_Air1 = f580024.S8Z58BA1,
                            BumpOns_Air1 = f580024.S8Z58NA1,
                            Shades_Air1 = f580024.S8Z58SA1,
                            BreatherTube_Air1 = f580024.S8Z58TA1.Equals("Y"),
                            GridStyle_Spacer1 = f580024.S8Z58GSS1,
                            PrairieBar = (f580024.S8Z585555 / 1000).ToString("0.000", CultureInfo.CurrentCulture).Length > 5 ? (f580024.S8Z585555 / 1000).ToString("0.000", CultureInfo.CurrentCulture).Substring(0, 5) : (f580024.S8Z585555 / 1000).ToString("0.000", CultureInfo.CurrentCulture),
                            NFRC = f580024.S8Z58NFRC,
                            NFRC_VrtOffset = f580024.S8Z58NFRCV / 1000,
                            NFRC_HztOffset = f580024.S8Z58NFRCH / 1000,
                            NarwMargCode = f580024.S8Z58IGNMC,
                            VertSashIntru = f580024.S8Z58IGSIV / 1000,
                            HorzSashIntru = f580024.S8Z58IGSIH / 1000,
                            MCode1_Air2 = f580024.S8Z58M1A2,
                            MCode1_Air2Ort = Convert.ToString(f580024.S8Z58M1A2O, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580024.S8Z58M1A2O, CultureInfo.CurrentCulture).Substring(0, 2) : Convert.ToString(f580024.S8Z58M1A2O, CultureInfo.CurrentCulture),
                            MCode2_Air2 = f580024.S8Z58M2A2,
                            MCode2_Air2Ort = Convert.ToString(f580024.S8Z58M2A2O, CultureInfo.CurrentCulture).Length > 2 ? Convert.ToString(f580024.S8Z58M2A2O, CultureInfo.CurrentCulture).Substring(0, 2) : Convert.ToString(f580024.S8Z58M2A2O, CultureInfo.CurrentCulture),
                            Grilles_HzAir2 = f580024.S8Z58GHA2,
                            Grilles_VtAir2 = f580024.S8Z58GVA2,
                            OpenWide_Air2 = f580024.S8Z58OWA2,
                            OpenHigh_Air2 = f580024.S8Z58OHA2,
                            OddOpen_Air2 = f580024.S8Z58OOA2,
                            CCode_Air2 = f580024.S8Z58CCA2,
                            BumpOnType_Air2 = f580024.S8Z58BA2,
                            BumpOns_Air2 = f580024.S8Z58NA2,
                            Shades_Air2 = f580024.S8Z58SA2,
                            BreatherTube_Air2 = f580024.S8Z58TA2.Equals("Y"),
                            PlacementCode = f580024.S8Z58IPC,
                            VertPos1 = f580024.S8Z58IGVP1 / 1000,
                            VertPos2 = f580024.S8Z58IGVP2 / 1000,
                            VertPos3 = f580024.S8Z58IGVP3 / 1000,
                            VertPos4 = f580024.S8Z58IGVP4 / 1000,
                            VertPos5 = f580024.S8Z58IGVP5 / 1000,
                            VertPos6 = f580024.S8Z58IGVP6 / 1000,
                            VertPos7 = f580024.S8Z58IGVP7 / 1000,
                            VertPos8 = f580024.S8Z58IGVP8 / 1000,
                            VertPos9 = f580024.S8Z58IGVP9 / 1000,
                            VertPos10 = f580024.S8Z58IGV10 / 1000,
                            VertPos11 = f580024.S8Z58IGV11 / 1000,
                            VertPos12 = f580024.S8Z58IGV12 / 1000,
                            VertPos13 = f580024.S8Z58IGV13 / 1000,
                            VertPos14 = f580024.S8Z58IGV14 / 1000,
                            VertPos15 = f580024.S8Z58IGV15 / 1000,
                            VertPos16 = f580024.S8Z58IGV16 / 1000,
                            VertPos17 = f580024.S8Z58IGV17 / 1000,
                            VertPos18 = f580024.S8Z58IGV18 / 1000,
                            VertPos19 = f580024.S8Z58IGV19 / 1000,
                            VertPos20 = f580024.S8Z58IGV20 / 1000,
                            HorzPos1 = f580024.S8Z58IGHP1 / 1000,
                            HorzPos2 = f580024.S8Z58IGHP2 / 1000,
                            HorzPos3 = f580024.S8Z58IGHP3 / 1000,
                            HorzPos4 = f580024.S8Z58IGHP4 / 1000,
                            HorzPos5 = f580024.S8Z58IGHP5 / 1000,
                            HorzPos6 = f580024.S8Z58IGHP6 / 1000,
                            HorzPos7 = f580024.S8Z58IGHP7 / 1000,
                            HorzPos8 = f580024.S8Z58IGHP8 / 1000,
                            HorzPos9 = f580024.S8Z58IGHP9 / 1000,
                            HorzPos10 = f580024.S8Z58IGH10 / 1000,
                            HorzPos11 = f580024.S8Z58IGH11 / 1000,
                            HorzPos12 = f580024.S8Z58IGH12 / 1000,
                            HorzPos13 = f580024.S8Z58IGH13 / 1000,
                            HorzPos14 = f580024.S8Z58IGH14 / 1000,
                            HorzPos15 = f580024.S8Z58IGH15 / 1000,
                            HorzPos16 = f580024.S8Z58IGH16 / 1000,
                            HorzPos17 = f580024.S8Z58IGH17 / 1000,
                            HorzPos18 = f580024.S8Z58IGH18 / 1000,
                            HorzPos19 = f580024.S8Z58IGH19 / 1000,
                            HorzPos20 = f580024.S8Z58IGH20 / 1000,
                            ShapeNum = int.TryParse(f580021.S7Z58SHAPE, out Tmp<int>.Value) ? Tmp<int>.Value : 0,
                            ExtPrsvCB = f580024.S8Z587563,
                            ExtPrsvColor = f580024.S8Z587566,
                            ExtOpacifier = f580024.S8Z587572,
                            ExtPrsvScoring = f580024.S8Z587569,
                            IntPrsvCB = f580024.S8Z587575,
                            IntPrsvColor = f580024.S8Z587578,
                            IntOpacifier = f580024.S8Z587584,
                            IntPrsvScoring = f580024.S8Z587581,
                            LogoNum = f580024.S8Z5840430,
                            SchedulerMsg1 = f580021.S7Z58MSG1,
                            SchedulerMsg2 = f580021.S7Z58MSG2,
                            BoxPriority = int.TryParse(f580021.S7Z58BOXP, out Tmp<int>.Value) ? Tmp<int>.Value : 0,
                            Bungee = f580021.S7Z58BGEE,
                            Bungee_Size = f580021.S7Z58BGSZ,
                            DrawingID = f580021.S7Z58DRAW,
                            MateLite = Convert.ToDecimal(0),
                            ProductType = f580021.S7Z58PT ?? "",
                            Lite1_Base_Leg = f580021.S7Z58L1BDM,
                            Lite1_Base_Num = f580021.S7Z58L1BNM,
                            Lite1_Base_Den = f580021.S7Z58L1BDN,
                            Lite1_Left_Leg = f580021.S7Z58L1LDM,
                            Lite1_Left_Num = f580021.S7Z58L1LNM,
                            Lite1_Left_Den = f580021.S7Z58L1LDN,
                            Lite1_Right_Leg = f580021.S7Z58L1RDM,
                            Lite1_Right_Num = f580021.S7Z58L1RNM,
                            Lite1_Right_Den = f580021.S7Z58L1RDN,
                            Lite1_Top_Leg = f580021.S7Z58L1TDM,
                            Lite1_Top_Num = f580021.S7Z58L1TNM,
                            Lite1_Top_Den = f580021.S7Z58L1TDN,
                            Lite1_S1_Leg = f580021.S7Z58L11DM,
                            Lite1_S1_Num = f580021.S7Z58L11NM,
                            Lite1_S1_Den = f580021.S7Z58L11DN,
                            Lite1_S2_Leg = f580021.S7Z58L12DM,
                            Lite1_S2_Num = f580021.S7Z58L12NM,
                            Lite1_S2_Den = f580021.S7Z58L12DN,
                            CustSeq1 = f580021.S7Z58CSQNO,
                            PrairieBarVOffset = f580024.S8Z585555 / 1000,
                            PrairieBarHOffset = f580024.S8Z5805560 / 1000,
                            CardinalProductType = f580024.S8Z5840310, //f580024.S8URRF,
                            CustomerDate = Utilities.Common.CastToDateTime(f580021.S7RSDJ),
                            E1BatchID = f580021.S7EDBT,
                            CustomerOverlayBar = f580024.S8Z5805735,
                            NotchLocation = f580024.S8Z5801480,
                            NotchHeight = f580024.S8Z5801481 / 1000,
                            NotchWidth = f580024.S8Z5801482 / 1000,
                            Comment1 = f580021.S7Z58CMT1,
                            Comment2 = f580021.S7Z58CMT2,
                            DockLocal = "",
                            CustSeq2 = "",
                            ShipZone = "",
                            MakeupLegacy = "",
                            OddDate = "",
                            DownloadDate = DateTime.Now.ToLocalTime(),
                            OrigCustNumSoldTo = f580021.S7Z58CTL,
                            TotalWeight = f580024.S8Z58GLWT / 1000,
                            CustomerSuppliedGlass = f580024.S8Z5840450 ?? "",
                            ReasonCode = f580024.S8RCD,
                            OriginalBranchPlant = f580024.S8Z5840415,
                            QCInspectFlag = f580024.S8Z5801840 ?? "",
                            EdgeDeleteWidth = f580024.S8Z5805680,
                            IGSubGridStyle = f580024.S8Z5805570,
                            IGBumpOnConfigAir01 = f580024.S8Z5805470,
                            IGBumpOnConfigAir02 = f580024.S8Z5806070,
                            L1VGrooveVerticalOffset = f580024.S8Z5801295,
                            L1VGrooveHorizontalOffset = f580024.S8Z5801300,
                            L1VGrooveVerticalStartStop = f580024.S8Z5801296,
                            L1VGrooveHorizontalStartStop = f580024.S8Z5801301,
                            L2VGrooveVerticalOffset = f580024.S8Z5802960,
                            L2VGrooveHorizontalOffset = f580024.S8Z5802965,
                            L2VGrooveVerticalStartStop = f580024.S8Z5802961,
                            L2VGrooveHorizontalStartStop = f580024.S8Z5802966,
                            L3VGrooveVerticalOffset = f580024.S8Z5804545,
                            L3VGrooveHorizontalOffset = f580024.S8Z5804550,
                            L3VGrooveVerticalStartStop = f580024.S8Z5804546,
                            L3VGrooveHorizontalStartStop = f580024.S8Z5804551,
                            CustMsg1 = f584715L.ZDZ58TX01 ?? "",
                            CustMsg2 = f584715L.ZDZ58TX02 ?? "",
                            CustMsg3 = f584715L.ZDZ58TX03 ?? "",
                            CustMsg4 = f584715L.ZDZ58TX04 ?? "",
                            CustMsg5 = f584715L.ZDZ58TX05 ?? "",
                            CustEtchingMsg1 = f584715E.ZDZ58TX01 ?? "",
                            CustEtchingMsg2 = f584715E.ZDZ58TX02 ?? "",
                            CustEtchingMsg3 = f584715E.ZDZ58TX03 ?? "",
                            CustEtchingMsg4 = f584715E.ZDZ58TX04 ?? "",
                            CustEtchingMsg5 = f584715E.ZDZ58TX05 ?? "",
                            QCInspect = f580024.S8Z5801840 ?? "",
                            PickingItemNumber = f580024.S8ITM,
                            BasePickingLocation = f580024.S8PPIK,
                            Airspace1Tape = f580024.S8Z5840470,
                            EnergyPanel = f580024.S8Z5801630,
                            Lite1LamiMixedTreatment = f580024.S8Z5801865,
                            Lite1LamiColorSurface = f580024.S8Z5801890,
                            Lite1LamiPatternSurface = f580024.S8Z58L1LPS,
                            Lite1LamiCoatingSurface = f580024.S8Z58L1LXS,
                            Lite1LamiThickness1 = f580024.S8Z5801905 / 10,
                            Lite1LamiInnerLayerThickness = f580024.S8Z5801906 / 1000,
                            Lite1LamiThickness2 = f580024.S8Z5801907 / 10,
                            Lite2LamiMixedTreatment = f580024.S8Z5803485,
                            Lite2LamiColorSurface = f580024.S8Z5803510,
                            Lite2LamiPatternSurface = f580024.S8Z58L2LPS,
                            Lite2LamiCoatingSurface = f580024.S8Z58L2LXS,
                            Lite2LamiThickness1 = f580024.S8Z5803525 / 10,
                            Lite2LamiInnerLayerThickness = f580024.S8Z5803526 / 1000,
                            Lite2LamiThickness2 = f580024.S8Z5803527 / 10,
                            Lite3LamiMixedTreatment = f580024.S8Z5805075,
                            Lite3LamiColorSurface = f580024.S8Z5805100,
                            Lite3LamiPatternSurface = f580024.S8Z58L3LPS,
                            Lite3LamiCoatingSurface = f580024.S8Z58L3LXS,
                            Lite3LamiThickness1 = f580024.S8Z5805115 / 10,
                            Lite3LamiInnerLayerThickness = f580024.S8Z5805116 / 1000,
                            Lite3LamiThickness2 = f580024.S8Z5805117 / 10,
                            ButylColor = f580024.S8Z5801990,
                            SiliconeColor = f580024.S8Z5802000,
                            IGCCCertificationFlag = f580024.S8Z5840435,
                            IGCaps = f580024.S8Z5801635,
                            CornerClipping = f580024.S8Z5800550,
                            EndItemPickItem = f580024.S8Z5880230,
                            IQLite1DiscretePart = f580024.S8Z5851551,
                            IQLite2DiscretePart = f580024.S8Z5851552,
                            IQLite3DiscretePart = f580024.S8Z5851553,
                            Lite1RawPickLocation = f580024.S8Z58L1RLO,
                            Lite2RawPickLocation = f580024.S8Z58L2RLO,
                            Lite3RawPickLocation = f580024.S8Z58L3RLO,
                            EdiLabelMsg1 = f4715Pivot.L1 ?? "",
                            EdiLabelMsg2 = f4715Pivot.L2 ?? "",
                            EdiLabelMsg3 = f4715Pivot.L3 ?? "",
                            EdiLabelMsg4 = f4715Pivot.L4 ?? "",
                            EdiLabelMsg5 = f4715Pivot.L5 ?? "",
                            EdiLabelMsg6 = f4715Pivot.L6 ?? "",
                            EdiLabelMsg7 = f4715Pivot.L7 ?? "",
                            EdiLabelMsg8 = f4715Pivot.L8 ?? "",
                            EdiLabelMsg9 = f4715Pivot.L9 ?? "",
                            EdiLabelMsg10 = f4715Pivot.L10 ?? "",
                            EdiLabelMsg11 = f4715Pivot.L11 ?? "",
                            EdiLabelMsg12 = f4715Pivot.L12 ?? "",
                            EdiLabelMsg13 = f4715Pivot.L13 ?? "",
                            EdiLabelMsg14 = f4715Pivot.L14 ?? "",
                            EdiLabelMsg15 = f4715Pivot.L15 ?? "",
                            EdiLabelMsg16 = f4715Pivot.L16 ?? "",
                            EdiLabelMsg17 = f4715Pivot.L17 ?? "",
                            EdiLabelMsg18 = f4715Pivot.L18 ?? "",
                            EdiLabelMsg19 = f4715Pivot.L19 ?? "",
                            EdiLabelMsg20 = f4715Pivot.L20 ?? "",
                            EdiEtchMsg1 = f4715Pivot.E1 ?? "",
                            EdiEtchMsg2 = f4715Pivot.E2 ?? "",
                            EdiEtchMsg3 = f4715Pivot.E3 ?? "",
                            EdiEtchMsg4 = f4715Pivot.E4 ?? "",
                            EdiEtchMsg5 = f4715Pivot.E5 ?? "",
                            EdiEtchMsg6 = f4715Pivot.E6 ?? "",
                            EdiEtchMsg7 = f4715Pivot.E7 ?? "",
                            EdiEtchMsg8 = f4715Pivot.E8 ?? "",
                            EdiEtchMsg9 = f4715Pivot.E9 ?? "",
                            EdiEtchMsg10 = f4715Pivot.E10 ?? "",
                            CrateId = f580220.CRZ58CRID,
                            CrateType = f580220.CRZ58CRTY ?? "",
                            CustBillToNum = f580021.S7AN8DR,
                            GenericSeqNum = f580021.S7Z58GSQNO,
                            RequestedDeliveryTime = (int) f580024.S8DRQT,
                            BatchName = f580024.S8Z58BDES,
                            BoxingOption = f580024.S8Z58CCV01,
                            RunOption = f580024.S8Z58CCV02,
                            CustomerShipTo = f580021.S7SHAN,
                            Crate = f580021.S7Z58CRATE,
                            Lite1_NumHoles = f580021.S7Z581365,
                            Lite2_NumHoles = f580021.S7Z5803005,
                            Lite3_NumHoles = f580021.S7Z5804595,
                            MailingName = f580024.S8MLNM,
                            NFRC2 = f580024.S8Z58NFRC2,
                            PickingNotes = f580024.S8Z5899011,
                            PackingNotes = f580024.S8Z5899012,
                            SCHED_VALUE03 = f580024.S8Z58CCV03,
                            SCHED_VALUE04 = f580024.S8Z58CCV04,
                            SCHED_VALUE05 = f580024.S8Z58CCV05,
                            SCHED_VALUE06 = f580024.S8Z58CCV06,
                            SCHED_VALUE07 = f580024.S8Z58CCV07,
                            SCHED_VALUE08 = f580024.S8Z58CCV08,
                            SCHED_VALUE09 = f580024.S8Z58CCV09,
                            SCHED_VALUE10 = f580024.S8Z58CCV10,
                            ConfigID = f580021.CMCFGID,
                            FabDrawing = f580024.S8Z5888100,
                            PLSide = f580024.S8Z5853001,
                            FabDesc = f580024.S8Z58FABDE,
                            SqftActual = f580024.S8Z58USF / 10000,
                            EdgeWorkSides = f580024.S8Z5853002,
                            ActualIGOverallThickness = f580024.S8Z5850865,
                            IGUnitOffsetAmount = f580024.S8Z5887001 / 1000,
                            IGUnitShapeOffsetSide = f580024.S8Z5887002,
                            TaglessLogo = f580024.S8Z5801621,
                            Lite1_Inventory = f580021.S7Z58L1QOH,
                            Lite2_Inventory = f580021.S7Z58L2QOH,
                            Lite3_Inventory = f580021.S7Z58L3QOH,
                            Lite1_ReleaseNote = f580021.S7Z58FLRN,
                            Lite1_ReleaseTimeNote = f580021.S7Z58FLTN,
                            Lite2_ReleaseNote = f580021.S7Z58SLRN,
                            Lite2_ReleaseTimeNote = f580021.S7Z58SLTN,
                            Lite3_ReleaseNote = f580021.S7Z58TLRN,
                            Lite3_ReleaseTimeNote = f580021.S7Z58TLTN,
                            ChangedNote = f580021.S7Z58CHGN,
                            RemakeFlag = f580024.S8TRNT,
                            VerticalBarOffset = f580024.S8Z5805562 / 1000,
                            HorizontalBarOffset = f580024.S8Z5805563 / 1000,
                            Lite1Plant = f580024.S8Z5852060,
                            Lite2Plant = f580024.S8Z5852061,
                            Lite3Plant = f580024.S8Z5852062,
                            Lite1Weight = f580024.S8Z5887004 / 1000,
                            Lite2Weight = f580024.S8Z5887005 / 1000,
                            Lite3Weight = f580024.S8Z5887006 / 1000,
                            //OrigCustomerPO = 
                            ShipCompleteId = f580024.S8Z58IR01,

                            DeliveryInstruction1 = f580021.SHDEL1,
                            DeliveryInstruction2 = f580021.SHDEL2
                        };
                        Logger.Debug($"Line {f580021.S7Z58SCSN} built: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");

                        //if (f580021.S7MCU == "19" && f580021.S7RCTO == "ST" && f580021.S7SHAN == 10084969) // WXIG => LLCG
                        //{
                            // Sales Line Attributes
                            //var originalSalesLineAttribute = _salesLineAttributeManager.GetOriginalSalesLineAttribute(e1Session, Convert.ToDecimal(f580021.S7RORN), f580021.S7RCTO, f580021.S7LNID);
                            //incomingOrderLine.OrigCustomerPO = originalSalesLineAttribute.OrigCustomerPO;

                            // Original Sales Line Attributes
                            var originalSalesLineAttribute = mtoSchedule.OriginalSalesLineAttributes?.Find(item => item.WorkOrderNum == f580021.S7DOCO);
                            if (originalSalesLineAttribute != null)
                            {
                                incomingOrderLine.OrigCustomerPO = originalSalesLineAttribute.OrigCustomerPO;
                            }

                        incomingOrderLine.ItemAttributes = _itemAttributesManager.GetConfiguredItemAttributes(e1Session, f580021.CMCFGID);

                        incomingOrderLine.CustomBarAirspace1 = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5885189");
                        incomingOrderLine.CustomBarAirspace2 = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5885190");

                        incomingOrderLine.PressureAdjusted = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5840210");

                        incomingOrderLine.Lite1_SandblastDraw = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5801900");
                        incomingOrderLine.Lite2_SandblastDraw = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5803520");
                        incomingOrderLine.Lite3_SandblastDraw = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5805110");

                        incomingOrderLine.Lite1LamiCustom = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5800512");
                        incomingOrderLine.Lite2LamiCustom = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5802192");
                        incomingOrderLine.Lite3LamiCustom = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5803782");
                        incomingOrderLine.VigCapLocation = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5860043");
                        incomingOrderLine.VigCapType = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5860044");
                        incomingOrderLine.WaterJet = incomingOrderLine.ItemAttributes.GetStringAttributeValue("Z5860003");

                        Logger.Debug($"Sales line attributes added: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                        //}
                        incomingOrderLines.Add(incomingOrderLine);
                    }
                    else
                    {
                        throw new ApplicationException("Missing F580024 at BuildMTOIncomingOrderLines");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return incomingOrderLines;
        }

        public SqlBulkCopyResponse DownloadSchedule(SqlConnection cnSFInt, MTOIncomingOrderLines mtoIncomingOrderLines, bool write)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            const string tableName = "MTOIncomingOrdersLedger";
            SqlBulkCopyResponse sqlBulkCopyResponse;
            try
            {
                var triggerManager = new TriggerManager();
                var triggers = triggerManager.GetTriggers(cnSFInt, tableName); 
                if (!write)
                {
                    triggerManager.SetTriggers(cnSFInt, tableName, false);
                }
                var bcMgr = new SqlBulkCopyManager
                {
                    DbConnectionString = cnSFInt.ConnectionString
                };
                IDataReader dr = mtoIncomingOrderLines.GetDataReader();
                sqlBulkCopyResponse = bcMgr.BulkReader(tableName, dr);
                if (!write)
                {
                    triggerManager.ResetTriggers(cnSFInt, triggers);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return sqlBulkCopyResponse;
        }

        public int CountDownloadedWorkOrders(SqlConnection cnSFInt, decimal scheduleNum, DateTime scheduleDate, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT  COUNT(*) AS 'RowCount'
                        FROM    ( SELECT DISTINCT
                                            [ScheduleNum]
                                           ,[ScheduleDate]
                                           ,[ScheduleSeq]
                                           ,[WOrderNum]
                                           ,[WOrderType]
                                           ,ROW_NUMBER() OVER (PARTITION BY [ScheduleNum], [ScheduleSeq] ORDER BY [DownloadDate] DESC) AS 'RowNumber'
                                  FROM      [EBS].[MTOIncomingOrdersLedger]
                                  WHERE     [ScheduleNum] = @ScheduleNum
                                        AND [ScheduleDate] = @ScheduleDate
                                ) x WHERE [RowNumber] = 1"
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Connection = cnSFInt;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rowCount = CastDbNull.To(reader["RowCount"], 0);
                }
                reader.Close();
                Logger.Trace($@"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }

        public static SqlDmlResponse DeleteIncomingOrders(SqlConnection cnSF, decimal scheduleNum, DateTime scheduleDate)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new SqlDmlResponse();
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                        DELETE
                        FROM    [dbo].[IncomingOrders]
                        WHERE   [ScheduleNum] = @ScheduleNum
                            AND [ScheduleDate] = @ScheduleDate
                        "
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Connection = cnSF;
                response = new SqlDmlResponse();
                response.Init(cmd);
                Logger.Debug(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($@"ROWS: {rowCount}");
                response.RowsAffected = rowCount;
                response.EndTime = DateTime.Now.ToLocalTime();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error(e));
                response.Message = e.Message;
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }
    }
}
