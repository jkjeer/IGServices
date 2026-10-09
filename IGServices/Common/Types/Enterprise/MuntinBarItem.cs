using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class MuntinBarItems : List<MuntinBarItem>
    {
    }

    [DataContract(Namespace = "", Name = "MuntinBarItem")]
    public class MuntinBarItem : Item
    {

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public decimal Length { get; set; }

        [DataMember(Order = 56, EmitDefaultValue = false)]
        public string PrimaryLocation { get; set; }

        [DataMember(Order = 58, EmitDefaultValue = false)]
        public decimal CostPerLinealFoot { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal QtyOnHand { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public Enterprise.MuntinBarCodes MuntinBarCodes { get; set; }


    }
}
