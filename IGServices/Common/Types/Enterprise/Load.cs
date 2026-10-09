using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Loads : List<Load>
    {
    }

    [DataContract(Namespace = "")]
    public class Load
    {

        [DataMember(Order = 10)]
        public Decimal LoadNum { get; set; }
        [DataMember(Order = 15)]
        public string PlantNum { get; set; }
        [DataMember(Order = 20)]
        public DateTime ActualShipDate { get; set; }
        [DataMember(Order = 30)]
        public DateTime ArrivalDate { get; set; }
        [DataMember(Order = 40)]
        public decimal ArrivalHour { get; set; }
        [DataMember(Order = 50)]
        public string FreightHandlingCode { get; set; }
        [DataMember(Order = 60)]
        public string TrailerNum { get; set; }
        [DataMember(Order = 70)]
        public string BorderCrossing { get; set; }
        [DataMember(Order = 80)]
        public string BrokerName { get; set; }
        [DataMember(Order = 85)]
        public string ParsNum { get; set; }
        [DataMember(Order = 90)]
        public decimal BillOfLadingNum { get; set; }

        [DataMember(Order = 100)]
        public Customer Customer { get; set; }
        [DataMember(Order = 110)]
        public Customer ShipToCustomer { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public Racks Racks { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public Invoices Invoices { get; set; }

    }
}
