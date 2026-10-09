using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class PurchasedLots : List<PurchasedLot>
    {
    }

    [DataContract(Namespace = "")]
    public class PurchasedLot
    {
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public decimal UniqueId { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal LotOrderNum { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string LotOrderType { get; set; }
        [DataMember(Order = 13, EmitDefaultValue = false)]
        public string LotOrderCompanyNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal LotOrderLineNum { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string LotNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string LastStatus { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string NextStatus { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public DateTime OrderDate { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public DateTime CancelDate { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal OrderQty { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public decimal ReceivedQty { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public decimal OpenQty { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string SupplierPlantNum { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public decimal SupplierOrderNum { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public string SupplierOrderType { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public decimal SupplierOrderLineNum { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public DateTime LotDate { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public DateTime LotReceivedDate { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public decimal LoadNum { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public DateTime LoadDate { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string ProcessId { get; set; }
        [DataMember(Order = 82, EmitDefaultValue = false)]
        public DateTime EventDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal RackId { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public string SerialNum { get; set; }
        [DataMember(Order = 104, EmitDefaultValue = false)]
        public decimal RackQty { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public GlassItem GlassItem { get; set; }
        public ItemAttributes ItemAttributes { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public RelatedOrderLines RelatedOrderLines { get; set; }

    }
}
