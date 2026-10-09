// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.Enterprise.SpecUnitItem
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class SpecUnitItems : List<SpecUnitItem>
    {
    }

    [DataContract(Namespace = "")]
  public class SpecUnitItem
  {
    [DataMember(Order = 10)]
    public Decimal ItemNum { get; set; }

    [DataMember(Order = 14)]
    public string StockType { get; set; }

    [DataMember(Order = 16)]
    public string UnitType { get; set; }

    [DataMember(Order = 20)]
    public Decimal ConfigId { get; set; }

    [DataMember(Order = 30)]
    public DateTime TransactionDate { get; set; }

    [DataMember(Order = 100)]
    public SpecUnit SpecUnit { get; set; }

    [DataMember(Order = 200)]
    public ItemAttributes ItemAttributes { get; set; }
  }
}
