// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.Enterprise.SpecUnitDocument
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SpecUnitDocuments : List<SpecUnitDocument>
    {
    }

    [DataContract(Namespace = "")]
  public class SpecUnitDocument
  {
    [DataMember(Order = 10)]
    public Decimal ConfigurationId { get; set; }

    [DataMember(Order = 12)]
    public Decimal ItemNum { get; set; }

    [DataMember(Order = 14)]
    public string StockType { get; set; }

    [DataMember(Order = 16)]
    public string UnitType { get; set; }

    [DataMember(Order = 20)]
    public Decimal DocumentNum { get; set; }

    [DataMember(Order = 30)]
    public string DocumentType { get; set; }

    [DataMember(Order = 40)]
    public Decimal DocumentLineNum { get; set; }

    [DataMember(Order = 50)]
    public DateTime TransactionDate { get; set; }
  }
}
