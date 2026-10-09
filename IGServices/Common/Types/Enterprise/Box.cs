using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    
    [CollectionDataContract(Namespace = "")]
    public class Boxes : List<Box>
    {
    }

    public class Box
    {

        public Box()
        {
        }

        [DataMember(Order = 10)]
        public String PlantNum { get; set; }
        [DataMember(Order = 12)]
        public String PlantCode { get; set; }

        [DataMember(Order = 20)]
        public Decimal ScheduleNum { get; set; }

        [DataMember(Order = 22)]
        public Int32 BoxHoldHeaderID { get; set; }

        [DataMember(Order = 40)]
        public Decimal RackId { get; set; }
        [DataMember(Order = 44)]
        public String RackType { get; set; }
        [DataMember(Order = 46)]
        public String RackTypeDesc { get; set; }
        [DataMember(Order = 50)]
        public bool BoxCrate { get; set; }
        [DataMember(Order = 52)]
        public Int16 BoxNum { get; set; }
        [DataMember(Order = 54)]
        public bool BoxRotated { get; set; }

        [DataMember(Order = 100)]
        public BoxItems BoxItems { get; set; }

    }

}
