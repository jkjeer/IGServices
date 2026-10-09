using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.WOStatusSvc
{
    [CollectionDataContract(Namespace = "")]
    public class PendingUpdatesResults : List<PendingUpdatesResult>
    {
    }
}
