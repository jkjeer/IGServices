using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class Airspaces : List<Airspace>
    {
    }
    
    [DataContract(Namespace = "")]
    public class Airspace
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int Position { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false)]
        public Spacer Spacer { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal ChannelSize { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string ChannelType { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string ChannelTypeDesc { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string ButylColor { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string SiliconeColor { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string GasMixture { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string GasMixtureDesc { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string Tubes { get; set; }

        [DataMember(Order = 41, EmitDefaultValue = false)]
        public string Shades { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string ShadesDesc { get; set; }

        [DataMember(Order = 51, EmitDefaultValue = false)]
        public string Tape { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public BarAssembly BarAssembly { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public GridPattern GridPattern { get; set; }
    }
}