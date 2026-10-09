using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class BarAssembly
    {

        //[DataMember(Order = 10, EmitDefaultValue = false)]
        //public BarItems BarItems { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public MuntinBars MuntinBars { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CCode { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string CCodeDesc { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string BumperType { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string BumperTypeDesc { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal BumperQty { get; set; }

        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string BumperConfig { get; set; }
    }
}