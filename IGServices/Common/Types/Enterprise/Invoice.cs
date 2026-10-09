using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class Invoices : List<Invoice>
    {
    }

    [DataContract(Namespace = "")]
    public class Invoice
    {
        [DataMember(Order = 10)]
        public decimal InvoiceNum { get; set; }
        [DataMember(Order = 12)]
        public string InvoiceType { get; set; }
        [DataMember(Order = 14)]
        public DateTime InvoiceDate { get; set; }

        [DataMember(Order = 16)]
        public string PlantNum { get; set; }
        [DataMember(Order = 18)]
        public Plant Plant { get; set; }
        [DataMember(Order = 20)]
        public Decimal SoldToCustomerNum { get; set; }
        [DataMember(Order = 22)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 24)]
        public Decimal ShipToCustomerNum { get; set; }
        [DataMember(Order = 26)]
        public Customer ShipToCustomer { get; set; }

        [DataMember(Order = 50)]
        public string Currency { get; set; }

        [DataMember(Order = 100)]
        public InvoiceLines InvoiceLines { get; set; }

    }
}
