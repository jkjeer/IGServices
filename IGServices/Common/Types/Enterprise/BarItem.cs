using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class BarItems : List<BarItem>
    {
    }

    [DataContract(Namespace = "", Name = "BarItem")]
    public class BarItem : Item
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string MCode { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string Color { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public decimal Width { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public decimal Thickness { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public decimal Length { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public String Orientation { get; set; }
    }
}
