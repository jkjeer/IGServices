using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class SpacerType
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string Material { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string Surface { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string Profile { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string Color { get; set; }

        [DataMember(Order = 38, EmitDefaultValue = false)]
        public bool Painted { get; set; }
    }
}
