using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class InvoiceLines : List<InvoiceLine>
    {
    }

    [DataContract(Namespace = "")]
    public class InvoiceLine
    {
        [DataMember(Order = 1)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(Order = 1)]
        public string SalesOrderType { get; set; }
        [DataMember(Order = 1)]
        public decimal SalesOrderLineNum { get; set; }
        [DataMember(Order = 1)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 1)]
        public string PlantNum { get; set; }
        [DataMember(Order = 1)]
        public string StockingType { get; set; }
        [DataMember(Order = 1)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 1)]
        public decimal Quantity { get; set; }
        [DataMember(Order = 1)]
        public string LineType { get; set; }
        [DataMember(Order = 1)]
        public string PayTerms { get; set; }
        [DataMember(Order = 1)]
        public string Description1 { get; set; }
        [DataMember(Order = 1)]
        public string Description2 { get; set; }
        [DataMember(Order = 1)]
        public string CustomerPo { get; set; }
        [DataMember(Order = 1)]
        public string CustomerPart1 { get; set; }
        [DataMember(Order = 1)]
        public string CustomerPart2 { get; set; }
        [DataMember(Order = 1)]
        public string CustomerPart3 { get; set; }
        [DataMember(Order = 1)]
        public string ProductType { get; set; }
        [DataMember(Order = 1)]
        public decimal BillableSqFt { get; set; }
        [DataMember(Order = 1)]
        public decimal ExtendedPrice { get; set; }
        [DataMember(Order = 1)]
        public decimal UnitListPrice { get; set; }
        [DataMember(Order = 1)]
        public decimal UnitPrice { get; set; }
        [DataMember(Order = 1)]
        public decimal ItemWeight { get; set; }
        [DataMember(Order = 1)]
        public string HarmonizingCode { get; set; }
        [DataMember(Order = 1)]
        public string UnitOfMeasure { get; set; }

        [DataMember(Order = 100)]
        public Item Item { get; set; }
    }
}
