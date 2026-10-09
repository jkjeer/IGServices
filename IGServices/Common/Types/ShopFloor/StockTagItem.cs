// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.ShopFloor.StockTagItem
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class StockTagItems : List<StockTagItem>
    {
    }

    [DataContract(Namespace = "")]
    public class StockTagItem
    {
        public StockTagItem()
        {
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = 0L;
            TransferDate = DateTime.Now.ToLocalTime();
            IsValid = true;
            ValidationStatus = "Valid";
        }

        [DataMember(Order = 14)] public string TransactionType { get; set; }

        [DataMember(Order = 20)] public string PlantNum { get; set; }

        [DataMember(Order = 30)] public decimal ScheduleNum { get; set; }

        [DataMember(Order = 32)] public int BoxNum { get; set; }

        [DataMember(Order = 40)] public decimal WorkOrderNum { get; set; }

        [DataMember(Order = 42)] public string LotNum { get; set; }

        [DataMember(Order = 44)] public string SerialNum { get; set; }

        [DataMember(Order = 46)] public decimal RackId { get; set; }

        [DataMember(Order = 48)] public string RackTypeCode { get; set; }

        [DataMember(Order = 50)] public decimal ItemNum { get; set; }

        [DataMember(Order = 60)] public decimal OrderQty { get; set; }

        [DataMember(Order = 62)] public decimal LitesPerRack { get; set; }

        [DataMember(Order = 64)] public decimal PackQty { get; set; }

        [DataMember(Order = 80)] public string PrintAddress { get; set; }

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