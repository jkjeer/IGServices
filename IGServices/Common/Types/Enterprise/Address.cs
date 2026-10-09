using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class Addresses : List<Address>
    {
    }

    [DataContract(Namespace = "")]
    public class Address
    {
        [DataMember(Order = 10)]
        public decimal AddressNum { get; set; }

        [DataMember(Order = 12)]
        public string Name { get; set; }

        [DataMember(Order = 14)]
        public string Address1 { get; set; }

        [DataMember(Order = 16)]
        public string Address2 { get; set; }

        [DataMember(Order = 18)]
        public string Address3 { get; set; }

        [DataMember(Order = 20)]
        public string Address4 { get; set; }

        [DataMember(Order = 22)]
        public string Address5 { get; set; }

        [DataMember(Order = 24)]
        public string City { get; set; }

        [DataMember(Order = 26)]
        public string State { get; set; }

        [DataMember(Order = 28)]
        public string PostalCode { get; set; }

        [DataMember(Order = 30)]
        public string Country { get; set; }

        [DataMember(Order = 50)]
        public string AddressType { get; set; }

        [DataMember(Order = 51)]
        public decimal Company { get; set; }

        [DataMember(Order = 52)]
        public string NationalAccountGroup { get; set; }

        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }
        public decimal UpdateDateId { get; set; }
        public decimal UpdateTimeId { get; set; }

    }
}
