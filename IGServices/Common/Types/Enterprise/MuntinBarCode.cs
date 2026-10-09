using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class MuntinBarCodes : List<MuntinBarCode>
    {
    }

    [DataContract(Namespace = "")]
    public class MuntinBarCode
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string MCode { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public int MCodeNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal Thickness { get; set; }

        [DataMember(Order = 27, EmitDefaultValue = false)]
        public decimal Width { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ColorDesc { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string ExtColorDesc { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string IntColorDesc { get; set; }

        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string ExtColor { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string IntColor { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = true)]
        public bool IsTwoTone { get; set; }



        [DataMember(Order = 100, EmitDefaultValue = true)]
        public DateTime UpdateDate { get; set; }

    }
}
