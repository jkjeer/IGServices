using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackItemEvents : List<RackItemEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class RackItemEvent
    {
        [DataMember(Order = 20)]
        public decimal EventId { get; set; }
        [DataMember(Order = 10)]
        public decimal RackId { get; set; }
        [DataMember(Order = 12)]
        public string SerialNum { get; set; }
        [DataMember(Order = 20)]
        public string EventCode { get; set; }
        [DataMember(Order = 24)]
        public DateTime EventDate { get; set; }

        [DataMember(Order = 30)]
        public string LotNum { get; set; }
        [DataMember(Order = 32)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 40)]
        public string Status { get; set; }
        [DataMember(Order = 42)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 50)]
        public decimal SOrderNum { get; set; }
        [DataMember(Order = 52)]
        public string SOrderType { get; set; }
        [DataMember(Order = 54)]
        public decimal SOrderLineNum { get; set; }

        [DataMember(Order = 60)]
        public decimal WOrderNum { get; set; }

        [DataMember(Order = 60)]
        public decimal POrderNum { get; set; }
        [DataMember(Order = 62)]
        public string POrderType { get; set; }
        [DataMember(Order = 64)]
        public decimal POrderLineNum { get; set; }

        [DataMember(Order = 70)]
        public decimal LoadNum { get; set; }

        [DataMember(Order = 100)]
        public RackItem RackItem { get; set; }
    }
}
