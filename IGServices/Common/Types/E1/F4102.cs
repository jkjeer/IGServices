using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable once InconsistentNaming

namespace IGServices.Common.Types.E1
{
    public class F4102s : List<F4102>
    {
    }

    public class F4102
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal IBANPL { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string ABALPH { get; set; }
    }
}
