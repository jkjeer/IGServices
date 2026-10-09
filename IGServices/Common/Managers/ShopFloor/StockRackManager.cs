// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.ShopFloor.StockRackManager
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
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.ShopFloor
{
  public class StockRackManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public StockRackManager(BusinessSystemSession e1Session)
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
      sqlCommand.CommandText = string.Format("\r\n            SELECT\r\n                *\r\n            FROM\r\n            (\r\n                SELECT\r\n                    sc.ScheduleId,\r\n                    sc.ScheduleNum,\r\n                    sc.ScheduleDate,\r\n                    sl.WOrderNum,\r\n                    sl.ItemNum,\r\n                    CAST(sl.Quantity AS INT) AS Quantity,\r\n\t\t            sl.RackType,\r\n                    CAST(sl.LitesPerRack AS INT) AS LitesPerRack,\r\n                    ROW_NUMBER() OVER (PARTITION BY\r\n                                           sc.ScheduleNum,\r\n                                            sl.WOrderNum\r\n                                       ORDER BY\r\n                                           sc.ScheduleDate DESC\r\n                                      ) AS RN\r\n\r\n                FROM\r\n                    EBS.Schedule sc\r\n                    INNER JOIN EBS.ScheduledOrder sl\r\n                    ON  sl.E1BatchID = sc.BatchNum AND\r\n                        sl.ScheduleNum = sc.ScheduleNum\r\n                WHERE\r\n                    {0} 1=1\r\n            ) A\r\n            WHERE\r\n                A.RN = 1", (object) stringBuilder);
      return sqlCommand;
    }

    public StockRackItems Extract(
      BusinessSystemSession session,
      SqlConnection cnShopFloor,
      Plant plant,
      Decimal scheduleNum,
      List<Decimal> workOrders)
    {
      StockRackItems stockRackItems = new StockRackItems();
      SqlCommand sqlCommand = new SqlCommand();
      List<string> filters = new List<string>();
      List<SqlParameter> source = new List<SqlParameter>();
      try
      {
        if (scheduleNum > Decimal.Zero)
        {
          filters.Add("sc.ScheduleNum = @ScheduleNum AND \r\n");
          source.Add(new SqlParameter("@ScheduleNum", SqlDbType.Decimal));
          source.Last<SqlParameter>().Precision = (byte) 5;
          source.Last<SqlParameter>().Scale = (byte) 0;
          source.Last<SqlParameter>().Value = (object) scheduleNum;
        }
        if (workOrders.Any<Decimal>())
        {
          string str = "";
          foreach (Decimal workOrder in workOrders)
            str = str + (object) workOrder + ",";
          filters.Add("sl.WOrderNum IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
        }
        SqlCommand cmd = this.Extract(filters);
        foreach (SqlParameter sqlParameter in source)
          cmd.Parameters.Add(sqlParameter);
        cmd.Connection = cnShopFloor;
        StockRackManager.Logger.Info(Logging.DbCmdString(cmd));
        IDataReader dataReader = (IDataReader) cmd.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
        {
          int num1 = CastDbNull.To<int>(dataReader["Quantity"], 0);
          int num2 = CastDbNull.To<int>(dataReader["LitesPerRack"], 0);
          int num3 = num1 / num2;
          int num4 = num1 % num2;
          int num5;
          for (num5 = 0; num3 > num5; ++num5)
            stockRackItems.Add(new StockRackItem()
            {
              PlantNum = plant.PlantNum,
              TransactionType = "TI",
              ScheduleNum = CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
              WorkOrderNum = CastDbNull.To<Decimal>(dataReader["WOrderNum"], Decimal.Zero),
              ItemNum = CastDbNull.To<Decimal>(dataReader["ItemNum"], Decimal.Zero),
              RackTypeCode = CastDbNull.To<string>(dataReader["RackType"], string.Empty).Trim(),
              OrderQty = Convert.ToDecimal(CastDbNull.To<int>(dataReader["Quantity"], 0)),
              LitesPerRack = Convert.ToDecimal(CastDbNull.To<int>(dataReader["LitesPerRack"], 0)),
              PackQty = Convert.ToDecimal(CastDbNull.To<int>(dataReader["LitesPerRack"], 0)),
              PrintAddress = "10.x.x.x",
              BoxNum = num5 + 1
            });
          if (num4 > 0)
            stockRackItems.Add(new StockRackItem()
            {
              PlantNum = plant.PlantNum,
              TransactionType = "TI",
              ScheduleNum = CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
              WorkOrderNum = CastDbNull.To<Decimal>(dataReader["WOrderNum"], Decimal.Zero),
              ItemNum = CastDbNull.To<Decimal>(dataReader["ItemNum"], Decimal.Zero),
              RackTypeCode = CastDbNull.To<string>(dataReader["RackType"], string.Empty).Trim(),
              OrderQty = Convert.ToDecimal(CastDbNull.To<int>(dataReader["Quantity"], 0)),
              LitesPerRack = (Decimal) num4,
              PackQty = (Decimal) num4,
              PrintAddress = "10.x.x.x",
              BoxNum = num5 + 1
            });
        }
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return stockRackItems;
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
        StockRackManager.Logger.Trace(Logging.DbCmdString(cmd));
        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        StockRackManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public F580022s Transform(StockRackItems records)
    {
      F580022s f580022s = new F580022s();
      try
      {
        int num = 0;
        foreach (StockRackItem record in (List<StockRackItem>) records)
        {
          F580022 f580022 = new F580022();
          ++num;
          f580022.CAEDUS = "SOASVC";
          f580022.CAEDBT = record.TransferBatchNum.ToString();
          f580022.CAEDTN = num.ToString().PadLeft(10, '0').PadRight(22);
          f580022.CAEDLN = new Decimal(1000);
          f580022.CAEDCT = record.TransactionType;
          f580022.CATYTN = "IGTAG";
          f580022.CAEDFT = "SVC_IGTAG";
          f580022.CAEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580022.CADRIN = "1";
          f580022.CAEDSP = "";
          f580022.CAPNID = "TagInfo";
          f580022.CATNAC = "02";
          f580022.CAMMCU = record.PlantNum.PadLeft(12);
          f580022.CADOCO = record.WorkOrderNum;
          f580022.CADCTO = "WO";
          f580022.CALOTN = "";
          f580022.CAMCU = "";
          f580022.CASFT = "";
          f580022.CASOQS = record.PackQty * new Decimal(1000);
          f580022.CAUOM = "EA";
          f580022.CAZ58SCN = record.ScheduleNum;
          f580022.CAURCD = "";
          f580022.CAURDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580022.CAURAT = E1TimeManager.CastToE1Time(DateTime.Now);
          f580022.CAURRF = "";
          f580022.CAUSER = "SOASVC";
          f580022.CAPID = "SVC_IGTAG";
          f580022.CAUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
          f580022.CATDAY = E1TimeManager.CastToE1Time(DateTime.Now);
          f580022.CAZ58RT = record.RackTypeCode;
          f580022.CAZ58IP01 = record.PrintAddress;
          f580022s.Add(f580022);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return f580022s;
    }
  }
}
