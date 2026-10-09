// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.Enterprise.ScheduleEvent
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class ScheduleEvents : List<ScheduleEvent>
    {
    }

    [DataContract(Namespace = "")]
  public class ScheduleEvent
  {
    [DataMember(Order = 6)]
    public Plant Plant { get; set; }

    [DataMember(Order = 10)]
    public string PlantNum { get; set; }

    [DataMember(Order = 20)]
    public Decimal ScheduleNum { get; set; }

    [DataMember(Order = 21)]
    public int ScheduleId { get; set; }

    [DataMember(Order = 22)]
    public int BusinessScheduleId { get; set; }

    [DataMember(Order = 23)]
    public long ShopFloorScheduleId { get; set; }

    [DataMember(Order = 30)]
    public string BatchNum { get; set; }

    [DataMember(Order = 32)]
    public string CostCenter { get; set; }

    [DataMember(Order = 40)]
    public DateTime ScheduleDate { get; set; }

    [DataMember(Order = 42)]
    public Decimal ScheduleDateE1 { get; set; }

    [DataMember(Order = 44)]
    public DateTime TransactionDate { get; set; }

    [DataMember(Order = 60)]
    public string ShopFloorSystem { get; set; }

    [DataMember(Order = 62)]
    public string StatusCode { get; set; }

    [DataMember(Order = 64)]
    public string ProcessFlag { get; set; }

    [DataMember(Order = 66)]
    public Decimal MathNumeric06 { get; set; }

    [DataMember(Order = 68)]
    public string OperationStatus { get; set; }

    [DataMember(Order = 70)]
    public string Description { get; set; }

    [DataMember(Order = 100)]
    public string TransferBatchNum { get; set; }

    [DataMember(Order = 102)]
    public string TransferStatusCode { get; set; }

    [DataMember(Order = 104)]
    public string TransferMessage { get; set; }

    [DataMember(Order = 106)]
    public Decimal TransferTransactionId { get; set; }

    [DataMember(Order = 108)]
    public int TransferLogId { get; set; }
  }
}
