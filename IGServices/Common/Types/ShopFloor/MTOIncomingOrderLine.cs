using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using IGServices.Common.Types.Enterprise;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class MTOIncomingOrderLines : List<MTOIncomingOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class MTOIncomingOrderLine
    {
        public MTOIncomingOrderLine()
        {
            PlantNum = "";
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
            CustIGLine = "";
            CustSchedNum = "";
            CustPL = "";
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
            Lite1_PatTextSrf = "";
            Lite1_LamiColorSrf = "";
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
            Lite2_PatTextSrf = "";
            Lite2_LamiColorSrf = "";
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
            Lite3_PatTextSrf = "";
            Lite3_LamiColorSrf = "";
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
            PrairieBar = "";
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
            PrairieBarHOffset = 0;
            PrairieBarVOffset = 0;
            CardinalProductType = "";
            CustomerDate = new DateTime(1900, 1, 1);
            E1BatchID = "";
            CustomerOverlayBar = "";
            NotchLocation = "";
            NotchHeight = 0;
            NotchWidth = 0;
            Comment1 = "";
            Comment2 = "";
            DownloadDate = new DateTime(1900, 1, 1);
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
            Lite1LamiThickness1 = 0;
            Lite1LamiInnerLayerThickness = 0;
            Lite1LamiThickness2 = 0;
            Lite2LamiThickness1 = 0;
            Lite2LamiInnerLayerThickness = 0;
            Lite2LamiThickness2 = 0;
            Lite3LamiThickness1 = 0;
            Lite3LamiInnerLayerThickness = 0;
            Lite3LamiThickness2 = 0;
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
            EdiLabelMsg11 = "";
            EdiLabelMsg12 = "";
            EdiLabelMsg13 = "";
            EdiLabelMsg14 = "";
            EdiLabelMsg15 = "";
            EdiLabelMsg16 = "";
            EdiLabelMsg17 = "";
            EdiLabelMsg18 = "";
            EdiLabelMsg19 = "";
            EdiLabelMsg20 = "";
            EdiLabelMsg21 = "";
            EdiLabelMsg22 = "";
            EdiLabelMsg23 = "";
            EdiLabelMsg24 = "";
            EdiLabelMsg25 = "";
            EdiLabelMsg26 = "";
            EdiLabelMsg27 = "";
            EdiLabelMsg28 = "";
            EdiLabelMsg29 = "";
            EdiLabelMsg30 = "";
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
            ReasonCode = "";
            CustBillToNum = 0;
            GenericSeqNum = "";
            RequestedDeliveryTime = 0;
            BatchName = "";
            BoxingOption = "";
            RunOption = "";
            CustomerShipTo = 0;
            Crate = "";
            TotalWeight = 0;
            Lite1_NumHoles = 0;
            Lite2_NumHoles = 0;
            Lite3_NumHoles = 0;
            MailingName = "";
            OrigCustNumSoldTo = 0;
            NFRC2 = "";
            PickingNotes = "";
            PackingNotes = "";
            SCHED_VALUE03 = "";
            SCHED_VALUE04 = "";
            SCHED_VALUE05 = "";
            SCHED_VALUE06 = "";
            SCHED_VALUE07 = "";
            SCHED_VALUE08 = "";
            SCHED_VALUE09 = "";
            SCHED_VALUE10 = "";
            OriginalBranchPlant = "";
            QCInspectFlag = "";
            EdgeDeleteWidth = "";
            ConfigID = 0;
            FabDrawing = "";
            PLSide = "";
            FabDesc = "";
            SqftActual = 0;
            EdgeWorkSides = "";       
            ActualIGOverallThickness = "";
            IGUnitOffsetAmount = 0;
            IGUnitShapeOffsetSide = "";
            TaglessLogo = "";
            RemakeFlag = "";

            Lite1_Inventory = 0;
            Lite2_Inventory = 0;
            Lite3_Inventory = 0;
            Lite1_ReleaseNote = "";
            Lite1_ReleaseTimeNote = "";
            Lite2_ReleaseNote = "";
            Lite2_ReleaseTimeNote = "";
            Lite3_ReleaseNote = "";
            Lite3_ReleaseTimeNote = "";
            ChangedNote = "";

            VerticalBarOffset = 0;
            HorizontalBarOffset = 0;

            Lite1Plant = "";
            Lite2Plant = "";
            Lite3Plant = "";
            Lite1Weight = 0;
            Lite2Weight = 0;
            Lite3Weight = 0;

            OrigCustomerPO = "";
            ShipCompleteId = "";

            CustomBarAirspace1 = "";
            CustomBarAirspace2 = "";

            DeliveryInstruction1 = "";
            DeliveryInstruction2 = "";

            PressureAdjusted = "";
            Lite1_SandblastDraw = "";
            Lite2_SandblastDraw = "";
            Lite3_SandblastDraw = "";

            Lite1LamiCustom = "";
            Lite2LamiCustom = "";
            Lite3LamiCustom = "";
            VigCapLocation = "";
            VigCapType = "";
            WaterJet = "";
   }

        [DataMember(Order = 0, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public decimal ScheduleSeq { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public decimal WOrderNum { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string WOrderType { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public decimal SOrderNum { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string SOrderType { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public decimal SOrderLine { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public int CustNum { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string CustName { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string CustPO { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string CustPrtNum1 { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string CustPrtNum2 { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string CustPrtNum3 { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string CustPrtNum4 { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public string CustIGLine { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string CustSchedNum { get; set; }
        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string CustPL { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public DateTime DueDate { get; set; }
        [DataMember(Order = 19, EmitDefaultValue = false)]
        public DateTime ScheduleDate { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ScheduleType { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public bool Picked { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string PriorityCode { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string DockLocal { get; set; }
        [DataMember(Order = 25, EmitDefaultValue = false)]
        public string CustSeq1 { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public string CustSeq2 { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public string Department { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public string ShipZone { get; set; }
        [DataMember(Order = 29, EmitDefaultValue = false)]
        public int LineID { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string SalesMsg1 { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string SalesMsg2 { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string SalesMsg3 { get; set; }
        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string SalesMsg4 { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string SalesMsg5 { get; set; }
        [DataMember(Order = 35, EmitDefaultValue = false)]
        public decimal MateLite { get; set; }
        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string ProductType { get; set; }
        [DataMember(Order = 37, EmitDefaultValue = false)]
        public bool FlipGlass { get; set; }
        [DataMember(Order = 38, EmitDefaultValue = false)]
        public bool AddTubes { get; set; }
        [DataMember(Order = 39, EmitDefaultValue = false)]
        public bool AddPreserve { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public bool AddScoring { get; set; }
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public bool CrateGlass { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string Lite1_Routing { get; set; }
        [DataMember(Order = 43, EmitDefaultValue = false)]
        public decimal Lite1_Thick { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public string Lite1_Color { get; set; }
        [DataMember(Order = 45, EmitDefaultValue = false)]
        public string Lite1_Heat { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public string Lite1_1stSrfCoat { get; set; }
        [DataMember(Order = 47, EmitDefaultValue = false)]
        public string Lite1_2ndSrfCoat { get; set; }
        [DataMember(Order = 48, EmitDefaultValue = false)]
        public string Lite1_Pattern { get; set; }
        [DataMember(Order = 49, EmitDefaultValue = false)]
        public string Lite1_Lami { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal Lite1_Base { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public decimal Lite1_Left { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public decimal Lite1_Right { get; set; }
        [DataMember(Order = 53, EmitDefaultValue = false)]
        public decimal Lite1_Top { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public decimal Lite1_s1 { get; set; }
        [DataMember(Order = 55, EmitDefaultValue = false)]
        public decimal Lite1_s2 { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public decimal Lite1_Base_Leg { get; set; }
        [DataMember(Order = 57, EmitDefaultValue = false)]
        public decimal Lite1_Base_Num { get; set; }
        [DataMember(Order = 58, EmitDefaultValue = false)]
        public decimal Lite1_Base_Den { get; set; }
        [DataMember(Order = 59, EmitDefaultValue = false)]
        public decimal Lite1_Left_Leg { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal Lite1_Left_Num { get; set; }
        [DataMember(Order = 61, EmitDefaultValue = false)]
        public decimal Lite1_Left_Den { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public decimal Lite1_Right_Leg { get; set; }
        [DataMember(Order = 63, EmitDefaultValue = false)]
        public decimal Lite1_Right_Num { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public decimal Lite1_Right_Den { get; set; }
        [DataMember(Order = 65, EmitDefaultValue = false)]
        public decimal Lite1_Top_Leg { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public decimal Lite1_Top_Num { get; set; }
        [DataMember(Order = 67, EmitDefaultValue = false)]
        public decimal Lite1_Top_Den { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public decimal Lite1_S1_Leg { get; set; }
        [DataMember(Order = 69, EmitDefaultValue = false)]
        public decimal Lite1_S1_Num { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal Lite1_S1_Den { get; set; }
        [DataMember(Order = 71, EmitDefaultValue = false)]
        public decimal Lite1_S2_Leg { get; set; }
        [DataMember(Order = 72, EmitDefaultValue = false)]
        public decimal Lite1_S2_Num { get; set; }
        [DataMember(Order = 73, EmitDefaultValue = false)]
        public decimal Lite1_S2_Den { get; set; }
        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string Lite1_GrvType { get; set; }
        [DataMember(Order = 75, EmitDefaultValue = false)]
        public int Lite1_GrvSurface { get; set; }
        [DataMember(Order = 76, EmitDefaultValue = false)]
        public string Lite1_GrvStyle { get; set; }
        [DataMember(Order = 77, EmitDefaultValue = false)]
        public int Lite1_GrvWide { get; set; }
        [DataMember(Order = 78, EmitDefaultValue = false)]
        public int Lite1_GrvHigh { get; set; }
        [DataMember(Order = 79, EmitDefaultValue = false)]
        public int Lite1_GrvOpen { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string Lite1_GrvDrawing { get; set; }
        [DataMember(Order = 81, EmitDefaultValue = false)]
        public string Lite1_Bevel { get; set; }
        [DataMember(Order = 82, EmitDefaultValue = false)]
        public string Lite1_Holes { get; set; }
        [DataMember(Order = 83, EmitDefaultValue = false)]
        public int LiteNumHoles { get; set; }
        [DataMember(Order = 84, EmitDefaultValue = false)]
        public string Lite1_Silk { get; set; }
        [DataMember(Order = 85, EmitDefaultValue = false)]
        public int Lite1_SilkSrf { get; set; }
        [DataMember(Order = 86, EmitDefaultValue = false)]
        public string Lite1_SilkDraw { get; set; }
        [DataMember(Order = 87, EmitDefaultValue = false)]
        public string Lite1_EdgeWrk { get; set; }
        [DataMember(Order = 88, EmitDefaultValue = false)]
        public string Lite1_ProdQuaLvl { get; set; }
        [DataMember(Order = 89, EmitDefaultValue = false)]
        public bool Lite1_TempLogo { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string Lite1_AnsiLogo { get; set; }
        [DataMember(Order = 91, EmitDefaultValue = false)]
        public decimal Lite1_offsetX { get; set; }
        [DataMember(Order = 92, EmitDefaultValue = false)]
        public decimal Lite1_OffsetY { get; set; }
        [DataMember(Order = 93, EmitDefaultValue = false)]
        public int Lite1_CusLiteNum { get; set; }
        [DataMember(Order = 94, EmitDefaultValue = false)]
        public string Lite1_PatTextSrf { get; set; }
        [DataMember(Order = 95, EmitDefaultValue = false)]
        public string Lite1_LamiColorSrf { get; set; }
        [DataMember(Order = 96, EmitDefaultValue = false)]
        public decimal Spacer_Size1 { get; set; }
        [DataMember(Order = 97, EmitDefaultValue = false)]
        public string Spacer_Type1 { get; set; }
        [DataMember(Order = 98, EmitDefaultValue = false)]
        public string Spacer_GasMix1 { get; set; }
        [DataMember(Order = 99, EmitDefaultValue = false)]
        public decimal Lite2_Thick { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string Lite2_Color { get; set; }
        [DataMember(Order = 101, EmitDefaultValue = false)]
        public string Lite2_Heat { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public string Lite2_3rdSrfCoat { get; set; }
        [DataMember(Order = 103, EmitDefaultValue = false)]
        public string Lite2_4thSrfCoat { get; set; }
        [DataMember(Order = 104, EmitDefaultValue = false)]
        public string Lite2_Pattern { get; set; }
        [DataMember(Order = 105, EmitDefaultValue = false)]
        public string Lite2_Lami { get; set; }
        [DataMember(Order = 106, EmitDefaultValue = false)]
        public decimal Lite2_Base { get; set; }
        [DataMember(Order = 107, EmitDefaultValue = false)]
        public decimal Lite2_Left { get; set; }
        [DataMember(Order = 108, EmitDefaultValue = false)]
        public decimal Lite2_Right { get; set; }
        [DataMember(Order = 109, EmitDefaultValue = false)]
        public decimal Lite2_Top { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal Lite2_s1 { get; set; }
        [DataMember(Order = 111, EmitDefaultValue = false)]
        public decimal Lite2_s2 { get; set; }
        [DataMember(Order = 112, EmitDefaultValue = false)]
        public string Lite2_GrvType { get; set; }
        [DataMember(Order = 113, EmitDefaultValue = false)]
        public int Lite2_GrvSurface { get; set; }
        [DataMember(Order = 114, EmitDefaultValue = false)]
        public string Lite2_GrvStyle { get; set; }
        [DataMember(Order = 115, EmitDefaultValue = false)]
        public int Lite2_GrvWide { get; set; }
        [DataMember(Order = 116, EmitDefaultValue = false)]
        public int Lite2_GrvHigh { get; set; }
        [DataMember(Order = 117, EmitDefaultValue = false)]
        public int Lite2_GrvOpen { get; set; }
        [DataMember(Order = 118, EmitDefaultValue = false)]
        public string Lite2_GrvDrawing { get; set; }
        [DataMember(Order = 119, EmitDefaultValue = false)]
        public string Lite2_Bevel { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string Lite2_Holes { get; set; }
        [DataMember(Order = 121, EmitDefaultValue = false)]
        public string Lite2_Silk { get; set; }
        [DataMember(Order = 122, EmitDefaultValue = false)]
        public int Lite2_SilkSrf { get; set; }
        [DataMember(Order = 123, EmitDefaultValue = false)]
        public string Lite2_SilkDraw { get; set; }
        [DataMember(Order = 124, EmitDefaultValue = false)]
        public string Lite2_EdgeWrk { get; set; }
        [DataMember(Order = 125, EmitDefaultValue = false)]
        public bool Lite2_TempLogo { get; set; }
        [DataMember(Order = 126, EmitDefaultValue = false)]
        public string Lite2_PatTextSrf { get; set; }
        [DataMember(Order = 127, EmitDefaultValue = false)]
        public string Lite2_LamiColorSrf { get; set; }
        [DataMember(Order = 128, EmitDefaultValue = false)]
        public decimal Spacer_Size2 { get; set; }
        [DataMember(Order = 129, EmitDefaultValue = false)]
        public string Spacer_Type2 { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string Lite2_Routing { get; set; }
        [DataMember(Order = 131, EmitDefaultValue = false)]
        public decimal Lite3_Thick { get; set; }
        [DataMember(Order = 132, EmitDefaultValue = false)]
        public string Lite3_Color { get; set; }
        [DataMember(Order = 133, EmitDefaultValue = false)]
        public string Lite3_Heat { get; set; }
        [DataMember(Order = 134, EmitDefaultValue = false)]
        public string Lite3_5thtSrfCoat { get; set; }
        [DataMember(Order = 135, EmitDefaultValue = false)]
        public string Lite3_6thSrfCoat { get; set; }
        [DataMember(Order = 136, EmitDefaultValue = false)]
        public string Lite3_Pattern { get; set; }
        [DataMember(Order = 137, EmitDefaultValue = false)]
        public string Lite3_Lami { get; set; }
        [DataMember(Order = 138, EmitDefaultValue = false)]
        public decimal Lite3_Base { get; set; }
        [DataMember(Order = 139, EmitDefaultValue = false)]
        public decimal Lite3_Left { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public decimal Lite3_Right { get; set; }
        [DataMember(Order = 141, EmitDefaultValue = false)]
        public decimal Lite3_Top { get; set; }
        [DataMember(Order = 142, EmitDefaultValue = false)]
        public decimal Lite3_s1 { get; set; }
        [DataMember(Order = 143, EmitDefaultValue = false)]
        public decimal Lite3_s2 { get; set; }
        [DataMember(Order = 144, EmitDefaultValue = false)]
        public string Lite3_GrvType { get; set; }
        [DataMember(Order = 145, EmitDefaultValue = false)]
        public int Lite3_GrvSurface { get; set; }
        [DataMember(Order = 146, EmitDefaultValue = false)]
        public string Lite3_GrvStyle { get; set; }
        [DataMember(Order = 147, EmitDefaultValue = false)]
        public int Lite3_GrvWide { get; set; }
        [DataMember(Order = 148, EmitDefaultValue = false)]
        public int Lite3_GrvHigh { get; set; }
        [DataMember(Order = 149, EmitDefaultValue = false)]
        public int Lite3_GrvOpen { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string Lite3_GrvDrawing { get; set; }
        [DataMember(Order = 151, EmitDefaultValue = false)]
        public string Lite3_Bevel { get; set; }
        [DataMember(Order = 152, EmitDefaultValue = false)]
        public string Lite3_Holes { get; set; }
        [DataMember(Order = 153, EmitDefaultValue = false)]
        public string Lite3_Silk { get; set; }
        [DataMember(Order = 154, EmitDefaultValue = false)]
        public int Lite3_SilkSrf { get; set; }
        [DataMember(Order = 155, EmitDefaultValue = false)]
        public string Lite3_SilkDraw { get; set; }
        [DataMember(Order = 156, EmitDefaultValue = false)]
        public string Lite3_EdgeWrk { get; set; }
        [DataMember(Order = 157, EmitDefaultValue = false)]
        public bool Lite3_TempLogo { get; set; }
        [DataMember(Order = 158, EmitDefaultValue = false)]
        public string Lite3_PatTextSrf { get; set; }
        [DataMember(Order = 159, EmitDefaultValue = false)]
        public string Lite3_LamiColorSrf { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string Lite3_Routing { get; set; }
        [DataMember(Order = 161, EmitDefaultValue = false)]
        public decimal MCode1_Air1 { get; set; }
        [DataMember(Order = 162, EmitDefaultValue = false)]
        public string MCode1_Air1Ort { get; set; }
        [DataMember(Order = 163, EmitDefaultValue = false)]
        public decimal MCode2_Air1 { get; set; }
        [DataMember(Order = 164, EmitDefaultValue = false)]
        public string MCode2_Air1Ort { get; set; }
        [DataMember(Order = 165, EmitDefaultValue = false)]
        public decimal Grilles_HzAir1 { get; set; }
        [DataMember(Order = 166, EmitDefaultValue = false)]
        public decimal Grilles_VtAir1 { get; set; }
        [DataMember(Order = 167, EmitDefaultValue = false)]
        public decimal OpenWide_Air1 { get; set; }
        [DataMember(Order = 168, EmitDefaultValue = false)]
        public decimal OpenHigh_Air1 { get; set; }
        [DataMember(Order = 169, EmitDefaultValue = false)]
        public decimal OddOpen_Air1 { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public decimal CCode_Air1 { get; set; }
        [DataMember(Order = 171, EmitDefaultValue = false)]
        public string BumpOnType_Air1 { get; set; }
        [DataMember(Order = 172, EmitDefaultValue = false)]
        public decimal BumpOns_Air1 { get; set; }
        [DataMember(Order = 173, EmitDefaultValue = false)]
        public string Shades_Air1 { get; set; }
        [DataMember(Order = 174, EmitDefaultValue = false)]
        public bool BreatherTube_Air1 { get; set; }
        [DataMember(Order = 175, EmitDefaultValue = false)]
        public string GridStyle_Spacer1 { get; set; }
        [DataMember(Order = 176, EmitDefaultValue = false)]
        public string PrairieBar { get; set; }
        [DataMember(Order = 177, EmitDefaultValue = false)]
        public string NFRC { get; set; }
        [DataMember(Order = 178, EmitDefaultValue = false)]
        public decimal NFRC_VrtOffset { get; set; }
        [DataMember(Order = 179, EmitDefaultValue = false)]
        public decimal NFRC_HztOffset { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public string NarwMargCode { get; set; }
        [DataMember(Order = 181, EmitDefaultValue = false)]
        public decimal VertSashIntru { get; set; }
        [DataMember(Order = 182, EmitDefaultValue = false)]
        public decimal HorzSashIntru { get; set; }
        [DataMember(Order = 183, EmitDefaultValue = false)]
        public decimal MCode1_Air2 { get; set; }
        [DataMember(Order = 184, EmitDefaultValue = false)]
        public string MCode1_Air2Ort { get; set; }
        [DataMember(Order = 185, EmitDefaultValue = false)]
        public decimal MCode2_Air2 { get; set; }
        [DataMember(Order = 186, EmitDefaultValue = false)]
        public string MCode2_Air2Ort { get; set; }
        [DataMember(Order = 187, EmitDefaultValue = false)]
        public decimal Grilles_HzAir2 { get; set; }
        [DataMember(Order = 188, EmitDefaultValue = false)]
        public decimal Grilles_VtAir2 { get; set; }
        [DataMember(Order = 189, EmitDefaultValue = false)]
        public decimal OpenWide_Air2 { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal OpenHigh_Air2 { get; set; }
        [DataMember(Order = 191, EmitDefaultValue = false)]
        public decimal OddOpen_Air2 { get; set; }
        [DataMember(Order = 192, EmitDefaultValue = false)]
        public decimal CCode_Air2 { get; set; }
        [DataMember(Order = 193, EmitDefaultValue = false)]
        public string BumpOnType_Air2 { get; set; }
        [DataMember(Order = 194, EmitDefaultValue = false)]
        public decimal BumpOns_Air2 { get; set; }
        [DataMember(Order = 195, EmitDefaultValue = false)]
        public string Shades_Air2 { get; set; }
        [DataMember(Order = 196, EmitDefaultValue = false)]
        public bool BreatherTube_Air2 { get; set; }
        [DataMember(Order = 197, EmitDefaultValue = false)]
        public string PlacementCode { get; set; }
        [DataMember(Order = 198, EmitDefaultValue = false)]
        public decimal VertPos1 { get; set; }
        [DataMember(Order = 199, EmitDefaultValue = false)]
        public decimal VertPos2 { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal VertPos3 { get; set; }
        [DataMember(Order = 201, EmitDefaultValue = false)]
        public decimal VertPos4 { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public decimal VertPos5 { get; set; }
        [DataMember(Order = 203, EmitDefaultValue = false)]
        public decimal VertPos6 { get; set; }
        [DataMember(Order = 204, EmitDefaultValue = false)]
        public decimal VertPos7 { get; set; }
        [DataMember(Order = 205, EmitDefaultValue = false)]
        public decimal VertPos8 { get; set; }
        [DataMember(Order = 206, EmitDefaultValue = false)]
        public decimal VertPos9 { get; set; }
        [DataMember(Order = 207, EmitDefaultValue = false)]
        public decimal VertPos10 { get; set; }
        [DataMember(Order = 208, EmitDefaultValue = false)]
        public decimal VertPos11 { get; set; }
        [DataMember(Order = 209, EmitDefaultValue = false)]
        public decimal VertPos12 { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal VertPos13 { get; set; }
        [DataMember(Order = 211, EmitDefaultValue = false)]
        public decimal VertPos14 { get; set; }
        [DataMember(Order = 212, EmitDefaultValue = false)]
        public decimal VertPos15 { get; set; }
        [DataMember(Order = 213, EmitDefaultValue = false)]
        public decimal VertPos16 { get; set; }
        [DataMember(Order = 214, EmitDefaultValue = false)]
        public decimal VertPos17 { get; set; }
        [DataMember(Order = 215, EmitDefaultValue = false)]
        public decimal VertPos18 { get; set; }
        [DataMember(Order = 216, EmitDefaultValue = false)]
        public decimal VertPos19 { get; set; }
        [DataMember(Order = 217, EmitDefaultValue = false)]
        public decimal VertPos20 { get; set; }
        [DataMember(Order = 218, EmitDefaultValue = false)]
        public decimal HorzPos1 { get; set; }
        [DataMember(Order = 219, EmitDefaultValue = false)]
        public decimal HorzPos2 { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal HorzPos3 { get; set; }
        [DataMember(Order = 221, EmitDefaultValue = false)]
        public decimal HorzPos4 { get; set; }
        [DataMember(Order = 222, EmitDefaultValue = false)]
        public decimal HorzPos5 { get; set; }
        [DataMember(Order = 223, EmitDefaultValue = false)]
        public decimal HorzPos6 { get; set; }
        [DataMember(Order = 224, EmitDefaultValue = false)]
        public decimal HorzPos7 { get; set; }
        [DataMember(Order = 225, EmitDefaultValue = false)]
        public decimal HorzPos8 { get; set; }
        [DataMember(Order = 226, EmitDefaultValue = false)]
        public decimal HorzPos9 { get; set; }
        [DataMember(Order = 227, EmitDefaultValue = false)]
        public decimal HorzPos10 { get; set; }
        [DataMember(Order = 228, EmitDefaultValue = false)]
        public decimal HorzPos11 { get; set; }
        [DataMember(Order = 229, EmitDefaultValue = false)]
        public decimal HorzPos12 { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public decimal HorzPos13 { get; set; }
        [DataMember(Order = 231, EmitDefaultValue = false)]
        public decimal HorzPos14 { get; set; }
        [DataMember(Order = 232, EmitDefaultValue = false)]
        public decimal HorzPos15 { get; set; }
        [DataMember(Order = 233, EmitDefaultValue = false)]
        public decimal HorzPos16 { get; set; }
        [DataMember(Order = 234, EmitDefaultValue = false)]
        public decimal HorzPos17 { get; set; }
        [DataMember(Order = 235, EmitDefaultValue = false)]
        public decimal HorzPos18 { get; set; }
        [DataMember(Order = 236, EmitDefaultValue = false)]
        public decimal HorzPos19 { get; set; }
        [DataMember(Order = 237, EmitDefaultValue = false)]
        public decimal HorzPos20 { get; set; }
        [DataMember(Order = 238, EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 239, EmitDefaultValue = false)]
        public string ExtPrsvCB { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public string ExtPrsvColor { get; set; }
        [DataMember(Order = 241, EmitDefaultValue = false)]
        public string ExtOpacifier { get; set; }
        [DataMember(Order = 242, EmitDefaultValue = false)]
        public decimal ExtPrsvScoring { get; set; }
        [DataMember(Order = 243, EmitDefaultValue = false)]
        public string IntPrsvCB { get; set; }
        [DataMember(Order = 244, EmitDefaultValue = false)]
        public string IntPrsvColor { get; set; }
        [DataMember(Order = 245, EmitDefaultValue = false)]
        public string IntOpacifier { get; set; }
        [DataMember(Order = 246, EmitDefaultValue = false)]
        public decimal IntPrsvScoring { get; set; }
        [DataMember(Order = 247, EmitDefaultValue = false)]
        public string LogoNum { get; set; }
        [DataMember(Order = 248, EmitDefaultValue = false)]
        public string SchedulerMsg1 { get; set; }
        [DataMember(Order = 249, EmitDefaultValue = false)]
        public string SchedulerMsg2 { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public int BoxPriority { get; set; }
        [DataMember(Order = 251, EmitDefaultValue = false)]
        public string Bungee { get; set; }
        [DataMember(Order = 252, EmitDefaultValue = false)]
        public decimal Bungee_Size { get; set; }
        [DataMember(Order = 253, EmitDefaultValue = false)]
        public string MakeupLegacy { get; set; }
        [DataMember(Order = 254, EmitDefaultValue = false)]
        public string OddDate { get; set; }
        [DataMember(Order = 255, EmitDefaultValue = false)]
        public string DrawingID { get; set; }
        [DataMember(Order = 256, EmitDefaultValue = false)]
        public string CustMsg1 { get; set; }
        [DataMember(Order = 257, EmitDefaultValue = false)]
        public string CustMsg2 { get; set; }
        [DataMember(Order = 258, EmitDefaultValue = false)]
        public string CustMsg3 { get; set; }
        [DataMember(Order = 259, EmitDefaultValue = false)]
        public string CustMsg4 { get; set; }
        [DataMember(Order = 260, EmitDefaultValue = false)]
        public string CustMsg5 { get; set; }
        [DataMember(Order = 261, EmitDefaultValue = false)]
        public decimal PrairieBarHOffset { get; set; }
        [DataMember(Order = 262, EmitDefaultValue = false)]
        public decimal PrairieBarVOffset { get; set; }
        [DataMember(Order = 263, EmitDefaultValue = false)]
        public string CardinalProductType { get; set; }
        [DataMember(Order = 264, EmitDefaultValue = false)]
        public DateTime CustomerDate { get; set; }
        [DataMember(Order = 265, EmitDefaultValue = false)]
        public string E1BatchID { get; set; }
        [DataMember(Order = 266, EmitDefaultValue = false)]
        public string CustomerOverlayBar { get; set; }
        [DataMember(Order = 267, EmitDefaultValue = false)]
        public string NotchLocation { get; set; }
        [DataMember(Order = 268, EmitDefaultValue = false)]
        public decimal NotchHeight { get; set; }
        [DataMember(Order = 269, EmitDefaultValue = false)]
        public decimal NotchWidth { get; set; }
        [DataMember(Order = 270, EmitDefaultValue = false)]
        public string Comment1 { get; set; }
        [DataMember(Order = 271, EmitDefaultValue = false)]
        public string Comment2 { get; set; }
        [DataMember(Order = 272, EmitDefaultValue = false)]
        public DateTime DownloadDate { get; set; }
        [DataMember(Order = 273, EmitDefaultValue = false)]
        public string IGSubGridStyle { get; set; }
        [DataMember(Order = 274, EmitDefaultValue = false)]
        public string IGBumpOnConfigAir01 { get; set; }
        [DataMember(Order = 275, EmitDefaultValue = false)]
        public string IGBumpOnConfigAir02 { get; set; }
        [DataMember(Order = 276, EmitDefaultValue = false)]
        public decimal L1VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 277, EmitDefaultValue = false)]
        public decimal L1VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 278, EmitDefaultValue = false)]
        public string L1VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 279, EmitDefaultValue = false)]
        public string L1VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 280, EmitDefaultValue = false)]
        public decimal L2VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 281, EmitDefaultValue = false)]
        public decimal L2VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 282, EmitDefaultValue = false)]
        public string L2VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 283, EmitDefaultValue = false)]
        public string L2VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 284, EmitDefaultValue = false)]
        public decimal L3VGrooveVerticalOffset { get; set; }
        [DataMember(Order = 285, EmitDefaultValue = false)]
        public decimal L3VGrooveHorizontalOffset { get; set; }
        [DataMember(Order = 286, EmitDefaultValue = false)]
        public string L3VGrooveVerticalStartStop { get; set; }
        [DataMember(Order = 287, EmitDefaultValue = false)]
        public string L3VGrooveHorizontalStartStop { get; set; }
        [DataMember(Order = 288, EmitDefaultValue = false)]
        public string CustEtchingMsg1 { get; set; }
        [DataMember(Order = 289, EmitDefaultValue = false)]
        public string CustEtchingMsg2 { get; set; }
        [DataMember(Order = 290, EmitDefaultValue = false)]
        public string CustEtchingMsg3 { get; set; }
        [DataMember(Order = 291, EmitDefaultValue = false)]
        public string CustEtchingMsg4 { get; set; }
        [DataMember(Order = 292, EmitDefaultValue = false)]
        public string CustEtchingMsg5 { get; set; }
        [DataMember(Order = 293, EmitDefaultValue = false)]
        public string QCInspect { get; set; }
        [DataMember(Order = 294, EmitDefaultValue = false)]
        public decimal PickingItemNumber { get; set; }
        [DataMember(Order = 295, EmitDefaultValue = false)]
        public string BasePickingLocation { get; set; }
        [DataMember(Order = 296, EmitDefaultValue = false)]
        public string Airspace1Tape { get; set; }
        [DataMember(Order = 297, EmitDefaultValue = false)]
        public string EnergyPanel { get; set; }
        [DataMember(Order = 298, EmitDefaultValue = false)]
        public string ShipBoxNumber { get; set; }
        [DataMember(Order = 299, EmitDefaultValue = false)]
        public string ShipMethod { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string Lite1LamiMixedTreatment { get; set; }
        [DataMember(Order = 301, EmitDefaultValue = false)]
        public string Lite1LamiColorSurface { get; set; }
        [DataMember(Order = 302, EmitDefaultValue = false)]
        public string Lite1LamiPatternSurface { get; set; }
        [DataMember(Order = 303, EmitDefaultValue = false)]
        public string Lite1LamiCoatingSurface { get; set; }
        [DataMember(Order = 304, EmitDefaultValue = false)]
        public string Lite2LamiMixedTreatment { get; set; }
        [DataMember(Order = 305, EmitDefaultValue = false)]
        public string Lite2LamiColorSurface { get; set; }
        [DataMember(Order = 306, EmitDefaultValue = false)]
        public string Lite2LamiPatternSurface { get; set; }
        [DataMember(Order = 307, EmitDefaultValue = false)]
        public string Lite2LamiCoatingSurface { get; set; }
        [DataMember(Order = 308, EmitDefaultValue = false)]
        public string Lite3LamiMixedTreatment { get; set; }
        [DataMember(Order = 309, EmitDefaultValue = false)]
        public string Lite3LamiColorSurface { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string Lite3LamiPatternSurface { get; set; }
        [DataMember(Order = 311, EmitDefaultValue = false)]
        public string Lite3LamiCoatingSurface { get; set; }
        [DataMember(Order = 312, EmitDefaultValue = false)]
        public string ButylColor { get; set; }
        [DataMember(Order = 313, EmitDefaultValue = false)]
        public string SiliconeColor { get; set; }
        [DataMember(Order = 314, EmitDefaultValue = false)]
        public string IGCCCertificationFlag { get; set; }
        [DataMember(Order = 315, EmitDefaultValue = false)]
        public decimal Lite1LamiThickness1 { get; set; }
        [DataMember(Order = 316, EmitDefaultValue = false)]
        public decimal Lite1LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 317, EmitDefaultValue = false)]
        public decimal Lite1LamiThickness2 { get; set; }
        [DataMember(Order = 318, EmitDefaultValue = false)]
        public decimal Lite2LamiThickness1 { get; set; }
        [DataMember(Order = 319, EmitDefaultValue = false)]
        public decimal Lite2LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public decimal Lite2LamiThickness2 { get; set; }
        [DataMember(Order = 321, EmitDefaultValue = false)]
        public decimal Lite3LamiThickness1 { get; set; }
        [DataMember(Order = 322, EmitDefaultValue = false)]
        public decimal Lite3LamiInnerLayerThickness { get; set; }
        [DataMember(Order = 323, EmitDefaultValue = false)]
        public decimal Lite3LamiThickness2 { get; set; }
        [DataMember(Order = 324, EmitDefaultValue = false)]
        public string IGCaps { get; set; }
        [DataMember(Order = 325, EmitDefaultValue = false)]
        public string CornerClipping { get; set; }
        [DataMember(Order = 326, EmitDefaultValue = false)]
        public decimal EndItemPickItem { get; set; }
        [DataMember(Order = 327, EmitDefaultValue = false)]
        public decimal IQLite1DiscretePart { get; set; }
        [DataMember(Order = 328, EmitDefaultValue = false)]
        public decimal IQLite2DiscretePart { get; set; }
        [DataMember(Order = 329, EmitDefaultValue = false)]
        public decimal IQLite3DiscretePart { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public string Lite1RawPickLocation { get; set; }
        [DataMember(Order = 331, EmitDefaultValue = false)]
        public string Lite2RawPickLocation { get; set; }
        [DataMember(Order = 332, EmitDefaultValue = false)]
        public string Lite3RawPickLocation { get; set; }
        [DataMember(Order = 333, EmitDefaultValue = false)]
        public string EdiLabelMsg1 { get; set; }
        [DataMember(Order = 334, EmitDefaultValue = false)]
        public string EdiLabelMsg2 { get; set; }
        [DataMember(Order = 335, EmitDefaultValue = false)]
        public string EdiLabelMsg3 { get; set; }
        [DataMember(Order = 336, EmitDefaultValue = false)]
        public string EdiLabelMsg4 { get; set; }
        [DataMember(Order = 337, EmitDefaultValue = false)]
        public string EdiLabelMsg5 { get; set; }
        [DataMember(Order = 338, EmitDefaultValue = false)]
        public string EdiLabelMsg6 { get; set; }
        [DataMember(Order = 339, EmitDefaultValue = false)]
        public string EdiLabelMsg7 { get; set; }
        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string EdiLabelMsg8 { get; set; }
        [DataMember(Order = 341, EmitDefaultValue = false)]
        public string EdiLabelMsg9 { get; set; }
        [DataMember(Order = 342, EmitDefaultValue = false)]
        public string EdiLabelMsg10 { get; set; }

        [DataMember(Order = 3330, EmitDefaultValue = false)]
        public string EdiLabelMsg11 { get; set; }
        [DataMember(Order = 3340, EmitDefaultValue = false)]
        public string EdiLabelMsg12 { get; set; }
        [DataMember(Order = 3350, EmitDefaultValue = false)]
        public string EdiLabelMsg13 { get; set; }
        [DataMember(Order = 3360, EmitDefaultValue = false)]
        public string EdiLabelMsg14 { get; set; }
        [DataMember(Order = 3370, EmitDefaultValue = false)]
        public string EdiLabelMsg15 { get; set; }
        [DataMember(Order = 3380, EmitDefaultValue = false)]
        public string EdiLabelMsg16 { get; set; }
        [DataMember(Order = 3390, EmitDefaultValue = false)]
        public string EdiLabelMsg17 { get; set; }
        [DataMember(Order = 3400, EmitDefaultValue = false)]
        public string EdiLabelMsg18 { get; set; }
        [DataMember(Order = 3410, EmitDefaultValue = false)]
        public string EdiLabelMsg19 { get; set; }
        [DataMember(Order = 3420, EmitDefaultValue = false)]
        public string EdiLabelMsg20 { get; set; }

        [DataMember(Order = 333, EmitDefaultValue = false)]
        public string EdiLabelMsg21 { get; set; }
        [DataMember(Order = 334, EmitDefaultValue = false)]
        public string EdiLabelMsg22 { get; set; }
        [DataMember(Order = 335, EmitDefaultValue = false)]
        public string EdiLabelMsg23 { get; set; }
        [DataMember(Order = 336, EmitDefaultValue = false)]
        public string EdiLabelMsg24 { get; set; }
        [DataMember(Order = 337, EmitDefaultValue = false)]
        public string EdiLabelMsg25 { get; set; }
        [DataMember(Order = 338, EmitDefaultValue = false)]
        public string EdiLabelMsg26 { get; set; }
        [DataMember(Order = 339, EmitDefaultValue = false)]
        public string EdiLabelMsg27 { get; set; }
        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string EdiLabelMsg28 { get; set; }
        [DataMember(Order = 341, EmitDefaultValue = false)]
        public string EdiLabelMsg29 { get; set; }
        [DataMember(Order = 342, EmitDefaultValue = false)]
        public string EdiLabelMsg30 { get; set; }


        [DataMember(Order = 343, EmitDefaultValue = false)]
        public string EdiEtchMsg1 { get; set; }
        [DataMember(Order = 344, EmitDefaultValue = false)]
        public string EdiEtchMsg2 { get; set; }
        [DataMember(Order = 345, EmitDefaultValue = false)]
        public string EdiEtchMsg3 { get; set; }
        [DataMember(Order = 346, EmitDefaultValue = false)]
        public string EdiEtchMsg4 { get; set; }
        [DataMember(Order = 347, EmitDefaultValue = false)]
        public string EdiEtchMsg5 { get; set; }
        [DataMember(Order = 348, EmitDefaultValue = false)]
        public string EdiEtchMsg6 { get; set; }
        [DataMember(Order = 349, EmitDefaultValue = false)]
        public string EdiEtchMsg7 { get; set; }
        [DataMember(Order = 350, EmitDefaultValue = false)]
        public string EdiEtchMsg8 { get; set; }
        [DataMember(Order = 351, EmitDefaultValue = false)]
        public string EdiEtchMsg9 { get; set; }
        [DataMember(Order = 352, EmitDefaultValue = false)]
        public string EdiEtchMsg10 { get; set; }
        [DataMember(Order = 353, EmitDefaultValue = false)]
        public decimal CrateId { get; set; }
        [DataMember(Order = 354, EmitDefaultValue = false)]
        public string CrateType { get; set; }
        [DataMember(Order = 355, EmitDefaultValue = false)]
        public string ReasonCode { get; set; }
        [DataMember(Order = 356, EmitDefaultValue = false)]
        public decimal CustBillToNum { get; set; }
        [DataMember(Order = 357, EmitDefaultValue = false)]
        public string GenericSeqNum { get; set; }
        [DataMember(Order = 358, EmitDefaultValue = false)]
        public int RequestedDeliveryTime { get; set; }
        [DataMember(Order = 359, EmitDefaultValue = false)]
        public string BatchName { get; set; }
        [DataMember(Order = 360, EmitDefaultValue = false)]
        public string BoxingOption { get; set; }
        [DataMember(Order = 361, EmitDefaultValue = false)]
        public string RunOption { get; set; }
        [DataMember(Order = 362, EmitDefaultValue = false)]
        public decimal CustomerShipTo { get; set; }
        [DataMember(Order = 363, EmitDefaultValue = false)]
        public string Crate { get; set; }
        [DataMember(Order = 364, EmitDefaultValue = false)]
        public decimal TotalWeight { get; set; }
        [DataMember(Order = 365, EmitDefaultValue = false)]
        public decimal Lite1_NumHoles { get; set; }
        [DataMember(Order = 366, EmitDefaultValue = false)]
        public decimal Lite2_NumHoles { get; set; }
        [DataMember(Order = 367, EmitDefaultValue = false)]
        public decimal Lite3_NumHoles { get; set; }
        [DataMember(Order = 368, EmitDefaultValue = false)]
        public string MailingName { get; set; }
        [DataMember(Order = 369, EmitDefaultValue = false)]
        public decimal OrigCustNumSoldTo { get; set; }
        [DataMember(Order = 370, EmitDefaultValue = false)]
        public string NFRC2 { get; set; }
        [DataMember(Order = 371, EmitDefaultValue = false)]
        public string PickingNotes { get; set; }
        [DataMember(Order = 372, EmitDefaultValue = false)]
        public string PackingNotes { get; set; }
        [DataMember(Order = 373, EmitDefaultValue = false)]
        public string SCHED_VALUE03 { get; set; }
        [DataMember(Order = 374, EmitDefaultValue = false)]
        public string SCHED_VALUE04 { get; set; }
        [DataMember(Order = 3741, EmitDefaultValue = false)]
        public string SCHED_VALUE05 { get; set; }

        [DataMember(Order = 3742, EmitDefaultValue = false)]
        public string SCHED_VALUE06 { get; set; }

        [DataMember(Order = 3743, EmitDefaultValue = false)]
        public string SCHED_VALUE07 { get; set; }

        [DataMember(Order = 3744, EmitDefaultValue = false)]
        public string SCHED_VALUE08 { get; set; }

        [DataMember(Order = 3745, EmitDefaultValue = false)]
        public string SCHED_VALUE09 { get; set; }

        [DataMember(Order = 3746, EmitDefaultValue = false)]
        public string SCHED_VALUE10 { get; set; }
        
        [DataMember(Order = 375, EmitDefaultValue = false)]
        public string OriginalBranchPlant { get; set; }

        [DataMember(Order = 376, EmitDefaultValue = false)]
        public string CustomerSuppliedGlass { get; set; }
        [DataMember(Order = 377, EmitDefaultValue = false)]
        public string QCInspectFlag { get; set; }
        [DataMember(Order = 378, EmitDefaultValue = false)]
        public string EdgeDeleteWidth { get; set; }
        [DataMember(Order = 379, EmitDefaultValue = false)]
        public decimal ConfigID { get; set; }

        [DataMember(Order = 380, EmitDefaultValue = false)]
        public string FabDrawing { get; set; }
        [DataMember(Order = 381, EmitDefaultValue = false)]
        public string PLSide { get; set; }
        [DataMember(Order = 382, EmitDefaultValue = false)]
        public string FabDesc { get; set; }
        [DataMember(Order = 383, EmitDefaultValue = false)]
        public decimal SqftActual { get; set; }

        [DataMember(Order = 390, EmitDefaultValue = false)]
        public string ActualIGOverallThickness { get; set; }
        [DataMember(Order = 391, EmitDefaultValue = false)]
        public decimal IGUnitOffsetAmount { get; set; }
        [DataMember(Order = 392, EmitDefaultValue = false)]
        public string IGUnitShapeOffsetSide { get; set; }

        [DataMember(Order = 393, EmitDefaultValue = false)]
        public string EdgeWorkSides { get; set; }

        [DataMember(Order = 394, EmitDefaultValue = false)]
        public string TaglessLogo { get; set; }

        [DataMember(Order = 395, EmitDefaultValue = false)]
        public decimal Lite1_Inventory { get; set; }

        [DataMember(Order = 396, EmitDefaultValue = false)]
        public string Lite1_ReleaseNote { get; set; }

        [DataMember(Order = 397, EmitDefaultValue = false)]
        public string Lite1_ReleaseTimeNote { get; set; }

        [DataMember(Order = 398, EmitDefaultValue = false)]
        public decimal Lite2_Inventory { get; set; }

        [DataMember(Order = 399, EmitDefaultValue = false)]
        public string Lite2_ReleaseNote { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public string Lite2_ReleaseTimeNote { get; set; }

        [DataMember(Order = 401, EmitDefaultValue = false)]
        public decimal Lite3_Inventory { get; set; }

        [DataMember(Order = 402, EmitDefaultValue = false)]
        public string Lite3_ReleaseNote { get; set; }

        [DataMember(Order = 403, EmitDefaultValue = false)]
        public string Lite3_ReleaseTimeNote { get; set; }

        [DataMember(Order = 404, EmitDefaultValue = false)]
        public string ChangedNote { get; set; }

        [DataMember(Order = 405, EmitDefaultValue = false)]
        public string RemakeFlag { get; set; }
        [DataMember(Order = 406, EmitDefaultValue = false)]
        public decimal VerticalBarOffset { get; set; }

        [DataMember(Order = 407, EmitDefaultValue = false)]
        public decimal HorizontalBarOffset { get; set; }

        [DataMember(Order = 408, EmitDefaultValue = false)]
        public string Lite1Plant { get; set; }
        [DataMember(Order = 409, EmitDefaultValue = false)]
        public string Lite2Plant { get; set; }
        [DataMember(Order = 410, EmitDefaultValue = false)]
        public string Lite3Plant { get; set; }

        [DataMember(Order = 411, EmitDefaultValue = false)]
        public decimal Lite1Weight { get; set; }
        [DataMember(Order = 412, EmitDefaultValue = false)]
        public decimal Lite2Weight { get; set; }
        [DataMember(Order = 413, EmitDefaultValue = false)]
        public decimal Lite3Weight { get; set; }

        [DataMember(Order = 414, EmitDefaultValue = false)]
        public string OrigCustomerPO { get; set; }
        [DataMember(Order = 415, EmitDefaultValue = false)]
        public string ShipCompleteId { get; set; }

        [DataMember(Order = 416, EmitDefaultValue = false)]
        public string CustomBarAirspace1 { get; set; }

        [DataMember(Order = 417, EmitDefaultValue = false)]
        public string CustomBarAirspace2 { get; set; }

        [DataMember(Order = 416, EmitDefaultValue = false)]
        public string DeliveryInstruction1 { get; set; }

        [DataMember(Order = 417, EmitDefaultValue = false)]
        public string DeliveryInstruction2 { get; set; }

        [DataMember(Order = 418, EmitDefaultValue = false)]
        public string PressureAdjusted { get; set; }

        [DataMember(Order = 419, EmitDefaultValue = false)]
        public string Lite1_SandblastDraw { get; set; }

        [DataMember(Order = 420, EmitDefaultValue = false)]
        public string Lite2_SandblastDraw { get; set; }

        [DataMember(Order = 421, EmitDefaultValue = false)]
        public string Lite3_SandblastDraw { get; set; }

        [DataMember(Order = 501, EmitDefaultValue = false)]
        public string Lite1LamiCustom { get; set; }
        [DataMember(Order = 502, EmitDefaultValue = false)]
        public string Lite2LamiCustom { get; set; }
        [DataMember(Order = 503, EmitDefaultValue = false)]
        public string Lite3LamiCustom { get; set; }

        [DataMember(Order = 504, EmitDefaultValue = false)]
        public string VigCapLocation { get; set; }
        [DataMember(Order = 505, EmitDefaultValue = false)]
        public string VigCapType { get; set; }
        [DataMember(Order = 505, EmitDefaultValue = false)]
        public string WaterJet { get; set; }


        public ItemAttributes ItemAttributes { get; set; }

    }
}
