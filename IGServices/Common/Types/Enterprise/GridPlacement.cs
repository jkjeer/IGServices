using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class GridPlacements : List<GridPlacement>
    {
    }

    [DataContract(Namespace = "")]
    public class GridPlacement
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int OrdinalNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal Position { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal Angle { get; set; }
    }
}