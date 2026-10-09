using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class CrateTags : List<CrateTag>
    {
    }

    [DataContract(Namespace = "")]
    public class CrateTag
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int TagInfoID { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string PlantID { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public decimal OrderNum { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string LineID { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Shift { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string WorkStation { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal QuantityPacked { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public DateTime ScheduleDate { get; set; }

        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string PackedBy { get; set; }

        [DataMember(Order = 120, EmitDefaultValue = false)]
        public bool PrintTo { get; set; }

        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string PrinterName { get; set; }

        [DataMember(Order = 140, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string SeriaLNumber { get; set; }

        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string RackTypeCode { get; set; }

        [DataMember(Order = 170, EmitDefaultValue = false)]
        public bool Transferred { get; set; }

        [DataMember(Order = 180, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 190, EmitDefaultValue = false)]
        public string IPAddress { get; set; }

    }
}
