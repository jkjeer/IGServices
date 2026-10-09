using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SalesOrders : List<SalesOrder>
    {
    }

    [DataContract(Namespace = "")]
    public class SalesOrder 
    {
    }

    [CollectionDataContract(Namespace = "")]
    public class SalesOrderLines : List<SalesOrderLine>
    {
    }

    [DataContract(Namespace = "")]
    public class SalesOrderLine
    {
    }
}
