using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class Spacer
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string SpacerType { get; set; }

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

        [DataMember(Order = 38, EmitDefaultValue = false)]
        public bool Endur { get; set; }
    }
}
