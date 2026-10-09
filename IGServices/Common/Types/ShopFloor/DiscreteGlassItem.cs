using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class DiscreteGlassItems : List<DiscreteGlassItem>
    {
    }

    [DataContract(Namespace = "")]
    public class DiscreteGlassItem
    {

        public DiscreteGlassItem()
        {
            PlantID = "";
        }

        [DataMember(Order = 10)]
        public string PlantID { get; set; }
        [DataMember(Order = 12)]
        public string ItemNum { get; set; }
        [DataMember(Order = 14)]
        public string StockingType { get; set; }
        [DataMember(Order = 16)]
        public string CategoryCode { get; set; }

        [DataMember(Order = 20)]
        public string UnitType { get; set; }
        [DataMember(Order = 22)]
        public string ProductType { get; set; }
        [DataMember(Order = 24)]
        public string CardinalProductType { get; set; }
        [DataMember(Order = 26)]
        public string VolumeIndicator { get; set; }
        [DataMember(Order = 28)]
        public decimal OverallThickness { get; set; }

        [DataMember(Order = 30)]
        public string FlexItemDesc1 { get; set; }
        [DataMember(Order = 31)]
        public string FlexItemDesc2 { get; set; }
        [DataMember(Order = 32)]
        public string FlexItemDesc3 { get; set; }
        [DataMember(Order = 33)]
        public string FlexItemDesc4 { get; set; }
        [DataMember(Order = 34)]
        public string FlexItemDesc5 { get; set; }
        [DataMember(Order = 35)]
        public string FlexItemDesc6 { get; set; }
        [DataMember(Order = 36)]
        public string FlexItemDesc7 { get; set; }
        [DataMember(Order = 37)]
        public string FlexItemDesc8 { get; set; }

        [DataMember(Order = 40)]
        public decimal SupplierNum { get; set; }
        [DataMember(Order = 42)]
        public string SupplierName { get; set; }

        [DataMember(Order = 50)]
        public decimal PlannerNum { get; set; }
        [DataMember(Order = 51)]
        public string PlannerName { get; set; }
        [DataMember(Order = 52)]
        public decimal SoldCustomerNum { get; set; }
        [DataMember(Order = 53)]
        public string SoldCustomerName { get; set; }
        [DataMember(Order = 54)]
        public decimal ShipCustomerNum { get; set; }
        [DataMember(Order = 55)]
        public string ShipCustomerName { get; set; }
        [DataMember(Order = 56)]
        public decimal OrigSoldCustomerNum { get; set; }
        [DataMember(Order = 57)]
        public string OrigSoldCustomerName { get; set; }

        [DataMember(Order = 60)]
        public string CustomerXrefPartNum { get; set; }
        [DataMember(Order = 61)]
        public string CustomerXrefType { get; set; }
        [DataMember(Order = 62)]
        public string CustomerPartNum { get; set; }

        [DataMember(Order = 80)]
        public string Dimensions { get; set; }
        [DataMember(Order = 82)]
        public decimal ShapeNum { get; set; }

        [DataMember(Order = 84)]
        public decimal BaseL { get; set; }
        [DataMember(Order = 86)]
        public decimal BaseN { get; set; }
        [DataMember(Order = 88)]
        public decimal BaseD { get; set; }
        [DataMember(Order = 90)]
        public decimal BaseMM { get; set; }
        [DataMember(Order = 92)]
        public decimal BaseDec { get; set; }

        [DataMember(Order = 94)]
        public decimal LeftL { get; set; }
        [DataMember(Order = 96)]
        public decimal LeftN { get; set; }
        [DataMember(Order = 98)]
        public decimal LeftD { get; set; }
        [DataMember(Order = 100)]
        public decimal LeftMM { get; set; }
        [DataMember(Order = 102)]
        public decimal LeftDec { get; set; }

        [DataMember(Order = 104)]
        public decimal RightL { get; set; }
        [DataMember(Order = 106)]
        public decimal RightN { get; set; }
        [DataMember(Order = 108)]
        public decimal RightD { get; set; }
        [DataMember(Order = 110)]
        public decimal RightMM { get; set; }
        [DataMember(Order = 112)]
        public decimal RightDec { get; set; }

        [DataMember(Order = 114)]
        public decimal TopL { get; set; }
        [DataMember(Order = 116)]
        public decimal TopN { get; set; }
        [DataMember(Order = 118)]
        public decimal TopD { get; set; }
        [DataMember(Order = 120)]
        public decimal TopMM { get; set; }
        [DataMember(Order = 122)]
        public decimal TopDec { get; set; }

        [DataMember(Order = 124)]
        public decimal Side1L { get; set; }
        [DataMember(Order = 126)]
        public decimal Side1N { get; set; }
        [DataMember(Order = 128)]
        public decimal Side1D { get; set; }
        [DataMember(Order = 130)]
        public decimal Side1MM { get; set; }
        [DataMember(Order = 132)]
        public decimal Side1Dec { get; set; }

        [DataMember(Order = 134)]
        public decimal Side2L { get; set; }
        [DataMember(Order = 136)]
        public decimal Side2N { get; set; }
        [DataMember(Order = 138)]
        public decimal Side2D { get; set; }
        [DataMember(Order = 140)]
        public decimal Side2MM { get; set; }
        [DataMember(Order = 142)]
        public decimal Side2Dec { get; set; }

        [DataMember(Order = 150)]
        public decimal ActualSqft { get; set; }
        [DataMember(Order = 152)]
        public decimal BillableSqft { get; set; }

        [DataMember(Order = 160)]
        public string Construction { get; set; }

        [DataMember(Order = 170)]
        public decimal Lite1_Thick { get; set; }
        [DataMember(Order = 172)]
        public string Lite1_Color { get; set; }
        [DataMember(Order = 174)]
        public string Lite1_Heat { get; set; }
        [DataMember(Order = 176)]
        public string Lite1_1stSrfCoat { get; set; }
        [DataMember(Order = 178)]
        public string Lite1_2ndSrfCoat { get; set; }
        [DataMember(Order = 180)]
        public string Lite1_Pattern { get; set; }
        [DataMember(Order = 182)]
        public string Lite1_Lami { get; set; }
        [DataMember(Order = 184)]
        public string Lite1_Quality { get; set; }
        [DataMember(Order = 186)]
        public string Lite1_AnsiiLogo { get; set; }
        [DataMember(Order = 188)]
        public string Lite1_TempLogo { get; set; }

        [DataMember(Order = 190)]
        public decimal Lite2_Thick { get; set; }
        [DataMember(Order = 192)]
        public string Lite2_Color { get; set; }
        [DataMember(Order = 194)]
        public string Lite2_Heat { get; set; }
        [DataMember(Order = 196)]
        public string Lite2_3rdSrfCoat { get; set; }
        [DataMember(Order = 198)]
        public string Lite2_4thSrfCoat { get; set; }
        [DataMember(Order = 200)]
        public string Lite2_Pattern { get; set; }
        [DataMember(Order = 202)]
        public string Lite2_Lami { get; set; }

        [DataMember(Order = 220)]
        public decimal Lite3_Thick { get; set; }
        [DataMember(Order = 222)]
        public string Lite3_Color { get; set; }
        [DataMember(Order = 224)]
        public string Lite3_Heat { get; set; }
        [DataMember(Order = 226)]
        public string Lite3_5thSrfCoat { get; set; }
        [DataMember(Order = 228)]
        public string Lite3_6thSrfCoat { get; set; }
        [DataMember(Order = 230)]
        public string Lite3_Pattern { get; set; }
        [DataMember(Order = 232)]
        public string Lite3_Lami { get; set; }

        [DataMember(Order = 250)]
        public decimal Spacer1_Size { get; set; }
        [DataMember(Order = 252)]
        public string Spacer1_Type { get; set; }
        [DataMember(Order = 254)]
        public string MCode1_Air1 { get; set; }
        [DataMember(Order = 256)]
        public string MCode1_Air1Ort { get; set; }
        [DataMember(Order = 258)]
        public string MCode2_Air1 { get; set; }
        [DataMember(Order = 260)]
        public string MCode2_Air1Ort { get; set; }
        [DataMember(Order = 262)]
        public string CCode_Air1 { get; set; }

        [DataMember(Order = 280)]
        public decimal Spacer2_Size { get; set; }
        [DataMember(Order = 282)]
        public string Spacer2_Type { get; set; }
        [DataMember(Order = 284)]
        public string MCode1_Air2 { get; set; }
        [DataMember(Order = 286)]
        public string MCode1_Air2Ort { get; set; }
        [DataMember(Order = 288)]
        public string MCode2_Air2 { get; set; }
        [DataMember(Order = 290)]
        public string MCode2_Air2Ort { get; set; }
        [DataMember(Order = 292)]
        public string CCode_Air2 { get; set; }

        [DataMember(Order = 300)]
        public string Spacer1_GasMix { get; set; }
        [DataMember(Order = 302)]
        public string Tubes { get; set; }


        [DataMember(Order = 320)]
        public string IntPrsvCB { get; set; }
        [DataMember(Order = 322)]
        public string IntPrsvColor { get; set; }
        [DataMember(Order = 324)]
        public string ExtPrsvCB { get; set; }
        [DataMember(Order = 326)]
        public string ExtPrsvColor { get; set; }
        [DataMember(Order = 328)]
        public string OverlayBar { get; set; }

        [DataMember(Order = 340)]
        public string GridStyle { get; set; }
        [DataMember(Order = 342)]
        public string DrawingID { get; set; }
        [DataMember(Order = 344)]
        public decimal Grilles_Horz { get; set; }
        [DataMember(Order = 346)]
        public decimal Grilles_Vert { get; set; }
        [DataMember(Order = 348)]
        public decimal OpenWide { get; set; }
        [DataMember(Order = 350)]
        public decimal OpenHigh { get; set; }
        [DataMember(Order = 350)]
        public decimal OpenOdd { get; set; }

        [DataMember(Order = 380)]
        public string NFRCNumber { get; set; }
        [DataMember(Order = 382)]
        public string NFRCNumber2 { get; set; }

        [DataMember(Order = 400)]
        public string RackType { get; set; }
        [DataMember(Order = 402)]
        public decimal LitesPerRack { get; set; }
        [DataMember(Order = 404)]
        public decimal RackRows { get; set; }
        [DataMember(Order = 406)]
        public decimal RackTiers { get; set; }
        [DataMember(Order = 408)]
        public string RackOrientVH { get; set; }
        [DataMember(Order = 410)]
        public string RackOrientIO { get; set; }
        [DataMember(Order = 412)]
        public string Crating { get; set; }

        [DataMember(Order = 500)]
        public DateTime UpdateDate { get; set; }

    }
}
