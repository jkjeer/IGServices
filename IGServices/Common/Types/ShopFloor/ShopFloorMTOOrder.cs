using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class ShopFloorMTOOrders : List<ShopFloorMTOOrder>
    {
        public DataTable GetDataTable()
        {
            var dt = new DataTable("Orders");

            var propertyInfos = typeof(ShopFloorMTOOrder).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            }
            foreach (var order in this)
            {
                var dr = dt.NewRow();
                foreach (var propertyInfo in propertyInfos)
                {
                    dr[propertyInfo.Name] = propertyInfo.GetValue(order, null) ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }

    [DataContract(Namespace = "")]
    public class ShopFloorMTOOrder
    {
        public ShopFloorMTOOrder()
        {
            ScheduleNum = 0;
            ScheduleSeq = 0;
            WOrderNum = 0;
            WOrderType = "";
            SOrderNum = 0;
            SOrderType = "";
            SOrderLine = 0;
            CustNum = 0;
            CustName = "";
            CustPO = "";
            CustPrtNum1 = "";
            CustPrtNum2 = "";
            CustPrtNum3 = "";
            CustPrtNum4 = "";
            CustIGLine = "0";
            CustSchedNum = "";
            CustPL = "0";
            DueDate = new DateTime(1900, 1, 1);
            ScheduleDate = new DateTime(1900, 1, 1);
            ScheduleType = "";
            Quantity = 0;
            Picked = false;
            PriorityCode = "";
            DockLocal = "";
            CustSeq1 = "";
            CustSeq2 = "";
            Department = "";
            ShipZone = "";
            LineID = 0;
            SalesMsg1 = "";
            SalesMsg2 = "";
            SalesMsg3 = "";
            SalesMsg4 = "";
            SalesMsg5 = "";
            MateLite = 0;
            ProductType = "";
            FlipGlass = false;
            AddTubes = false;
            AddPreserve = false;
            AddScoring = false;
            CrateGlass = false;
            Lite1_Routing = "";
            Lite1_Thick = 0;
            Lite1_Color = "";
            Lite1_Heat = "";
            Lite1_1stSrfCoat = "";
            Lite1_2ndSrfCoat = "";
            Lite1_Pattern = "";
            Lite1_Lami = "";
            Lite1_Base = 0;
            Lite1_Left = 0;
            Lite1_Right = 0;
            Lite1_Top = 0;
            Lite1_s1 = 0;
            Lite1_s2 = 0;
            Lite1_Base_Leg = 0;
            Lite1_Base_Num = 0;
            Lite1_Base_Den = 0;
            Lite1_Left_Leg = 0;
            Lite1_Left_Num = 0;
            Lite1_Left_Den = 0;
            Lite1_Right_Leg = 0;
            Lite1_Right_Num = 0;
            Lite1_Right_Den = 0;
            Lite1_Top_Leg = 0;
            Lite1_Top_Num = 0;
            Lite1_Top_Den = 0;
            Lite1_S1_Leg = 0;
            Lite1_S1_Num = 0;
            Lite1_S1_Den = 0;
            Lite1_S2_Leg = 0;
            Lite1_S2_Num = 0;
            Lite1_S2_Den = 0;
            Lite1_GrvType = "";
            Lite1_GrvSurface = 0;
            Lite1_GrvStyle = "";
            Lite1_GrvWide = 0;
            Lite1_GrvHigh = 0;
            Lite1_GrvOpen = 0;
            Lite1_GrvDrawing = "";
            Lite1_Bevel = "";
            Lite1_Holes = "";
            LiteNumHoles = 0;
            Lite1_Silk = "";
            Lite1_SilkSrf = 0;
            Lite1_SilkDraw = "";
            Lite1_EdgeWrk = "";
            Lite1_ProdQuaLvl = "";
            Lite1_TempLogo = false;
            Lite1_AnsiLogo = "";
            Lite1_offsetX = 0;
            Lite1_OffsetY = 0;
            Lite1_CusLiteNum = 0;
            Lite1_PatTextSrf = "0";
            Lite1_LamiColorSrf = "0";
            Spacer_Size1 = 0;
            Spacer_Type1 = "";
            Spacer_GasMix1 = "";
            Lite2_Thick = 0;
            Lite2_Color = "";
            Lite2_Heat = "";
            Lite2_3rdSrfCoat = "";
            Lite2_4thSrfCoat = "";
            Lite2_Pattern = "";
            Lite2_Lami = "";
            Lite2_Base = 0;
            Lite2_Left = 0;
            Lite2_Right = 0;
            Lite2_Top = 0;
            Lite2_s1 = 0;
            Lite2_s2 = 0;
            Lite2_GrvType = "";
            Lite2_GrvSurface = 0;
            Lite2_GrvStyle = "";
            Lite2_GrvWide = 0;
            Lite2_GrvHigh = 0;
            Lite2_GrvOpen = 0;
            Lite2_GrvDrawing = "";
            Lite2_Bevel = "";
            Lite2_Holes = "";
            Lite2_Silk = "";
            Lite2_SilkSrf = 0;
            Lite2_SilkDraw = "";
            Lite2_EdgeWrk = "";
            Lite2_TempLogo = false;
            Lite2_PatTextSrf = "0";
            Lite2_LamiColorSrf = "0";
            Spacer_Size2 = 0;
            Spacer_Type2 = "";
            Lite2_Routing = "";
            Lite3_Thick = 0;
            Lite3_Color = "";
            Lite3_Heat = "";
            Lite3_5thtSrfCoat = "";
            Lite3_6thSrfCoat = "";
            Lite3_Pattern = "";
            Lite3_Lami = "";
            Lite3_Base = 0;
            Lite3_Left = 0;
            Lite3_Right = 0;
            Lite3_Top = 0;
            Lite3_s1 = 0;
            Lite3_s2 = 0;
            Lite3_GrvType = "";
            Lite3_GrvSurface = 0;
            Lite3_GrvStyle = "";
            Lite3_GrvWide = 0;
            Lite3_GrvHigh = 0;
            Lite3_GrvOpen = 0;
            Lite3_GrvDrawing = "";
            Lite3_Bevel = "";
            Lite3_Holes = "";
            Lite3_Silk = "";
            Lite3_SilkSrf = 0;
            Lite3_SilkDraw = "";
            Lite3_EdgeWrk = "";
            Lite3_TempLogo = false;
            Lite3_PatTextSrf = "0";
            Lite3_LamiColorSrf = "0";
            Lite3_Routing = "";
            MCode1_Air1 = 0;
            MCode1_Air1Ort = "";
            MCode2_Air1 = 0;
            MCode2_Air1Ort = "";
            Grilles_HzAir1 = 0;
            Grilles_VtAir1 = 0;
            OpenWide_Air1 = 0;
            OpenHigh_Air1 = 0;
            OddOpen_Air1 = 0;
            CCode_Air1 = 0;
            BumpOnType_Air1 = "";
            BumpOns_Air1 = 0;
            Shades_Air1 = "";
            BreatherTube_Air1 = false;
            GridStyle_Spacer1 = "";
            PrairieBar = "0.000";
            NFRC = "";
            NFRC_VrtOffset = 0;
            NFRC_HztOffset = 0;
            NarwMargCode = "";
            VertSashIntru = 0;
            HorzSashIntru = 0;
            MCode1_Air2 = 0;
            MCode1_Air2Ort = "";
            MCode2_Air2 = 0;
            MCode2_Air2Ort = "";
            Grilles_HzAir2 = 0;
            Grilles_VtAir2 = 0;
            OpenWide_Air2 = 0;
            OpenHigh_Air2 = 0;
            OddOpen_Air2 = 0;
            CCode_Air2 = 0;
            BumpOnType_Air2 = "";
            BumpOns_Air2 = 0;
            Shades_Air2 = "";
            BreatherTube_Air2 = false;
            PlacementCode = "";
            VertPos1 = 0;
            VertPos2 = 0;
            VertPos3 = 0;
            VertPos4 = 0;
            VertPos5 = 0;
            VertPos6 = 0;
            VertPos7 = 0;
            VertPos8 = 0;
            VertPos9 = 0;
            VertPos10 = 0;
            VertPos11 = 0;
            VertPos12 = 0;
            VertPos13 = 0;
            VertPos14 = 0;
            VertPos15 = 0;
            VertPos16 = 0;
            VertPos17 = 0;
            VertPos18 = 0;
            VertPos19 = 0;
            VertPos20 = 0;
            HorzPos1 = 0;
            HorzPos2 = 0;
            HorzPos3 = 0;
            HorzPos4 = 0;
            HorzPos5 = 0;
            HorzPos6 = 0;
            HorzPos7 = 0;
            HorzPos8 = 0;
            HorzPos9 = 0;
            HorzPos10 = 0;
            HorzPos11 = 0;
            HorzPos12 = 0;
            HorzPos13 = 0;
            HorzPos14 = 0;
            HorzPos15 = 0;
            HorzPos16 = 0;
            HorzPos17 = 0;
            HorzPos18 = 0;
            HorzPos19 = 0;
            HorzPos20 = 0;
            ShapeNum = 0;
            ExtPrsvCB = "";
            ExtPrsvColor = "";
            ExtOpacifier = "";
            ExtPrsvScoring = 0;
            IntPrsvCB = "";
            IntPrsvColor = "";
            IntOpacifier = "";
            IntPrsvScoring = 0;
            LogoNum = "";
            SchedulerMsg1 = "";
            SchedulerMsg2 = "";
            BoxPriority = 0;
            Bungee = "";
            Bungee_Size = 0;
            MakeupLegacy = "";
            OddDate = "";
            DrawingID = "";
            CustMsg1 = "";
            CustMsg2 = "";
            CustMsg3 = "";
            CustMsg4 = "";
            CustMsg5 = "";
            CustEtchingMsg1 = "";
            CustEtchingMsg2 = "";
            CustEtchingMsg3 = "";
            CustEtchingMsg4 = "";
            CustEtchingMsg5 = "";
            QCInspect = "";
            PickingItemNumber = 0;
            BasePickingLocation = "";
            Airspace1Tape = "";
            EnergyPanel = "";
            ShipBoxNumber = "";
            ShipMethod = "";
            Lite1LamiMixedTreatment = "";
            Lite1LamiColorSurface = "";
            Lite1LamiPatternSurface = "";
            Lite1LamiCoatingSurface = "";
            Lite2LamiMixedTreatment = "";
            Lite2LamiColorSurface = "";
            Lite2LamiPatternSurface = "";
            Lite2LamiCoatingSurface = "";
            Lite3LamiMixedTreatment = "";
            Lite3LamiColorSurface = "";
            Lite3LamiPatternSurface = "";
            Lite3LamiCoatingSurface = "";
            ButylColor = "";
            SiliconeColor = "";
            IGCCCertificationFlag = "";
            IGCaps = "";
            CornerClipping = "";
            EndItemPickItem = 0;
            IQLite1DiscretePart = 0;
            IQLite2DiscretePart = 0;
            IQLite3DiscretePart = 0;
            Lite1RawPickLocation = "";
            Lite2RawPickLocation = "";
            Lite3RawPickLocation = "";
            EdiLabelMsg1 = "";
            EdiLabelMsg2 = "";
            EdiLabelMsg3 = "";
            EdiLabelMsg4 = "";
            EdiLabelMsg5 = "";
            EdiLabelMsg6 = "";
            EdiLabelMsg7 = "";
            EdiLabelMsg8 = "";
            EdiLabelMsg9 = "";
            EdiLabelMsg10 = "";
            EdiEtchMsg1 = "";
            EdiEtchMsg2 = "";
            EdiEtchMsg3 = "";
            EdiEtchMsg4 = "";
            EdiEtchMsg5 = "";
            EdiEtchMsg6 = "";
            EdiEtchMsg7 = "";
            EdiEtchMsg8 = "";
            EdiEtchMsg9 = "";
            EdiEtchMsg10 = "";
            CrateId = 0;
            CrateType = "";
            CustBillToNum = 0;
            GenericSeqNum = "";
            RequestedDeliveryTime = 0;
            BatchName = "";
            BoxingOption = "";
            RunOption = "";
            CustomerShipTo = 0;
            Crate = "";
            MailingName = "";
            NFRC2 = "";
            PickingNotes = "";
            PackingNotes = "";
            SCHED_VALUE03 = "";
            SCHED_VALUE04 = "";
            QCInspectFlag = "";
            EdgeDeleteWidth = "";

            PrairieBarHOffset = 0;
            PrairieBarVOffset = 0;
            CardinalProductType = "";
            CustomerDate = null;
            E1BatchID = "";
            CustomerOverlayBar = "";
            NotchLocation = "";
            NotchHeight = 0;
            NotchWidth = 0;
            Comment1 = "";
            Comment2 = "";
            DownloadDate = DateTime.Now;
            IGSubGridStyle = "";
            IGBumpOnConfigAir01 = "";
            IGBumpOnConfigAir02 = "";
            L1VGrooveVerticalOffset = 0;
            L1VGrooveHorizontalOffset = 0;
            L1VGrooveVerticalStartStop = "";
            L1VGrooveHorizontalStartStop = "";
            L2VGrooveVerticalOffset = 0;
            L2VGrooveHorizontalOffset = 0;
            L2VGrooveVerticalStartStop = "";
            L2VGrooveHorizontalStartStop = "";
            L3VGrooveVerticalOffset = 0;
            L3VGrooveHorizontalOffset = 0;
            L3VGrooveVerticalStartStop = "";
            L3VGrooveHorizontalStartStop = "";
            OrigCustNumSoldTo = 0;
            TotalWeight = 0;
            CustomerSuppliedGlass = "";
            ReasonCode = "";
            OriginalBranchPlant = "";
        }

        [DataMember(Order = 1)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 2)]
        public decimal ScheduleSeq { get; set; }
        [DataMember(Order = 3)]
        public decimal WOrderNum { get; set; }
        [DataMember(Order = 4)]
        public string WOrderType { get; set; }
        [DataMember(Order = 5)]
        public decimal SOrderNum { get; set; }
        [DataMember(Order = 6)]
        public string SOrderType { get; set; }
        [DataMember(Order = 7)]
        public decimal SOrderLine { get; set; }
        [DataMember(Order = 8)]
        public int CustNum { get; set; }
        [DataMember(Order = 9)]
        public string CustName { get; set; }
        [DataMember(Order = 10)]
        public string CustPO { get; set; }
        [DataMember(Order = 11)]
        public string CustPrtNum1 { get; set; }
        [DataMember(Order = 12)]
        public string CustPrtNum2 { get; set; }
        [DataMember(Order = 13)]
        public string CustPrtNum3 { get; set; }
        [DataMember(Order = 14)]
        public string CustPrtNum4 { get; set; }
        [DataMember(Order = 15)]
        public string CustIGLine { get; set; }
        [DataMember(Order = 16)]
        public string CustSchedNum { get; set; }
        [DataMember(Order = 17)]
        public string CustPL { get; set; }
        [DataMember(Order = 18)]
        public DateTime? DueDate { get; set; }
        [DataMember(Order = 19)]
        public DateTime? ScheduleDate { get; set; }
        [DataMember(Order = 20)]
        public string ScheduleType { get; set; }
        [DataMember(Order = 21)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 22)]
        public bool Picked { get; set; }
        [DataMember(Order = 23)]
        public string PriorityCode { get; set; }
        [DataMember(Order = 24)]
        public string DockLocal { get; set; }
        [DataMember(Order = 25)]
        public string CustSeq1 { get; set; }
        [DataMember(Order = 26)]
        public string CustSeq2 { get; set; }
        [DataMember(Order = 27)]
        public string Department { get; set; }
        [DataMember(Order = 28)]
        public string ShipZone { get; set; }
        [DataMember(Order = 29)]
        public int LineID { get; set; }
        [DataMember(Order = 30)]
        public string SalesMsg1 { get; set; }
        [DataMember(Order = 31)]
        public string SalesMsg2 { get; set; }
        [DataMember(Order = 32)]
        public string SalesMsg3 { get; set; }
        [DataMember(Order = 33)]
        public string SalesMsg4 { get; set; }
        [DataMember(Order = 34)]
        public string SalesMsg5 { get; set; }
        [DataMember(Order = 35)]
        public decimal MateLite { get; set; }
        [DataMember(Order = 36)]
        public string ProductType { get; set; }
        [DataMember(Order = 37)]
        public bool FlipGlass { get; set; }
        [DataMember(Order = 38)]
        public bool AddTubes { get; set; }
        [DataMember(Order = 39)]
        public bool AddPreserve { get; set; }
        [DataMember(Order = 40)]
        public bool AddScoring { get; set; }
        [DataMember(Order = 41)]
        public bool CrateGlass { get; set; }
        [DataMember(Order = 42)]
        public string Lite1_Routing { get; set; }
        [DataMember(Order = 43)]
        public decimal Lite1_Thick { get; set; }
        [DataMember(Order = 44)]
        public string Lite1_Color { get; set; }
        [DataMember(Order = 45)]
        public string Lite1_Heat { get; set; }
        [DataMember(Order = 46)]
        public string Lite1_1stSrfCoat { get; set; }
        [DataMember(Order = 47)]
        public string Lite1_2ndSrfCoat { get; set; }
        [DataMember(Order = 48)]
        public string Lite1_Pattern { get; set; }
        [DataMember(Order = 49)]
        public string Lite1_Lami { get; set; }
        [DataMember(Order = 50)]
        public decimal Lite1_Base { get; set; }
        [DataMember(Order = 51)]
        public decimal Lite1_Left { get; set; }
        [DataMember(Order = 52)]
        public decimal Lite1_Right { get; set; }
        [DataMember(Order = 53)]
        public decimal Lite1_Top { get; set; }
        [DataMember(Order = 54)]
        public decimal Lite1_s1 { get; set; }
        [DataMember(Order = 55)]
        public decimal Lite1_s2 { get; set; }
        [DataMember(Order = 56)]
        public decimal Lite1_Base_Leg { get; set; }
        [DataMember(Order = 57)]
        public decimal Lite1_Base_Num { get; set; }
        [DataMember(Order = 58)]
        public decimal Lite1_Base_Den { get; set; }
        [DataMember(Order = 59)]
        public decimal Lite1_Left_Leg { get; set; }
        [DataMember(Order = 60)]
        public decimal Lite1_Left_Num { get; set; }
        [DataMember(Order = 61)]
        public decimal Lite1_Left_Den { get; set; }
        [DataMember(Order = 62)]
        public decimal Lite1_Right_Leg { get; set; }
        [DataMember(Order = 63)]
        public decimal Lite1_Right_Num { get; set; }
        [DataMember(Order = 64)]
        public decimal Lite1_Right_Den { get; set; }
        [DataMember(Order = 65)]
        public decimal Lite1_Top_Leg { get; set; }
        [DataMember(Order = 66)]
        public decimal Lite1_Top_Num { get; set; }
        [DataMember(Order = 67)]
        public decimal Lite1_Top_Den { get; set; }
        [DataMember(Order = 68)]
        public decimal Lite1_S1_Leg { get; set; }
        [DataMember(Order = 69)]
        public decimal Lite1_S1_Num { get; set; }
        [DataMember(Order = 70)]
        public decimal Lite1_S1_Den { get; set; }
        [DataMember(Order = 71)]
        public decimal Lite1_S2_Leg { get; set; }
        [DataMember(Order = 72)]
        public decimal Lite1_S2_Num { get; set; }
        [DataMember(Order = 73)]
        public decimal Lite1_S2_Den { get; set; }
        [DataMember(Order = 74)]
        public string Lite1_GrvType { get; set; }
        [DataMember(Order = 75)]
        public int Lite1_GrvSurface { get; set; }
        [DataMember(Order = 76)]
        public string Lite1_GrvStyle { get; set; }
        [DataMember(Order = 77)]
        public int Lite1_GrvWide { get; set; }
        [DataMember(Order = 78)]
        public int Lite1_GrvHigh { get; set; }
        [DataMember(Order = 79)]
        public int Lite1_GrvOpen { get; set; }
        [DataMember(Order = 80)]
        public string Lite1_GrvDrawing { get; set; }
        [DataMember(Order = 81)]
        public string Lite1_Bevel { get; set; }
        [DataMember(Order = 82)]
        public string Lite1_Holes { get; set; }
        [DataMember(Order = 83)]
        public int LiteNumHoles { get; set; }
        [DataMember(Order = 84)]
        public string Lite1_Silk { get; set; }
        [DataMember(Order = 85)]
        public int Lite1_SilkSrf { get; set; }
        [DataMember(Order = 86)]
        public string Lite1_SilkDraw { get; set; }
        [DataMember(Order = 87)]
        public string Lite1_EdgeWrk { get; set; }
        [DataMember(Order = 88)]
        public string Lite1_ProdQuaLvl { get; set; }
        [DataMember(Order = 89)]
        public bool Lite1_TempLogo { get; set; }
        [DataMember(Order = 90)]
        public string Lite1_AnsiLogo { get; set; }
        [DataMember(Order = 91)]
        public decimal Lite1_offsetX { get; set; }
        [DataMember(Order = 92)]
        public decimal Lite1_OffsetY { get; set; }
        [DataMember(Order = 93)]
        public int Lite1_CusLiteNum { get; set; }
        [DataMember(Order = 94)]
        public string Lite1_PatTextSrf { get; set; }
        [DataMember(Order = 95)]
        public string Lite1_LamiColorSrf { get; set; }
        [DataMember(Order = 96)]
        public decimal Spacer_Size1 { get; set; }
        [DataMember(Order = 97)]
        public string Spacer_Type1 { get; set; }
        [DataMember(Order = 98)]
        public string Spacer_GasMix1 { get; set; }
        [DataMember(Order = 99)]
        public decimal Lite2_Thick { get; set; }
        [DataMember(Order = 100)]
        public string Lite2_Color { get; set; }
        [DataMember(Order = 101)]
        public string Lite2_Heat { get; set; }
        [DataMember(Order = 102)]
        public string Lite2_3rdSrfCoat { get; set; }
        [DataMember(Order = 103)]
        public string Lite2_4thSrfCoat { get; set; }
        [DataMember(Order = 104)]
        public string Lite2_Pattern { get; set; }
        [DataMember(Order = 105)]
        public string Lite2_Lami { get; set; }
        [DataMember(Order = 106)]
        public decimal Lite2_Base { get; set; }
        [DataMember(Order = 107)]
        public decimal Lite2_Left { get; set; }
        [DataMember(Order = 108)]
        public decimal Lite2_Right { get; set; }
        [DataMember(Order = 109)]
        public decimal Lite2_Top { get; set; }
        [DataMember(Order = 110)]
        public decimal Lite2_s1 { get; set; }
        [DataMember(Order = 111)]
        public decimal Lite2_s2 { get; set; }
        [DataMember(Order = 112)]
        public string Lite2_GrvType { get; set; }
        [DataMember(Order = 113)]
        public int Lite2_GrvSurface { get; set; }
        [DataMember(Order = 114)]
        public string Lite2_GrvStyle { get; set; }
        [DataMember(Order = 115)]
        public int Lite2_GrvWide { get; set; }
        [DataMember(Order = 116)]
        public int Lite2_GrvHigh { get; set; }
        [DataMember(Order = 117)]
        public int Lite2_GrvOpen { get; set; }
        [DataMember(Order = 118)]
        public string Lite2_GrvDrawing { get; set; }
        [DataMember(Order = 119)]
        public string Lite2_Bevel { get; set; }
        [DataMember(Order = 120)]
        public string Lite2_Holes { get; set; }
        [DataMember(Order = 121)]
        public string Lite2_Silk { get; set; }
        [DataMember(Order = 122)]
        public int Lite2_SilkSrf { get; set; }
        [DataMember(Order = 123)]
        public string Lite2_SilkDraw { get; set; }
        [DataMember(Order = 124)]
        public string Lite2_EdgeWrk { get; set; }
        [DataMember(Order = 125)]
        public bool Lite2_TempLogo { get; set; }
        [DataMember(Order = 126)]
        public string Lite2_PatTextSrf { get; set; }
        [DataMember(Order = 127)]
        public string Lite2_LamiColorSrf { get; set; }
        [DataMember(Order = 128)]
        public decimal Spacer_Size2 { get; set; }
        [DataMember(Order = 129)]
        public string Spacer_Type2 { get; set; }
        [DataMember(Order = 130)]
        public string Lite2_Routing { get; set; }
        [DataMember(Order = 131)]
        public decimal Lite3_Thick { get; set; }
        [DataMember(Order = 132)]
        public string Lite3_Color { get; set; }
        [DataMember(Order = 133)]
        public string Lite3_Heat { get; set; }
        [DataMember(Order = 134)]
        public string Lite3_5thtSrfCoat { get; set; }
        [DataMember(Order = 135)]
        public string Lite3_6thSrfCoat { get; set; }
        [DataMember(Order = 136)]
        public string Lite3_Pattern { get; set; }
        [DataMember(Order = 137)]
        public string Lite3_Lami { get; set; }
        [DataMember(Order = 138)]
        public decimal Lite3_Base { get; set; }
        [DataMember(Order = 139)]
        public decimal Lite3_Left { get; set; }
        [DataMember(Order = 140)]
        public decimal Lite3_Right { get; set; }
        [DataMember(Order = 141)]
        public decimal Lite3_Top { get; set; }
        [DataMember(Order = 142)]
        public decimal Lite3_s1 { get; set; }
        [DataMember(Order = 143)]
        public decimal Lite3_s2 { get; set; }
        [DataMember(Order = 144)]
        public string Lite3_GrvType { get; set; }
        [DataMember(Order = 145)]
        public int Lite3_GrvSurface { get; set; }
        [DataMember(Order = 146)]
        public string Lite3_GrvStyle { get; set; }
        [DataMember(Order = 147)]
        public int Lite3_GrvWide { get; set; }
        [DataMember(Order = 148)]
        public int Lite3_GrvHigh { get; set; }
        [DataMember(Order = 149)]
        public int Lite3_GrvOpen { get; set; }
        [DataMember(Order = 150)]
        public string Lite3_GrvDrawing { get; set; }
        [DataMember(Order = 151)]
        public string Lite3_Bevel { get; set; }
        [DataMember(Order = 152)]
        public string Lite3_Holes { get; set; }
        [DataMember(Order = 153)]
        public string Lite3_Silk { get; set; }
        [DataMember(Order = 154)]
        public int Lite3_SilkSrf { get; set; }
        [DataMember(Order = 155)]
        public string Lite3_SilkDraw { get; set; }
        [DataMember(Order = 156)]
        public string Lite3_EdgeWrk { get; set; }
        [DataMember(Order = 157)]
        public bool Lite3_TempLogo { get; set; }
        [DataMember(Order = 158)]
        public string Lite3_PatTextSrf { get; set; }
        [DataMember(Order = 159)]
        public string Lite3_LamiColorSrf { get; set; }
        [DataMember(Order = 160)]
        public string Lite3_Routing { get; set; }
        [DataMember(Order = 161)]
        public decimal MCode1_Air1 { get; set; }
        [DataMember(Order = 162)]
        public string MCode1_Air1Ort { get; set; }
        [DataMember(Order = 163)]
        public decimal MCode2_Air1 { get; set; }
        [DataMember(Order = 164)]
        public string MCode2_Air1Ort { get; set; }
        [DataMember(Order = 165)]
        public decimal Grilles_HzAir1 { get; set; }
        [DataMember(Order = 166)]
        public decimal Grilles_VtAir1 { get; set; }
        [DataMember(Order = 167)]
        public decimal OpenWide_Air1 { get; set; }
        [DataMember(Order = 168)]
        public decimal OpenHigh_Air1 { get; set; }
        [DataMember(Order = 169)]
        public decimal OddOpen_Air1 { get; set; }
        [DataMember(Order = 170)]
        public decimal CCode_Air1 { get; set; }
        [DataMember(Order = 171)]
        public string BumpOnType_Air1 { get; set; }
        [DataMember(Order = 172)]
        public decimal BumpOns_Air1 { get; set; }
        [DataMember(Order = 173)]
        public string Shades_Air1 { get; set; }
        [DataMember(Order = 174)]
        public bool BreatherTube_Air1 { get; set; }
        [DataMember(Order = 175)]
        public string GridStyle_Spacer1 { get; set; }
        [DataMember(Order = 176)]
        public string PrairieBar { get; set; }
        [DataMember(Order = 177)]
        public string NFRC { get; set; }
        [DataMember(Order = 178)]
        public decimal NFRC_VrtOffset { get; set; }
        [DataMember(Order = 179)]
        public decimal NFRC_HztOffset { get; set; }
        [DataMember(Order = 180)]
        public string NarwMargCode { get; set; }
        [DataMember(Order = 181)]
        public decimal VertSashIntru { get; set; }
        [DataMember(Order = 182)]
        public decimal HorzSashIntru { get; set; }
        [DataMember(Order = 183)]
        public decimal MCode1_Air2 { get; set; }
        [DataMember(Order = 184)]
        public string MCode1_Air2Ort { get; set; }
        [DataMember(Order = 185)]
        public decimal MCode2_Air2 { get; set; }
        [DataMember(Order = 186)]
        public string MCode2_Air2Ort { get; set; }
        [DataMember(Order = 187)]
        public decimal Grilles_HzAir2 { get; set; }
        [DataMember(Order = 188)]
        public decimal Grilles_VtAir2 { get; set; }
        [DataMember(Order = 189)]
        public decimal OpenWide_Air2 { get; set; }
        [DataMember(Order = 190)]
        public decimal OpenHigh_Air2 { get; set; }
        [DataMember(Order = 191)]
        public decimal OddOpen_Air2 { get; set; }
        [DataMember(Order = 192)]
        public decimal CCode_Air2 { get; set; }
        [DataMember(Order = 193)]
        public string BumpOnType_Air2 { get; set; }
        [DataMember(Order = 194)]
        public decimal BumpOns_Air2 { get; set; }
        [DataMember(Order = 195)]
        public string Shades_Air2 { get; set; }
        [DataMember(Order = 196)]
        public bool BreatherTube_Air2 { get; set; }
        [DataMember(Order = 197)]
        public string PlacementCode { get; set; }
        [DataMember(Order = 198)]
        public decimal VertPos1 { get; set; }
        [DataMember(Order = 199)]
        public decimal VertPos2 { get; set; }
        [DataMember(Order = 200)]
        public decimal VertPos3 { get; set; }
        [DataMember(Order = 201)]
        public decimal VertPos4 { get; set; }
        [DataMember(Order = 202)]
        public decimal VertPos5 { get; set; }
        [DataMember(Order = 203)]
        public decimal VertPos6 { get; set; }
        [DataMember(Order = 204)]
        public decimal VertPos7 { get; set; }
        [DataMember(Order = 205)]
        public decimal VertPos8 { get; set; }
        [DataMember(Order = 206)]
        public decimal VertPos9 { get; set; }
        [DataMember(Order = 207)]
        public decimal VertPos10 { get; set; }
        [DataMember(Order = 208)]
        public decimal VertPos11 { get; set; }
        [DataMember(Order = 209)]
        public decimal VertPos12 { get; set; }
        [DataMember(Order = 210)]
        public decimal VertPos13 { get; set; }
        [DataMember(Order = 211)]
        public decimal VertPos14 { get; set; }
        [DataMember(Order = 212)]
        public decimal VertPos15 { get; set; }
        [DataMember(Order = 213)]
        public decimal VertPos16 { get; set; }
        [DataMember(Order = 214)]
        public decimal VertPos17 { get; set; }
        [DataMember(Order = 215)]
        public decimal VertPos18 { get; set; }
        [DataMember(Order = 216)]
        public decimal VertPos19 { get; set; }
        [DataMember(Order = 217)]
        public decimal VertPos20 { get; set; }
        [DataMember(Order = 218)]
        public decimal HorzPos1 { get; set; }
        [DataMember(Order = 219)]
        public decimal HorzPos2 { get; set; }
        [DataMember(Order = 220)]
        public decimal HorzPos3 { get; set; }
        [DataMember(Order = 221)]
        public decimal HorzPos4 { get; set; }
        [DataMember(Order = 222)]
        public decimal HorzPos5 { get; set; }
        [DataMember(Order = 223)]
        public decimal HorzPos6 { get; set; }
        [DataMember(Order = 224)]
        public decimal HorzPos7 { get; set; }
        [DataMember(Order = 225)]
        public decimal HorzPos8 { get; set; }
        [DataMember(Order = 226)]
        public decimal HorzPos9 { get; set; }
        [DataMember(Order = 227)]
        public decimal HorzPos10 { get; set; }
        [DataMember(Order = 228)]
        public decimal HorzPos11 { get; set; }
        [DataMember(Order = 229)]
        public decimal HorzPos12 { get; set; }
        [DataMember(Order = 230)]
        public decimal HorzPos13 { get; set; }
        [DataMember(Order = 231)]
        public decimal HorzPos14 { get; set; }
        [DataMember(Order = 232)]
        public decimal HorzPos15 { get; set; }
        [DataMember(Order = 233)]
        public decimal HorzPos16 { get; set; }
        [DataMember(Order = 234)]
        public decimal HorzPos17 { get; set; }
        [DataMember(Order = 235)]
        public decimal HorzPos18 { get; set; }
        [DataMember(Order = 236)]
        public decimal HorzPos19 { get; set; }
        [DataMember(Order = 237)]
        public decimal HorzPos20 { get; set; }
        [DataMember(Order = 238)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 239)]
        public string ExtPrsvCB { get; set; }
        [DataMember(Order = 240)]
        public string ExtPrsvColor { get; set; }
        [DataMember(Order = 241)]
        public string ExtOpacifier { get; set; }
        [DataMember(Order = 242)]
        public decimal ExtPrsvScoring { get; set; }
        [DataMember(Order = 243)]
        public string IntPrsvCB { get; set; }
        [DataMember(Order = 244)]
        public string IntPrsvColor { get; set; }
        [DataMember(Order = 245)]
        public string IntOpacifier { get; set; }
        [DataMember(Order = 246)]
        public decimal IntPrsvScoring { get; set; }
        [DataMember(Order = 247)]
        public string LogoNum { get; set; }
        [DataMember(Order = 248)]
        public string SchedulerMsg1 { get; set; }
        [DataMember(Order = 249)]
        public string SchedulerMsg2 { get; set; }
        [DataMember(Order = 250)]
        public int BoxPriority { get; set; }
        [DataMember(Order = 251)]
        public string Bungee { get; set; }
        [DataMember(Order = 252)]
        public decimal Bungee_Size { get; set; }
        [DataMember(Order = 253)]
        public string MakeupLegacy { get; set; }
        [DataMember(Order = 254)]
        public string OddDate { get; set; }
        [DataMember(Order = 255)]
        public string DrawingID { get; set; }
        [DataMember(Order = 256)]
        public string CustMsg1 { get; set; }
        [DataMember(Order = 257)]
        public string CustMsg2 { get; set; }
        [DataMember(Order = 258)]
        public string CustMsg3 { get; set; }
        [DataMember(Order = 259)]
        public string CustMsg4 { get; set; }
        [DataMember(Order = 260)]
        public string CustMsg5 { get; set; }
        [DataMember(Order = 261)]
        public decimal PrairieBarHOffset { get; set; }
        [DataMember(Order = 262)]
        public decimal PrairieBarVOffset { get; set; }
        [DataMember(Order = 263)]
        public string CardinalProductType { get; set; }
        [DataMember(Order = 264)]
        public DateTime? CustomerDate { get; set; }
        [DataMember(Order = 265)]
        public string E1BatchID { get; set; }
        [DataMember(Order = 266)]
        public string CustomerOverlayBar { get; set; }
        [DataMember(Order = 267)]
        public string NotchLocation { get; set; }
        [DataMember(Order = 268)]
        public decimal NotchHeight { get; set; }
        [DataMember(Order = 269)]
        public decimal NotchWidth { get; set; }
        [DataMember(Order = 270)]
        public string Comment1 { get; set; }
        [DataMember(Order = 271)]
        public string Comment2 { get; set; }
        [DataMember(Order = 272)]
        public DateTime? DownloadDate { get; set; }
        [DataMember(Order = 273)]
        public string IGSubGridStyle { get; set; }
        [DataMember(Order = 274)]
        public string IGBumpOnConfigAir01 { get; set; }
        [DataMember(Order = 275)]
        public string IGBumpOnConfigAir02 { get; set; }
        [DataMember(Order = 276)]
        public decimal L1VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 277)]
        public decimal L1VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 278)]
        public string L1VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 279)]
        public string L1VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 280)]
        public decimal L2VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 281)]
        public decimal L2VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 282)]
        public string L2VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 283)]
        public string L2VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 284)]
        public decimal L3VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 285)]
        public decimal L3VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 286)]
        public string L3VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 287)]
        public string L3VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 288)]
        public decimal OrigCustNumSoldTo { get; set; }
        [DataMember(Order = 289)]
        public decimal TotalWeight { get; set; }
        [DataMember(Order = 290)]
        public string CustomerSuppliedGlass { get; set; }
        [DataMember(Order = 291)]
        public string ReasonCode { get; set; }
        [DataMember(Order = 292)]
        public string OriginalBranchPlant { get; set; }

        [DataMember(Order = 300)]
        public string CustEtchingMsg1 { get; set; }
        [DataMember(Order = 301)]
        public string CustEtchingMsg2 { get; set; }
        [DataMember(Order = 302)]
        public string CustEtchingMsg3 { get; set; }
        [DataMember(Order = 303)]
        public string CustEtchingMsg4 { get; set; }
        [DataMember(Order = 304)]
        public string CustEtchingMsg5 { get; set; }

        [DataMember(Order = 293)]
        public string QCInspect { get; set; }
        [DataMember(Order = 294)]
        public decimal PickingItemNumber { get; set; }
        [DataMember(Order = 295)]
        public string BasePickingLocation { get; set; }
        [DataMember(Order = 296)]
        public string Airspace1Tape { get; set; }
        [DataMember(Order = 297)]
        public string EnergyPanel { get; set; }
        [DataMember(Order = 298)]
        public string ShipBoxNumber { get; set; }
        [DataMember(Order = 299)]
        public string ShipMethod { get; set; }
        [DataMember(Order = 300)]
        public string Lite1LamiMixedTreatment { get; set; }
        [DataMember(Order = 301)]
        public string Lite1LamiColorSurface { get; set; }
        [DataMember(Order = 302)]
        public string Lite1LamiPatternSurface { get; set; }
        [DataMember(Order = 303)]
        public string Lite1LamiCoatingSurface { get; set; }
        [DataMember(Order = 304)]
        public string Lite2LamiMixedTreatment { get; set; }
        [DataMember(Order = 305)]
        public string Lite2LamiColorSurface { get; set; }
        [DataMember(Order = 306)]
        public string Lite2LamiPatternSurface { get; set; }
        [DataMember(Order = 307)]
        public string Lite2LamiCoatingSurface { get; set; }
        [DataMember(Order = 308)]
        public string Lite3LamiMixedTreatment { get; set; }
        [DataMember(Order = 309)]
        public string Lite3LamiColorSurface { get; set; }
        [DataMember(Order = 310)]
        public string Lite3LamiPatternSurface { get; set; }
        [DataMember(Order = 311)]
        public string Lite3LamiCoatingSurface { get; set; }
        [DataMember(Order = 312)]
        public string ButylColor { get; set; }
        [DataMember(Order = 313)]
        public string SiliconeColor { get; set; }
        [DataMember(Order = 314)]
        public string IGCCCertificationFlag { get; set; }
        [DataMember(Order = 315)]
        public decimal Lite1LamiThickness1 { get; set; }
        [DataMember(Order = 316)]
        public decimal Lite1LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 317)]
        public decimal Lite1LamiThickness2 { get; set; }
        [DataMember(Order = 318)]
        public decimal Lite2LamiThickness1 { get; set; }
        [DataMember(Order = 319)]
        public decimal Lite2LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 320)]
        public decimal Lite2LamiThickness2 { get; set; }
        [DataMember(Order = 321)]
        public decimal Lite3LamiThickness1 { get; set; }
        [DataMember(Order = 322)]
        public decimal Lite3LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 323)]
        public decimal Lite3LamiThickness2 { get; set; }
        [DataMember(Order = 324)]
        public string IGCaps { get; set; }
        [DataMember(Order = 325)]
        public string CornerClipping { get; set; }
        [DataMember(Order = 326)]
        public decimal EndItemPickItem { get; set; }
        [DataMember(Order = 327)]
        public decimal IQLite1DiscretePart { get; set; }
        [DataMember(Order = 328)]
        public decimal IQLite2DiscretePart { get; set; }
        [DataMember(Order = 329)]
        public decimal IQLite3DiscretePart { get; set; }
        [DataMember(Order = 330)]
        public string Lite1RawPickLocation { get; set; }
        [DataMember(Order = 331)]
        public string Lite2RawPickLocation { get; set; }
        [DataMember(Order = 332)]
        public string Lite3RawPickLocation { get; set; }
        [DataMember(Order = 333)]
        public string EdiLabelMsg1 { get; set; }
        [DataMember(Order = 334)]
        public string EdiLabelMsg2 { get; set; }
        [DataMember(Order = 335)]
        public string EdiLabelMsg3 { get; set; }
        [DataMember(Order = 336)]
        public string EdiLabelMsg4 { get; set; }
        [DataMember(Order = 337)]
        public string EdiLabelMsg5 { get; set; }
        [DataMember(Order = 338)]
        public string EdiLabelMsg6 { get; set; }
        [DataMember(Order = 339)]
        public string EdiLabelMsg7 { get; set; }
        [DataMember(Order = 340)]
        public string EdiLabelMsg8 { get; set; }
        [DataMember(Order = 341)]
        public string EdiLabelMsg9 { get; set; }
        [DataMember(Order = 342)]
        public string EdiLabelMsg10 { get; set; }
        [DataMember(Order = 343)]
        public string EdiEtchMsg1 { get; set; }
        [DataMember(Order = 344)]
        public string EdiEtchMsg2 { get; set; }
        [DataMember(Order = 345)]
        public string EdiEtchMsg3 { get; set; }
        [DataMember(Order = 346)]
        public string EdiEtchMsg4 { get; set; }
        [DataMember(Order = 347)]
        public string EdiEtchMsg5 { get; set; }
        [DataMember(Order = 348)]
        public string EdiEtchMsg6 { get; set; }
        [DataMember(Order = 349)]
        public string EdiEtchMsg7 { get; set; }
        [DataMember(Order = 350)]
        public string EdiEtchMsg8 { get; set; }
        [DataMember(Order = 351)]
        public string EdiEtchMsg9 { get; set; }
        [DataMember(Order = 352)]
        public string EdiEtchMsg10 { get; set; }
        [DataMember(Order = 353)]
        public decimal CrateId { get; set; }
        [DataMember(Order = 354)]
        public string CrateType { get; set; }
        [DataMember(Order = 356)]
        public decimal CustBillToNum { get; set; }
        [DataMember(Order = 357)]
        public string GenericSeqNum { get; set; }
        [DataMember(Order = 358)]
        public int RequestedDeliveryTime { get; set; }
        [DataMember(Order = 359)]
        public string BatchName { get; set; }
        [DataMember(Order = 360)]
        public string BoxingOption { get; set; }
        [DataMember(Order = 361)]
        public string RunOption { get; set; }
        [DataMember(Order = 362)]
        public decimal CustomerShipTo { get; set; }
        [DataMember(Order = 363)]
        public string Crate { get; set; }
        [DataMember(Order = 365)]
        public decimal Lite1_NumHoles { get; set; }
        [DataMember(Order = 366)]
        public decimal Lite2_NumHoles { get; set; }
        [DataMember(Order = 367)]
        public decimal Lite3_NumHoles { get; set; }
        [DataMember(Order = 368)]
        public string MailingName { get; set; }
        [DataMember(Order = 370)]
        public string NFRC2 { get; set; }
        [DataMember(Order = 371)]
        public string PickingNotes { get; set; }
        [DataMember(Order = 372)]
        public string PackingNotes { get; set; }
        [DataMember(Order = 373)]
        public string SCHED_VALUE03 { get; set; }
        [DataMember(Order = 374)]
        public string SCHED_VALUE04 { get; set; }

        [DataMember(Order = 377)]
        public string QCInspectFlag { get; set; }
        [DataMember(Order = 378)]
        public string EdgeDeleteWidth { get; set; }

    }
}
