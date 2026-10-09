using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.WOStatusSvc
{
    [CollectionDataContract(Namespace = "")]
    public class PendingUpdatesResults : List<PendingUpdatesResult>
    {
    }

    [DataContract(Namespace = "")]
    public class PendingUpdatesResult
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public int Updates { get; set; }
    }
}
