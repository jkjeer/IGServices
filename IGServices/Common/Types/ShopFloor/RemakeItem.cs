using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class RemakeItems : List<RemakeItem>
    {
    }

    public class RemakeItem
    {
        [DataMember(Order = 10)]
        public int E1RemakeId { get; set; }
        [DataMember(Order = 20)]
        public string PlantId { get; set; }
        [DataMember(Order = 30)]
        public decimal WOrderNum { get; set; }
        [DataMember(Order = 40)]
        public string WOrderType { get; set; }
        [DataMember(Order = 42)]
        public decimal LiteNumber { get; set; }
        [DataMember(Order = 43)]
        public decimal OriginalScheduleNum { get; set; }
        [DataMember(Order = 44)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 50)]
        public decimal ScheduleSeq { get; set; }
        [DataMember(Order = 52)]
        public string UnitID { get; set; }
        [DataMember(Order = 54)]
        public DateTime TransactionDate { get; set; }
        [DataMember(Order = 56)]
        public string TransactionDateString { get; set; }
        [DataMember(Order = 58)]
        public DateTime ProductionDate { get; set; }
        [DataMember(Order = 70)]
        public decimal Quantity { get; set; }

        [DataMember(Order = 80)]
        public string CustName { get; set; }
        [DataMember(Order = 82)]
        public string WorkCenter { get; set; }
        [DataMember(Order = 84)]
        public string WorkStation { get; set; }
        [DataMember(Order = 86)]
        public string LineId { get; set; }
        [DataMember(Order = 88)]
        public string Shift { get; set; }
        [DataMember(Order = 90)]
        public string Crew { get; set; }

        [DataMember(Order = 100)]
        public string GlassType { get; set; }
        [DataMember(Order = 102)]
        public string Dimension { get; set; }
        [DataMember(Order = 104)]
        public int ShapeNum { get; set; }
        [DataMember(Order = 110)]
        public string Supplier { get; set; }

        [DataMember(Order = 120)]
        public string WipRack { get; set; }
        [DataMember(Order = 122)]
        public string WipSlot { get; set; }

        [DataMember(Order = 200)]
        public bool Transferred { get; set; }
        [DataMember(Order = 201)]
        public DateTime? TransferredDate { get; set; }
        [DataMember(Order = 202)]
        public string TransferDateTime { get; set; }
        [DataMember(Order = 204)]
        public decimal TransferId { get; set; }
        [DataMember(Order = 206)]
        public decimal TransferBatchId { get; set; }
        [DataMember(Order = 208)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 210)]
        public string TransferMessage { get; set; }

    }

}
