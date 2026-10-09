using System.Collections.Generic;
using System.Runtime.Serialization;
using IGServices.Common.Types.E1;


namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ScheduleLines : List<ScheduleLine>
    {
    }

    [DataContract(Namespace = "")]
    public class ScheduleLine
    {

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal BatchNum { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string BatchName { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string BatchGroupName { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public string BatchDynamicName { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public WorkOrder WorkOrder { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public RelatedSalesLine RelatedSalesLine { get; set; }

        [DataMember(Order = 140, EmitDefaultValue = false)]
        public GlassPart GlassPart { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public SpecUnit SpecUnit { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public long WorkOrderId { get; set; }

        [DataMember(Order = 310, EmitDefaultValue = false)]
        public long SpecUnitId { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public string DeliveryInstruction1 { get; set; }
        [DataMember(Order = 410, EmitDefaultValue = false)]
        public string DeliveryInstruction2 { get; set; }

        public F5834A11 F5834A11 { get; set; }
        public F580232 F580232 { get; set; }
        public F580021 F580021 { get; set; }
        public F580024 F580024 { get; set; }
        public List<F580220> F580220s { get; set; }
        public List<F4715> F4715s { get; set; }
        public List<F584715> F584715s { get; set; }
        public ItemAttributes ItemAttributes { get; set; }
        public List<string> FlexItemDescriptions { get; set; }
    }
}
