using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IGServices.Common.Types;

namespace IGServices.Common.Types.Response
{

    [DataContract(Namespace = "")]
    public class ResultSet<T>
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public CardinalCollection<T> Result { get; set; }
    }
}
