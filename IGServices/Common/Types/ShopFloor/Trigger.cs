using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class Triggers : List<Trigger>
    {
    }

    [DataContract(Namespace = "")]
    public class Trigger
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }
        [DataMember(Order = 2)]
        public string TriggerName { get; set; }
        [DataMember(Order = 10)]
        public bool InitiallyDisabled { get; set; }
        [DataMember(Order = 11)]
        public bool CurrentState { get; set; }
    }
}
