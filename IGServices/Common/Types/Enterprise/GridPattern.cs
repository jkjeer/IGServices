using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class GridPattern
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string GridStyle { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string GridStyleDesc { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string SubGridStyle { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string SubGridStyleDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal VerticalPrairieOffset { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public decimal HorizontalPrairieOffset { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string OverlayBar { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string OverlayBarDesc { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string PlacementType { get; set; }

        [DataMember(Order = 51, EmitDefaultValue = false)]
        public string PlacementTypeDesc { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal NumVertical { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal NumHorizontal { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal NumOpeningsHigh { get; set; }

        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal NumOpeningsWide { get; set; }

        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal NumOpeningsOdd { get; set; }

        [DataMember(Order = 230, EmitDefaultValue = false)]
        public decimal NumOpeningsRectangle { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string Drawing { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = true)]
        public int ReferenceCorner { get; set; }

        [DataMember(Order = 500, EmitDefaultValue = false)]
        public GridPlacements VerticalBarPlacements { get; set; }

        [DataMember(Order = 510, EmitDefaultValue = false)]
        public GridPlacements HorizontalBarPlacements { get; set; }
    }
}