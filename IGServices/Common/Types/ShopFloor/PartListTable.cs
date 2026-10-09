// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.ShopFloor.PartListTable
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace IGServices.Common.Types.ShopFloor
{
  [DesignerCategory("Code")]
  public class PartListTable : DataTable
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
        return this.AsEnumerable().Select<DataRow, Decimal>((Func<DataRow, Decimal>) (a => a.Field<Decimal>("TransactionId"))).Max();
      }
      set
      {
      }
    }

    public PartListTable()
    {
      this.ExtendedProperties.Add((object) "Target", (object) "[EBS].[PartListLedger]");
      this.Columns.Add("WOrderNum", typeof (Decimal));
      this.Columns.Add("ItemNum", typeof (Decimal));
      this.Columns.Add("SequenceNum", typeof (Decimal));
      this.Columns.Add("ComponentNum", typeof (Decimal));
      this.Columns.Add("PartCategory", typeof (string));
      this.Columns.Add("PartType1", typeof (string));
      this.Columns.Add("PartType2", typeof (string));
      this.Columns.Add("PartName", typeof (string));
      this.Columns.Add("PartIndex", typeof (int));
      this.Columns.Add("PlannedQty", typeof (Decimal));
      this.Columns.Add("ActualQty", typeof (Decimal));
      this.Columns.Add("UnitOfMeasure", typeof (string));
      this.Columns.Add("Location", typeof (string));
      this.Columns.Add("TransactionDate", typeof (DateTime));
      this.Columns.Add("TransactionId", typeof (Decimal));
    }

    public void Add(IDataReader reader, Plant plant)
    {
      try
      {
        while (reader.Read())
        {
          DataRow row = this.NewRow();
          row["WOrderNum"] = (object) CastDbNull.To<Decimal>(reader["WMDOCO"], Decimal.Zero);
          row["ItemNum"] = (object) CastDbNull.To<Decimal>(reader["WMCPIT"], Decimal.Zero);
          row["SequenceNum"] = (object) CastDbNull.To<Decimal>(reader["WMOPSQ"], Decimal.Zero);
          row["ComponentNum"] = (object) CastDbNull.To<Decimal>(reader["WMCPNB"], Decimal.Zero);
          row["PartCategory"] = (object) CastDbNull.To<string>(reader["IMGLPT"], string.Empty).Trim();
          row["PartType1"] = (object) CastDbNull.To<string>(reader["IMPRP1"], string.Empty).Trim();
          row["PartType2"] = (object) CastDbNull.To<string>(reader["IMPRP2"], string.Empty).Trim();
          row["PlannedQty"] = (object) (CastDbNull.To<Decimal>(reader["WMUORG"], Decimal.Zero) / new Decimal(1000));
          row["ActualQty"] = (object) (CastDbNull.To<Decimal>(reader["WMTRQT"], Decimal.Zero) / new Decimal(1000));
          row["UnitOfMeasure"] = (object) CastDbNull.To<string>(reader["WMUM"], string.Empty).Trim();
          row["Location"] = (object) CastDbNull.To<string>(reader["WMLOCN"], string.Empty).Trim();
          Decimal dateId = CastDbNull.To<Decimal>(reader["WMUPMJ"], Decimal.Zero);
          Decimal timeId = CastDbNull.To<Decimal>(reader["WMTDAY"], Decimal.Zero);
          row["TransactionDate"] = (object) IGServices.Common.Utilities.Common.CastToDateTime(dateId, timeId).AddHours((double) plant.TimeOffset);
          row["TransactionId"] = (object) CastDbNull.To<Decimal>(reader["WMUKID"], Decimal.Zero);
          this.Rows.Add(row);
        }
        reader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
