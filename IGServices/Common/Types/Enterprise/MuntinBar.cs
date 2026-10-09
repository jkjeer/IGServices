using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class MuntinBars : List<MuntinBar>
    {
    }

    [DataContract(Namespace = "")]
    public class MuntinBar
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string MCode { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public int MCodeNum { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string Orientation { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string ExtColor { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string IntColor { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public bool IsTwoTone { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal Width { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public decimal Thickness { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }
    }
}
