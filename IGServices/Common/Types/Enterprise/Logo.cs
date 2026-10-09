using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [DataContract(Namespace = "")]
    public class Logos
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string AnsiLogo { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string XOffset { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string YOffset { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public NfrcLogo NfrcLogo { get; set; }
    }

    [DataContract(Namespace = "")]
    public class Logo
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string AnsiLogo { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal XOffset { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal YOffset { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string NfrcLogo1 { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string NfrcLogo2 { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public decimal NfrcHorizontalOffset { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal NfrcVerticalOffset { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public string IgLogo { get; set; }
    }
}
