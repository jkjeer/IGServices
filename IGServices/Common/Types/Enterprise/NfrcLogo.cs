using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class NfrcLogo
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Code1 { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string Code2 { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal HorizontalOffset { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public decimal VerticalOffset { get; set; }

    }
}
