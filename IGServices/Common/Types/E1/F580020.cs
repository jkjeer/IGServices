using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580020s : List<F580020>
    {
    }

    [DataContract(Namespace = "")]
    public class F580020
    {
        [DataMember(Order = 10)]
        public decimal S6Z58SCN { get; set; }

        [DataMember(Order = 20)]
        public string S6EDBT { get; set; }

        [DataMember(Order = 30)]
        public string S6MCU { get; set; }

        [DataMember(Order = 40)]
        public decimal S6Z58SCD { get; set; }

        [DataMember(Order = 50)]
        public string S6Z58SFS { get; set; }

        [DataMember(Order = 52, EmitDefaultValue = false)]
        public string S6Z58SFS_NAME { get; set; }

        [DataMember(Order = 60)]
        public string S6TSC { get; set; }

        [DataMember(Order = 70)]
        public string S6EDSP { get; set; }

        [DataMember(Order = 80)]
        public decimal S6MATH06 { get; set; }

        [DataMember(Order = 90)]
        public string S6WOPST { get; set; }

        [DataMember(Order = 100)]
        public string S6DS50 { get; set; }
    }
}
