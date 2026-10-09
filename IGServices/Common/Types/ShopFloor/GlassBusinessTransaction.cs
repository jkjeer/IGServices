// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.ShopFloor.GlassBusinessTransaction
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class GlassBusinessTransactions : List<GlassBusinessTransaction>
    {
    }

    [DataContract(Namespace = "")]
    public class GlassBusinessTransaction
    {
        public GlassBusinessTransaction()
        {
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = 0L;
            TransferDate = DateTime.Now.ToLocalTime();
            IsValid = true;
            ValidationStatus = "Valid";
        }

        [DataMember(Order = 10)] public int GlassBusinessTransactionId { get; set; }

        [DataMember(Order = 12)] public string TransactionType { get; set; }

        [DataMember(Order = 14)] public string TransactionSourceCode { get; set; }

        [DataMember(Order = 15)] public int TransactionId { get; set; }

        [DataMember(Order = 16)] public DateTime? TransactionDate { get; set; }

        [DataMember(Order = 17)] public string TransactionDateChar { get; set; }

        [DataMember(Order = 18)] public DateTime? ProductionDate { get; set; }

        [DataMember(Order = 20)] public string PlantID { get; set; }

        [DataMember(Order = 22)] public decimal WOrderNum { get; set; }

        [DataMember(Order = 23)] public string LotNum { get; set; }

        [DataMember(Order = 24)] public string SerialNum { get; set; }

        [DataMember(Order = 26)] public decimal ItemNum { get; set; }

        [DataMember(Order = 28)] public decimal ScheduleNum { get; set; }

        [DataMember(Order = 30)] public int LiteNum { get; set; }

        [DataMember(Order = 40)] public string LineTypeCode { get; set; }

        [DataMember(Order = 41)] public string ProductionLineCode { get; set; }

        [DataMember(Order = 42)] public string E1WorkcenterCode { get; set; }

        [DataMember(Order = 44)] public string LineOperationCode { get; set; }

        [DataMember(Order = 46)] public string E1OperationCode { get; set; }

        [DataMember(Order = 64)] public decimal Quantity { get; set; }

        [DataMember(Order = 80)] public string RejectReason { get; set; }

        [DataMember(Order = 81)] public string RejectCode { get; set; }

        [DataMember(Order = 82)] public string Comments { get; set; }

        [DataMember(Order = 100)] public bool IsValid { get; set; }

        [DataMember(Order = 102)] public string ValidationStatus { get; set; }

        [DataMember(Order = 110)] public bool Transferred { get; set; }

        [DataMember(Order = 112)] public string TransferStatus { get; set; }

        [DataMember(Order = 114)] public DateTime TransferDate { get; set; }

        [DataMember(Order = 116)] public string TransferMessage { get; set; }

        [DataMember(Order = 118)] public long TransferBatchNum { get; set; }

        [DataMember(Order = 120)] public decimal TransferId { get; set; }
    }
}