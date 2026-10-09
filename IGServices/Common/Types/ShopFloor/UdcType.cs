using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class UdcTypes : List<UdcType>
    {
    }

    [DataContract(Namespace = "")]
    public class UdcType
    {

        public UdcType()
        {
            ProductCode = "";
            UserCode = "";
            Description = "";
            CodeLength = 0;
            NumericYN = "";
        }

        [DataMember(Order = 2)]
        public string ProductCode { get; set; }
        [DataMember(Order = 3)]
        public string UserCode { get; set; }
        [DataMember(Order = 4)]
        public string Description { get; set; }
        [DataMember(Order = 5)]
        public decimal CodeLength { get; set; }
        [DataMember(Order = 6)]
        public string NumericYN { get; set; }
        [DataMember(Order = 7)]
        public DateTime UpdateDate { get; set; }
    }
}
