using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Crates : List<Crate>
    {
    }

    [DataContract(Namespace = "")]
    public class Crate
    {
        //public Crate()
        //{
        //    CrateId = 0;
        //    CrateType = "";
        //    OrderQuantity = 0;
        //    Description = "";
        //    Quantity = 0;
        //    Message = "";
        //}

        [DataMember(Order = 10)]
        public decimal CrateId { get; set; }

        [DataMember(Order = 20)]
        public string CrateType { get; set; }

        [DataMember(Order = 30)]
        public decimal OrderNum { get; set; }

        [DataMember(Order = 31)]
        public string OrderType { get; set; }

        [DataMember(Order = 32)]
        public string OrderCompany { get; set; }

        [DataMember(Order = 33)]
        public decimal OrderLineNum { get; set; }

        [DataMember(Order = 34)]
        public decimal OrderQuantity { get; set; }

        [DataMember(Order = 40)]
        public string Description { get; set; }

        [DataMember(Order = 50)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 60)]
        public string Message { get; set; }
    }
}
