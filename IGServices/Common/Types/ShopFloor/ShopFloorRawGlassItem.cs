using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class ShopFloorRawGlassItems : List<ShopFloorRawGlassItem>
    {
    }

    [DataContract(Namespace = "")]
    public class ShopFloorRawGlassItem
    {
        public ShopFloorRawGlassItem()
        {
            ItemNumber = "";
            BusinessUnit = "";
            PartType = "";
            WorderNum = 0;
            DocType = "";
            Desc1 = "";
            Desc2 = "";
            Location = "";
            OrderQty = 0;
            QtyAvail = 0;
            LiteThickness = 0;
            LiteHeat = "";
            LiteColor = "";
            FirstSrfCoating = "";
            SecondSrfCoating = "";
            LitePattern = "";
            BaseDim = 0;
            BaseN = 0;
            BaseD = 0;
            LeftDim = 0;
            LeftN = 0;
            LeftD = 0;
            RightDim = 0;
            RightN = 0;
            RightD = 0;
            TopDim = 0;
            TopN = 0;
            TopD = 0;
            ShapeNum = "";
            E1BatchID = "";
            S7ITM = 0;
            S7Z58SCN = 0;
            WMOPSQ = 0;
        }

        [DataMember(Order = 10)]
        public string ItemNumber { get; set; }
        [DataMember(Order = 20)]
        public string BusinessUnit { get; set; }
        [DataMember(Order = 30)]
        public string PartType { get; set; }
        [DataMember(Order = 40)]
        public decimal WorderNum { get; set; }
        [DataMember(Order = 50)]
        public string DocType { get; set; }
        [DataMember(Order = 60)]
        public string Desc1 { get; set; }
        [DataMember(Order = 70)]
        public string Desc2 { get; set; }
        [DataMember(Order = 80)]
        public string Location { get; set; }
        [DataMember(Order = 90)]
        public decimal OrderQty { get; set; }
        [DataMember(Order = 100)]
        public decimal QtyAvail { get; set; }
        [DataMember(Order = 110)]
        public decimal LiteThickness { get; set; }
        [DataMember(Order = 120)]
        public string LiteHeat { get; set; }
        [DataMember(Order = 130)]
        public string LiteColor { get; set; }
        [DataMember(Order = 140)]
        public string FirstSrfCoating { get; set; }
        [DataMember(Order = 150)]
        public string SecondSrfCoating { get; set; }
        [DataMember(Order = 160)]
        public string LitePattern { get; set; }
        [DataMember(Order = 170)]
        public decimal BaseDim { get; set; }
        [DataMember(Order = 180)]
        public decimal BaseN { get; set; }
        [DataMember(Order = 190)]
        public decimal BaseD { get; set; }
        [DataMember(Order = 200)]
        public decimal LeftDim { get; set; }
        [DataMember(Order = 210)]
        public decimal LeftN { get; set; }
        [DataMember(Order = 220)]
        public decimal LeftD { get; set; }
        [DataMember(Order = 230)]
        public decimal RightDim { get; set; }
        [DataMember(Order = 240)]
        public decimal RightN { get; set; }
        [DataMember(Order = 250)]
        public decimal RightD { get; set; }
        [DataMember(Order = 260)]
        public decimal TopDim { get; set; }
        [DataMember(Order = 270)]
        public decimal TopN { get; set; }
        [DataMember(Order = 280)]
        public decimal TopD { get; set; }

        [DataMember(Order = 290)]
        public string ShapeNum { get; set; }
        [DataMember(Order = 300)]
        public string E1BatchID { get; set; }
        [DataMember(Order = 310)]
        public decimal S7ITM { get; set; }
        [DataMember(Order = 320)]
        public decimal S7Z58SCN { get; set; }
        [DataMember(Order = 330)]
        public decimal WMOPSQ { get; set; }
    }
}
