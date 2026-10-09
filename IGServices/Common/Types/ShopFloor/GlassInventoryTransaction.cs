using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class GlassInventoryTransactions : List<GlassInventoryTransaction>
    {
    }

    [DataContract(Namespace = "")]
    public class GlassInventoryTransaction
    {
        public GlassInventoryTransaction()
        {
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = 0;
            TransferDate = DateTime.Now.ToLocalTime();
            IsValid = true;
            ValidationStatus = "Valid";
        }

        [DataMember(Order = 10)]
        public int TransactionId { get; set; }
        [DataMember(Order = 12)]
        public string PlantNum { get; set; }
        [DataMember(Order = 14)]
        public decimal WOrderNum { get; set; }
        [DataMember(Order = 16)]
        public int ScheduleNum { get; set; }
        [DataMember(Order = 18)]
        public int UnitId { get; set; }
        [DataMember(Order = 20)]
        public int LiteNum { get; set; }
        [DataMember(Order = 22)]
        public string LiteName { get; set; }

        [DataMember(Order = 30)]
        public DateTime TransactionDate { get; set; }
        [DataMember(Order = 32)]
        public string TransactionType { get; set; }
        [DataMember(Order = 34)]
        public int SourceTransactionId { get; set; }

        [DataMember(Order = 40)]
        public string RoutingCode { get; set; }
        [DataMember(Order = 42)]
        public string ProductionLineCode { get; set; }
        [DataMember(Order = 44)]
        public string LineOperationCode { get; set; }
        [DataMember(Order = 46)]
        public int Quantity { get; set; }

        [DataMember(Order = 50)]
        public string RejectReason { get; set; }

        [DataMember(Order = 60)]
        public decimal RackId { get; set; }
        [DataMember(Order = 62)]
        public string SerialNum { get; set; }
        [DataMember(Order = 64)]
        public decimal ItemNum { get; set; }
        [DataMember(Order = 66)]
        public string E1WorkCenterId { get; set; }

        [DataMember(Order = 70)]
        public string Comments { get; set; }

        [DataMember(Order = 100)]
        public bool IsValid { get; set; }
        [DataMember(Order = 102)]
        public string ValidationStatus { get; set; }

        [DataMember(Order = 110)]
        public bool Transferred { get; set; }
        [DataMember(Order = 112)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 114)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 116)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 118)]
        public long TransferBatchNum { get; set; }
        [DataMember(Order = 120)]
        public decimal TransferId { get; set; }


    }
}
