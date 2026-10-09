using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class UdcItems : List<UdcItem>
    {
    }

    [DataContract(Namespace = "")]
    public class UdcItem
    {

        public UdcItem()
        {
            ProductCode = "";
            UserCode = "";
            UserValue = "";
            Description1 = "";
            Description2 = "";
            HandlingCode = "";
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string ProductCode { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string UserCode { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string UserValue { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string Description1 { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string Description2 { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string HandlingCode { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }
    }
}
