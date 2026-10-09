using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RelatedOrderLines : List<RelatedOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class RelatedOrderLine
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public String Company { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public Decimal OrderNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public String OrderType { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public Decimal OrderLineNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public Decimal LiteNum { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public String TransactionType { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public WorkOrder WorkOrder { get; set; }
    }
}
