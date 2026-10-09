using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class SalesLineAttributes : List<SalesLineAttribute>
    {
    }

    [DataContract(Namespace = "")]
    public class SalesLineAttribute
    {
        [DataMember(Order = 10)]
        public decimal WorkOrderNum { get; set; }

        [DataMember(Order = 20)]
        public string OrigCustomerPO { get; set; }
    }
}