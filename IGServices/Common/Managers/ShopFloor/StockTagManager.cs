// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.ShopFloor.StockTagManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.ShopFloor
{
  public class StockTagManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public StockTagManager(BusinessSystemSession e1Session)
    {
      Dictionary<string, iDB2Command> dictionary = new Dictionary<string, iDB2Command>();
      foreach (string key in dictionary.Keys.Where<string>((Func<string, bool>) (keyName => !e1Session.DbCommands.ContainsKey(keyName))))
        e1Session.DbCommands.Add(key, dictionary[key]);
    }

    public SqlCommand Extract(List<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in filters)
        stringBuilder.Append(filter);
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandType = CommandType.Text;
      sqlCommand.CommandText = string.Format("                \r\n            SELECT\r\n                ri.WorkOrderNum,\r\n                ri.RackId,\r\n                rk.RackType,\r\n\t\t\t\trk.ScheduleNum,\r\n                ri.SerialNum,\r\n                ri.ItemNum,\r\n                ri.QtyRecd,\r\n\t\t\t\tcp.UnitsPacked,\r\n\t\t\t\tord.Quantity,\r\n\t\t\t\tord.LitesPerRack\r\n            FROM\r\n                EBS.Rack rk\r\n                INNER JOIN EBS.RackDetail ri\r\n                ON  rk.RackId = ri.RackId\r\n\t\t\t\tINNER JOIN LiteSource.MtsE1CompletedRacks cp\r\n\t\t\t\tON cp.E1RackID = rk.RackId\r\n\t\t\t\tINNER JOIN EBS.ScheduledOrder ord\r\n\t\t\t\tON ord.WOrderNum = ri.WorkOrderNum\r\n\t\t\tWHERE \r\n                {0} 1=1", (object) stringBuilder);
      return sqlCommand;
    }

    public StockTagItems Extract(
      BusinessSystemSession session,
      SqlConnection cnShopFloor,
      Plant plant,
      Decimal scheduleNum,
      Decimal rackId,
      List<Decimal> workOrders)
    {
      StockTagItems stockTagItems = new StockTagItems();
      SqlCommand sqlCommand = new SqlCommand();
      List<string> filters = new List<string>();
      List<SqlParameter> source = new List<SqlParameter>();
      try
      {
        if (scheduleNum > Decimal.Zero)
        {
          filters.Add("rk.ScheduleNum = @ScheduleNum AND \r\n");
          source.Add(new SqlParameter("@ScheduleNum", SqlDbType.Decimal));
          source.Last<SqlParameter>().Precision = (byte) 5;
          source.Last<SqlParameter>().Scale = (byte) 0;
          source.Last<SqlParameter>().Value = (object) scheduleNum;
        }
        if (rackId > Decimal.Zero)
        {
          filters.Add("rk.RackId = @RackId AND \r\n");
          source.Add(new SqlParameter("@RackId", SqlDbType.Decimal));
          source.Last<SqlParameter>().Precision = (byte) 15;
          source.Last<SqlParameter>().Scale = (byte) 0;
          source.Last<SqlParameter>().Value = (object) rackId;
        }
        if (workOrders.Any<Decimal>())
        {
          string str = "";
          foreach (Decimal workOrder in workOrders)
            str = str + (object) workOrder + ",";
          filters.Add("ri.WorkOrderNum IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
        }
        SqlCommand cmd = this.Extract(filters);
        foreach (SqlParameter sqlParameter in source)
          cmd.Parameters.Add(sqlParameter);
        cmd.Connection = cnShopFloor;
        StockTagManager.Logger.Info(Logging.DbCmdString(cmd));
        IDataReader dataReader = (IDataReader) cmd.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
          stockTagItems.Add(new StockTagItem()
          {
            PlantNum = plant.PlantNum,
            TransactionType = "TP",
            ScheduleNum = CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
            WorkOrderNum = CastDbNull.To<Decimal>(dataReader["WorkOrderNum"], Decimal.Zero),
            LotNum = CastDbNull.To<Decimal>(dataReader["WorkOrderNum"], Decimal.Zero).ToString(),
            SerialNum = CastDbNull.To<string>(dataReader["SerialNum"], string.Empty).Trim(),
            RackId = CastDbNull.To<Decimal>(dataReader["RackId"], Decimal.Zero),
            ItemNum = CastDbNull.To<Decimal>(dataReader["ItemNum"], Decimal.Zero),
            RackTypeCode = CastDbNull.To<string>(dataReader["RackType"], string.Empty).Trim(),
            OrderQty = CastDbNull.To<Decimal>(dataReader["Quantity"], Decimal.Zero),
            LitesPerRack = CastDbNull.To<Decimal>(dataReader["LitesPerRack"], Decimal.Zero),
            PackQty = (Decimal) CastDbNull.To<int>(dataReader["UnitsPacked"], 0),
            PrintAddress = "x.x.x.x"
          });
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return stockTagItems;
    }

    public SqlDmlResponse SetTransferStatus(
      SqlConnection cn,
      string preStatus,
      string postStatus)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.Connection = cn;
      sqlCommand.CommandType = CommandType.Text;
      SqlCommand cmd = sqlCommand;
      try
      {
        cmd.CommandText = "UPDATE EBS.CrateDimension SET TransferStatus = '" + postStatus + "' WHERE TransferStatus = '" + preStatus + "'";
        StockTagManager.Logger.Trace(Logging.DbCmdString(cmd));
        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        StockTagManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public F580030s Transform(StockTagItems records)
    {
      F580030s f580030s = new F580030s();
      try
      {
        int num = 0;
        foreach (StockTagItem record in (List<StockTagItem>) records)
        {
          F580030 f580030 = new F580030();
          ++num;
          f580030.SFEDUS = "SOASVC";
          f580030.SFUKID = record.TransferId;
          f580030.SFEDBT = record.TransferBatchNum.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          f580030.SFEDTN = num.ToString().PadLeft(10, '0').PadRight(22);
          f580030.SFEDLN = new Decimal(1000);
          f580030.SFEDCT = record.TransactionType;
          f580030.SFTYTN = "IGTAG";
          f580030.SFEDFT = "SVC_IGTAG";
          f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFSTTM = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) DateTime.Now);
          f580030.SFDTMS = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) DateTime.Now.Date);
          f580030.SFDRIN = "1";
          f580030.SFEDSP = "";
          f580030.SFPNID = "TagInfo";
          f580030.SFTNAC = "02";
          f580030.SFMMCU = record.PlantNum.PadLeft(12);
          f580030.SFDOCO = record.WorkOrderNum;
          f580030.SFDCTO = "WO";
          f580030.SFLOTN = record.LotNum;
          f580030.SFZ58SN = record.SerialNum;
          f580030.SFZ58RID = record.RackId;
          f580030.SFLITM = record.ItemNum.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          f580030.SFMCU = "";
          f580030.SFSFT = "";
          f580030.SFUORG = record.PackQty * new Decimal(1000);
          f580030.SFZ58SCN = record.ScheduleNum;
          f580030.SFUSER = "SOASVC";
          f580030.SFPID = "SVC_IGTAG";
          f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
          f580030.SFZ58RT = record.RackTypeCode;
          f580030.SFZ58IP01 = record.PrintAddress;
          f580030s.Add(f580030);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return f580030s;
    }
  }
}
