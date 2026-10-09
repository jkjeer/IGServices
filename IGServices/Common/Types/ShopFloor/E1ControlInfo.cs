using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class E1ControlInfos : List<E1ControlInfo>
    {
    }

    [DataContract(Namespace = "")]
    public class E1ControlInfo
    {
        public E1ControlInfo()
        {
            AppCode = "";
            SysCode = "";
            UserCodeType = "";
            KeyValue = "";
            DescriptionOne = "";
            DescriptionTwo = "";
            ShortDescription = "";
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string AppCode { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string SysCode { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string UserCodeType { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string KeyValue { get; set; }
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string DescriptionOne { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string DescriptionTwo { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string ShortDescription { get; set; }
        [DataMember(Order = 8, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string WrittenBy { get; set; }
    }
}
