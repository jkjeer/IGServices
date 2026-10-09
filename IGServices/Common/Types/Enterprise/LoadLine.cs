using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class LoadLines : List<LoadLine>
    {
    }

    [DataContract(Namespace = "")]
    public class LoadLine
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Decimal SequenceNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public Decimal RackId { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public Decimal RacksShip { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public Decimal RacksLoad { get; set; }


    }
}
