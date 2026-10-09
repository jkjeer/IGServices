using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Name="SpacerItems", Namespace = "")]
    public class SpacerItems : CardinalCollection<SpacerItem>
    {
    }

    [DataContract(Namespace = "", Name = "SpacerItem")]
    public class SpacerItem : Item
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public SpacerType SpacerType { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal Width { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal Length { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string Bead { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Material { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string Gauge { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string Profile { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string Color { get; set; }

        [DataMember(Order = 38, EmitDefaultValue = false)]
        public string Painted { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public bool PreKeyed { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = true)]
        public decimal CostPerLinealFoot { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = true)]
        public string PrimaryLocation { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = true)]
        public decimal QtyOnHand { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = true)]
        public DateTime UpdateDate { get; set; }

        //// DEPRECATED
        ////[DataMember(Order = 56, EmitDefaultValue = true)]
        //public Boolean Endur { get; set; }

        ////[DataMember(Order = 100, EmitDefaultValue = false)]
        //public string NarrowMargin { get; set; }

        ////[DataMember(Order = 110, EmitDefaultValue = false)]
        //public string NarrowMarginDesc { get; set; }

        ////[DataMember(Order = 110, EmitDefaultValue = false)]
        //public decimal Guage { get; set; }

        ////[DataMember(Order = 16, EmitDefaultValue = false)]
        //public string Bead { get; set; }
    }
}