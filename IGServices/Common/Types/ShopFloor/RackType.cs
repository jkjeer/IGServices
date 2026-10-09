using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class RackTypes : List<RackType>
    {
    }

    public class RackType
    {
        public RackType()
        {
            RackTypeCode = "";
            RackTypeDesc = "";
            RackTypeStatus = "";
            CustomerNumber = 0;
            ShipTo = 0;
            CorporateRackTypeFlag = "";
            RackTypeShippableFlag = "";
            RackTypeTareWeight = 0;
            MinTotalWeight = 0;
            MaxTotalWeight = 0;
            WeightUOM = "";
            GrossHeight = 0;
            GrossWidth = 0;
            GrossDepth = 0;
            GlassMinLength = 0;
            GlassMaxLength = 0;
            GlassMinHeight = 0;
            GlassMaxHeight = 0;
            GlassMinWidth = 0;
            GlassMaxWidth = 0;
            DimensionUOM = "";
            RackSides = 0;
            RackPositions = 0;
            RackStacks = 0;
            RackRows = 0;
            RackSlots = 0;
            RackShapeFlag = "";
            RackAnnealedFlag = "";
            ItemNumber = 0;
            GrossCubes = 0;
            VolumeUOM = "";
        }

        //[DataMember(Order = 1)]
        //public int RackTypeLedgerId { get; set; }
        [DataMember(Order = 2)]
        public string RackTypeCode { get; set; }
        [DataMember(Order = 3)]
        public string RackTypeDesc { get; set; }
        [DataMember(Order = 4)]
        public string RackTypeStatus { get; set; }
        [DataMember(Order = 5)]
        public decimal CustomerNumber { get; set; }
        [DataMember(Order = 6)]
        public decimal ShipTo { get; set; }
        [DataMember(Order = 7)]
        public string CorporateRackTypeFlag { get; set; }
        [DataMember(Order = 8)]
        public string RackTypeShippableFlag { get; set; }
        [DataMember(Order = 9)]
        public decimal RackTypeTareWeight { get; set; }
        [DataMember(Order = 10)]
        public decimal MinTotalWeight { get; set; }
        [DataMember(Order = 11)]
        public decimal MaxTotalWeight { get; set; }
        [DataMember(Order = 12)]
        public string WeightUOM { get; set; }
        [DataMember(Order = 13)]
        public decimal GrossHeight { get; set; }
        [DataMember(Order = 14)]
        public decimal GrossWidth { get; set; }
        [DataMember(Order = 15)]
        public decimal GrossDepth { get; set; }
        [DataMember(Order = 16)]
        public decimal GlassMinLength { get; set; }
        [DataMember(Order = 17)]
        public decimal GlassMaxLength { get; set; }
        [DataMember(Order = 18)]
        public decimal GlassMinHeight { get; set; }
        [DataMember(Order = 19)]
        public decimal GlassMaxHeight { get; set; }
        [DataMember(Order = 20)]
        public decimal GlassMinWidth { get; set; }
        [DataMember(Order = 21)]
        public decimal GlassMaxWidth { get; set; }
        [DataMember(Order = 22)]
        public string DimensionUOM { get; set; }
        [DataMember(Order = 23)]
        public decimal RackSides { get; set; }
        [DataMember(Order = 24)]
        public decimal RackPositions { get; set; }
        [DataMember(Order = 25)]
        public decimal RackStacks { get; set; }
        [DataMember(Order = 26)]
        public decimal RackRows { get; set; }
        [DataMember(Order = 27)]
        public decimal RackSlots { get; set; }
        [DataMember(Order = 28)]
        public string RackShapeFlag { get; set; }
        [DataMember(Order = 29)]
        public string RackAnnealedFlag { get; set; }
        [DataMember(Order = 30)]
        public decimal ItemNumber { get; set; }
        [DataMember(Order = 31)]
        public decimal GrossCubes { get; set; }
        [DataMember(Order = 32)]
        public string VolumeUOM { get; set; }
        [DataMember(Order = 33)]
        public DateTime UpdateDate { get; set; }
    }
}
