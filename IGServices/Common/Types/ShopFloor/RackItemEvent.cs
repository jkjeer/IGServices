using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RackItemEvents : List<RackItemEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class RackItemEvent
    {

        public RackItemEvent()
        {
            EventId = 0;
            EventCode = "";
            EventDate = null;
            RackId = 0;
            SerialNum = "";
            Status = "";
            Quantity = 0;
            ItemNo = 0;
            LotNo = "";
            Location = "";
            GLCategory = "";
            MfgDate = null;
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
        }

        [DataMember(Order = 1)]
        public decimal EventId { get; set; }
        [DataMember(Order = 2)]
        public string EventCode { get; set; }
        [DataMember(Order = 3)]
        public DateTime? EventDate { get; set; }
        [DataMember(Order = 4)]
        public decimal RackId { get; set; }
        [DataMember(Order = 5)]
        public string SerialNum { get; set; }
        [DataMember(Order = 6)]
        public string Status { get; set; }
        [DataMember(Order = 7)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 8)]
        public decimal ItemNo { get; set; }
        [DataMember(Order = 9)]
        public string LotNo { get; set; }
        [DataMember(Order = 10)]
        public string Location { get; set; }
        [DataMember(Order = 11)]
        public string GLCategory { get; set; }
        [DataMember(Order = 12)]
        public DateTime? MfgDate { get; set; }
        [DataMember(Order = 13)]
        public decimal Lite1Thickness { get; set; }
        [DataMember(Order = 14)]
        public string Lite1Heat { get; set; }
        [DataMember(Order = 15)]
        public string Lite1Color { get; set; }
        [DataMember(Order = 16)]
        public string Lite1FirstSrfCoat { get; set; }
        [DataMember(Order = 17)]
        public string Lite1SecondSrfCoat { get; set; }
        [DataMember(Order = 18)]
        public string Lite1Pattern { get; set; }
        [DataMember(Order = 19)]
        public string Lite1Lami { get; set; }
        [DataMember(Order = 20)]
        public string Lite1Quality { get; set; }
        [DataMember(Order = 21)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 22)]
        public decimal BaseDim { get; set; }
        [DataMember(Order = 23)]
        public decimal LeftDim { get; set; }
        [DataMember(Order = 24)]
        public decimal RightDim { get; set; }
        [DataMember(Order = 25)]
        public decimal TopDim { get; set; }
        [DataMember(Order = 26)]
        public decimal Side1Dim { get; set; }
        [DataMember(Order = 27)]
        public decimal Side2Dim { get; set; }
        [DataMember(Order = 28)]
        public decimal ActualSqft { get; set; }
        [DataMember(Order = 29)]
        public decimal BillableSqft { get; set; }
        [DataMember(Order = 30)]
        public decimal GlassLbs { get; set; }
    }
}