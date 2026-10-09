using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class GlassParts : List<GlassPart>
    {
    }

    [DataContract(Namespace = "")]
    public class GlassPart
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string Construction { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string StockingType { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string GLCategory { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string PrimaryUom { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string UnitType { get; set; }

        // Parts List Instance
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal SequenceNum { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public int LiteNum { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string LiteName { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal OrderQty { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public decimal TargetQty { get; set; }




    }
}