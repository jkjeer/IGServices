using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RackItems : List<RackItem>
    {
    }

    [DataContract(Namespace = "")]
    public class RackItem
    {

        public RackItem()
        {
            RackId = 0;
            SerialNum = "";
            Status = "";
            StatusDate = null;
            Quantity = 0;
            ItemNum = 0;
            LotNum = "";
            GLCategory = "";
            Lite1Thickness = 0;
            Lite1Heat = "";
            Lite1Color = "";
            Lite1FirstSrfCoat = "";
            Lite1SecondSrfCoat = "";
            Lite1Pattern = "";
            Lite1Lami = "";
            Lite1Quality = "";
            ShapeNum = 0;
            BaseDim = 0;
            LeftDim = 0;
            RightDim = 0;
            TopDim = 0;
            Side1Dim = 0;
            Side2Dim = 0;
            ActualSqft = 0;
            BillableSqft = 0;
            GlassLbs = 0;
            OrderNum = 0;
            OrderType = "";
            OrderLineNum = 0;
            WorkOrderNum = 0;
            PlantId = 0;
        }

        [DataMember(Order = 2)]
        public decimal RackId { get; set; }
        [DataMember(Order = 3)]
        public string SerialNum { get; set; }
        [DataMember(Order = 4)]
        public string Status { get; set; }
        [DataMember(Order = 5)]
        public DateTime? StatusDate { get; set; }
        [DataMember(Order = 6)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 7)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 8)]
        public string LotNum { get; set; }
        [DataMember(Order = 9)]
        public string GLCategory { get; set; }
        [DataMember(Order = 10)]
        public decimal Lite1Thickness { get; set; }
        [DataMember(Order = 11)]
        public string Lite1Heat { get; set; }
        [DataMember(Order = 12)]
        public string Lite1Color { get; set; }
        [DataMember(Order = 13)]
        public string Lite1FirstSrfCoat { get; set; }
        [DataMember(Order = 14)]
        public string Lite1SecondSrfCoat { get; set; }
        [DataMember(Order = 15)]
        public string Lite1Pattern { get; set; }
        [DataMember(Order = 16)]
        public string Lite1Lami { get; set; }
        [DataMember(Order = 17)]
        public string Lite1Quality { get; set; }
        [DataMember(Order = 18)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 19)]
        public decimal BaseDim { get; set; }
        [DataMember(Order = 20)]
        public decimal LeftDim { get; set; }
        [DataMember(Order = 21)]
        public decimal RightDim { get; set; }
        [DataMember(Order = 22)]
        public decimal TopDim { get; set; }
        [DataMember(Order = 23)]
        public decimal Side1Dim { get; set; }
        [DataMember(Order = 24)]
        public decimal Side2Dim { get; set; }
        [DataMember(Order = 25)]
        public decimal ActualSqft { get; set; }
        [DataMember(Order = 26)]
        public decimal BillableSqft { get; set; }
        [DataMember(Order = 27)]
        public decimal GlassLbs { get; set; }
        [DataMember(Order = 28)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 29)]
        public string OrderType { get; set; }
        [DataMember(Order = 30)]
        public decimal OrderLineNum { get; set; }
        [DataMember(Order = 31)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 32)]
        public int PlantId { get; set; }

    }
}
