using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class TagInfoItems : List<TagInfoItem>
    {
    }

    public class TagInfoItem
    {
        [DataMember(Order = 1)]
        public int TagInfoID { get; set; }
        [DataMember(Order = 2)]
        public string PlantID { get; set; }
        [DataMember(Order = 3)]
        public decimal OrderNum { get; set; }
        [DataMember(Order = 4)]
        public string OrderType { get; set; }
        [DataMember(Order = 5)]
        public string LineID { get; set; }
        [DataMember(Order = 6)]
        public string Shift { get; set; }
        [DataMember(Order = 7)]
        public DateTime? WrittenDate { get; set; }
        [DataMember(Order = 8)]
        public string WorkStation { get; set; }
        [DataMember(Order = 9)]
        public decimal QuantityPacked { get; set; }
        [DataMember(Order = 10)]
        public DateTime? ScheduleDate { get; set; }
        [DataMember(Order = 11)]
        public string PackedBy { get; set; }
        [DataMember(Order = 12)]
        public bool PrintTo { get; set; }
        [DataMember(Order = 13)]
        public string PrinterName { get; set; }
        [DataMember(Order = 14)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 15)]
        public string SeriaLNumber { get; set; }
        [DataMember(Order = 16)]
        public string RackTypeCode { get; set; }
        [DataMember(Order = 17)]
        public bool Transferred { get; set; }
        [DataMember(Order = 18)]
        public DateTime? TransferDate { get; set; }
        [DataMember(Order = 19)]
        public string IPAddress { get; set; }

        [DataMember(Order = 200)]
        public decimal TransferId { get; set; }

        [DataMember(Order = 202)]
        public decimal TransferBatchId { get; set; }

        [DataMember(Order = 204)]
        public string TransferStatus { get; set; }

        [DataMember(Order = 206)]
        public string TransferMessage { get; set; }

        [DataMember(Order = 208)]
        public string TransferDateTime { get; set; }
    }

}
