// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.ShopFloor.SpecUnitDocumentTable
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IGServices.Common.Types.Enterprise;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace IGServices.Common.Types.ShopFloor
{
  [DesignerCategory("Code")]
  public class SpecUnitDocumentTable : DataTable
  {
    public DateTime MaxDate
    {
      get
      {
        return this.AsEnumerable().Select<DataRow, DateTime>((Func<DataRow, DateTime>) (a => a.Field<DateTime>("TransactionDate"))).Max<DateTime>();
      }
      set
      {
      }
    }

    public Decimal MaxId
    {
      get
      {
        return this.AsEnumerable().Select<DataRow, Decimal>((Func<DataRow, Decimal>) (a => a.Field<Decimal>("ConfigurationId"))).Max();
      }
      set
      {
      }
    }

    public SpecUnitDocumentTable()
    {
      this.ExtendedProperties.Add((object) "Target", (object) "[EBS].[SpecUnitDocumentLedger]");
      this.Columns.Add("ConfigurationId", typeof (Decimal));
      this.Columns.Add("DocumentNum", typeof (Decimal));
      this.Columns.Add("DocumentType", typeof (string));
      this.Columns.Add("DocumentLineNum", typeof (Decimal));
      this.Columns.Add("TransactionDate", typeof (DateTime));
    }

    public void Add(SpecUnitDocuments specUnitDocuments)
    {
      try
      {
        foreach (SpecUnitDocument specUnitDocument in (List<SpecUnitDocument>) specUnitDocuments)
        {
          DataRow row = this.NewRow();
          row["ConfigurationId"] = (object) specUnitDocument.ConfigurationId;
          row["DocumentNum"] = (object) specUnitDocument.DocumentNum;
          row["DocumentType"] = (object) specUnitDocument.DocumentType;
          row["DocumentLineNum"] = (object) specUnitDocument.DocumentLineNum;
          row["TransactionDate"] = (object) specUnitDocument.TransactionDate;
          this.Rows.Add(row);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
