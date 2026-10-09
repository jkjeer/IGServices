using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class AddressBookItems : List<AddressBookItem>
    {
    }

    [DataContract(Namespace = "")]
    public class AddressBookItem
    {

        public AddressBookItem()
        {
            AddressNum = 0;
            AddressType = "";
            AddressTypeName = "";
            Name = "";
            Address1 = "";
            Address2 = "";
            Address3 = "";
            Address4 = "";
            PostalCode = "";
            City = "";
            State = "";
            AreaCode = "";
            Phone = "";
            ParentAddressNum = 0;
            Company = 0;
        }

        [DataMember(Order = 1)]
        public decimal AddressNum { get; set; }
        [DataMember(Order = 2)]
        public string AddressType { get; set; }
        [DataMember(Order = 3)]
        public string AddressTypeName { get; set; }
        [DataMember(Order = 4)]
        public string Name { get; set; }
        [DataMember(Order = 5)]
        public string Address1 { get; set; }
        [DataMember(Order = 6)]
        public string Address2 { get; set; }
        [DataMember(Order = 7)]
        public string Address3 { get; set; }
        [DataMember(Order = 8)]
        public string Address4 { get; set; }
        [DataMember(Order = 9)]
        public string PostalCode { get; set; }
        [DataMember(Order = 10)]
        public string City { get; set; }
        [DataMember(Order = 11)]
        public string State { get; set; }
        [DataMember(Order = 12)]
        public string AreaCode { get; set; }
        [DataMember(Order = 13)]
        public string Phone { get; set; }
        [DataMember(Order = 14)]
        public decimal ParentAddressNum { get; set; }
        [DataMember(Order = 15)]
        public decimal Company { get; set; }
        [DataMember(Order = 50)]
        public string NationalAccountGroup { get; set; }
        [DataMember(Order = 100)]
        public DateTime UpdateDate { get; set; }
    }
}
