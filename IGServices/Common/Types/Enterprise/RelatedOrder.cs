using System;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class RelatedOrder
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public String Company { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public Decimal OrderNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public String OrderType { get; set; }
    }
}
