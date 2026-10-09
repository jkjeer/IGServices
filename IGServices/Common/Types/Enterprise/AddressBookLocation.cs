using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class AddressBookLocations : List<AddressBookLocation>
    {
    }
    
    [DataContract(Namespace = "")]
    public class AddressBookLocation
    {
        [DataMember(Order = 10)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 11)]
        public decimal ParentAddressNum { get; set; }
        [DataMember(Order = 12)]
        public string Name { get; set; }
        [DataMember(Order = 15)]
        public decimal Company { get; set; }

        [DataMember(Order = 20)]
        public string AddressType { get; set; }
        [DataMember(Order = 21)]
        public string AddressTypeName { get; set; }
        [DataMember(Order = 22)]
        public string Address1 { get; set; }
        [DataMember(Order = 23)]
        public string Address2 { get; set; }
        [DataMember(Order = 24)]
        public string Address3 { get; set; }
        [DataMember(Order = 25)]
        public string Address4 { get; set; }
        [DataMember(Order = 26)]
        public string PostalCode { get; set; }
        [DataMember(Order = 27)]
        public string City { get; set; }
        [DataMember(Order = 28)]
        public string State { get; set; }
        [DataMember(Order = 29)]
        public string Country { get; set; }

        [DataMember(Order = 40)]
        public string AreaCode { get; set; }
        [DataMember(Order = 41)]
        public string Phone { get; set; }

        [DataMember(Order = 50)]
        public string NationalAccountGroup { get; set; }
        [DataMember(Order = 51)]
        public string NationalAccountGroupName { get; set; }

        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }
        public decimal UpdateDateId { get; set; }
        public decimal UpdateTimeId { get; set; }
    }
}
