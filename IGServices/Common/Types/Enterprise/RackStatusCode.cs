using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RackStatusCodes : List<RackStatusCode>
    {
    }

    [DataContract(Namespace = "")]
    public class RackStatusCode : UdcItem
    {
    }
}
