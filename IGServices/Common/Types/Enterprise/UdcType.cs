using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class UdcTypes : List<UdcType>
    {
    }

    [DataContract(Namespace = "")]
    public class UdcType
    {
        [DataMember(Order = 1)]
        public string ProductCode { get; set; }
        [DataMember(Order = 2)]
        public string UserCode { get; set; }
        [DataMember(Order = 3)]
        public string Description { get; set; }
        [DataMember(Order = 4)]
        public decimal CodeLength { get; set; }
        [DataMember(Order = 5)]
        public string Numeric { get; set; }
        [DataMember(Order = 10)]
        public DateTime UpdateDateTime { get; set; }
    }
}



