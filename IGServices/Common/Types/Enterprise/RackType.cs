using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackTypes : List<RackType>
    {
    }

    [DataContract(Namespace = "")]
    public class RackType
    {
        [DataMember(Order = 1)]
        public string RackTypeCode { get; set; }
        [DataMember(Order = 2)]
        public string RackTypeName { get; set; }
        [DataMember(Order = 3)]
        public string RackTypeStatus { get; set; }
        [DataMember(Order = 4)]
        public string CostCenter { get; set; }

        [DataMember(Order = 20)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 21)]
        public decimal AddressShipToNum { get; set; }

        [DataMember(Order = 25)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 30)]
        public decimal TareWeight { get; set; }
        [DataMember(Order = 31)]
        public decimal MinTotalWeight { get; set; }
        [DataMember(Order = 32)]
        public decimal MaxTotalWeight { get; set; }
        [DataMember(Order = 33)]
        public string WeightUom { get; set; }

        [DataMember(Order = 40)]
        public string VolumeUom { get; set; }
        [DataMember(Order = 41)]
        public decimal GrossHeight { get; set; }
        [DataMember(Order = 42)]
        public decimal GrossWidth { get; set; }
        [DataMember(Order = 43)]
        public decimal GrossDepth { get; set; }
        [DataMember(Order = 44)]
        public decimal GlassMinLength { get; set; }
        [DataMember(Order = 45)]
        public decimal GlassMaxLength { get; set; }
        [DataMember(Order = 46)]
        public decimal GlassMinHeight { get; set; }
        [DataMember(Order = 47)]
        public decimal GlassMaxHeight { get; set; }
        [DataMember(Order = 48)]
        public decimal GlassMinWidth { get; set; }
        [DataMember(Order = 49)]
        public decimal GlassMaxWidth { get; set; }
        [DataMember(Order = 50)]
        public string DimensionUom { get; set; }
        [DataMember(Order = 51)]
        public decimal GrossCubicDimensions { get; set; }

        [DataMember(Order = 55)]
        public decimal RackSides { get; set; }
        [DataMember(Order = 56)]
        public decimal RackPositions { get; set; }
        [DataMember(Order = 57)]
        public decimal RackStacks { get; set; }
        [DataMember(Order = 58)]
        public decimal RackRows { get; set; }
        [DataMember(Order = 59)]
        public decimal RackSlots { get; set; }

        [DataMember(Order = 60)]
        public string ShapeFlag { get; set; }
        [DataMember(Order = 61)]
        public string AnnealedFlag { get; set; }
        [DataMember(Order = 62)]
        public string CorpFlag { get; set; }
        [DataMember(Order = 63)]
        public string ShippableFlag { get; set; }

        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }
    }
}
