// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.ShopFloor.GlassBusinessTransactionManager
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
  public class GlassBusinessTransactionManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public GlassBusinessTransactionManager(BusinessSystemSession e1Session)
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
      sqlCommand.CommandText = string.Format("\r\n\t        SELECT \r\n                GlassBusinessTransactionId,\r\n                TransactionSourceCode,\r\n                TransactionId,\r\n                TransactionDate,\r\n                ProductionDate,\r\n                WOrderNum,\r\n                SerialNum,\r\n                ItemNum,\r\n                ScheduleNum,\r\n                LiteNum,\r\n                LineTypeCode,\r\n                ProductionLineCode,\r\n                E1WorkCenterCode,\r\n                LineOperationCode,\r\n                E1OperationCode,\r\n                BusinessTransactionType,\r\n                Quantity,\r\n                RejectReason,\r\n                RejectCode,\r\n                Comments,\r\n                TransferStatus,\r\n                TransferBatchNum,\r\n                TransferId,\r\n                TransferDate,\r\n                TransferMessage,\r\n                WrittenDate\r\n            FROM \r\n                EBS.GlassBusinessTransaction\r\n            WHERE \r\n                TransferStatus = 'R'\r\n            ORDER BY\r\n                TransactionDate", (object) stringBuilder);
      return sqlCommand;
    }

    public GlassBusinessTransactions Extract(
      BusinessSystemSession session,
      SqlConnection cnShopFloor,
      Plant plant,
      string status)
    {
      GlassBusinessTransactions businessTransactions = new GlassBusinessTransactions();
      SqlCommand sqlCommand = new SqlCommand();
      List<string> filters = new List<string>();
      List<SqlParameter> sqlParameterList = new List<SqlParameter>();
      try
      {
        SqlCommand cmd = this.Extract(filters);
        foreach (SqlParameter sqlParameter in sqlParameterList)
          cmd.Parameters.Add(sqlParameter);
        cmd.Connection = cnShopFloor;
        GlassBusinessTransactionManager.Logger.Info(Logging.DbCmdString(cmd));
        IDataReader dataReader = (IDataReader) cmd.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
          businessTransactions.Add(new GlassBusinessTransaction()
          {
            PlantID = plant.PlantNum,
            GlassBusinessTransactionId = CastDbNull.To<int>(dataReader["GlassBusinessTransactionId"], 0),
            TransactionSourceCode = CastDbNull.To<string>(dataReader["TransactionSourceCode"], string.Empty).Trim(),
            TransactionId = CastDbNull.To<int>(dataReader["TransactionId"], 0),
            TransactionType = CastDbNull.To<string>(dataReader["BusinessTransactionType"], string.Empty).Trim(),
            WOrderNum = CastDbNull.To<Decimal>(dataReader["WOrderNum"], Decimal.Zero),
            LotNum = CastDbNull.To<Decimal>(dataReader["WOrderNum"], Decimal.Zero).ToString(),
            LiteNum = CastDbNull.To<int>(dataReader["LiteNum"], 0),
            ScheduleNum = CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
            TransactionDate = new DateTime?(CastDbNull.To<DateTime>(dataReader["TransactionDate"], DateTime.MinValue)),
            TransactionDateChar = CastDbNull.To<DateTime>(dataReader["TransactionDate"], DateTime.MinValue).ToString("MM/dd/yyyy hh:mm:ss tt"),
            ProductionDate = new DateTime?(CastDbNull.To<DateTime>(dataReader["ProductionDate"], DateTime.MinValue)),
            SerialNum = CastDbNull.To<string>(dataReader["SerialNum"], string.Empty).Trim(),
            LineTypeCode = CastDbNull.To<string>(dataReader["LineTypeCode"], string.Empty).Trim(),
            ProductionLineCode = CastDbNull.To<string>(dataReader["ProductionLineCode"], string.Empty).Trim(),
            E1WorkcenterCode = CastDbNull.To<string>(dataReader["E1WorkCenterCode"], string.Empty).Trim(),
            LineOperationCode = CastDbNull.To<string>(dataReader["LineOperationCode"], string.Empty).Trim(),
            E1OperationCode = CastDbNull.To<string>(dataReader["E1OperationCode"], string.Empty).Trim(),
            RejectReason = CastDbNull.To<string>(dataReader["RejectReason"], string.Empty).Trim(),
            RejectCode = CastDbNull.To<string>(dataReader["RejectCode"], string.Empty).Trim(),
            Comments = CastDbNull.To<string>(dataReader["Comments"], string.Empty).Trim(),
            Quantity = (Decimal) CastDbNull.To<int>(dataReader["Quantity"], 0)
          });
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return businessTransactions;
    }

    public SqlDmlResponses SaveTransferProperties(
      GlassBusinessTransactions items,
      short sFUpdateBatchSize,
      SqlConnection cn)
    {
      SqlDmlResponses sqlDmlResponses = new SqlDmlResponses();
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.Connection = cn;
      sqlCommand.CommandType = CommandType.Text;
      SqlCommand cmd = sqlCommand;
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        int num1 = 0;
        foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) items)
        {
          ++num1;
          string str = string.Format("UPDATE EBS.GlassBusinessTransaction SET TransferStatus = '{0}', TransferDate = '{1}', TransferId = {2}, TransferBatchNum = '{3}', TransferMessage = '{4}' WHERE GlassBusinessTransactionId = {5};", (object) businessTransaction.TransferStatus, (object) businessTransaction.TransferDate, (object) businessTransaction.TransferId, (object) businessTransaction.TransferBatchNum, (object) businessTransaction.TransferMessage, (object) businessTransaction.GlassBusinessTransactionId);
          stringBuilder.Append(str);
          if (num1 >= (int) sFUpdateBatchSize)
          {
            cmd.CommandText = stringBuilder.ToString();
            GlassBusinessTransactionManager.Logger.Trace(Logging.DbCmdString(cmd));
            cmd.ExecuteNonQuery();
            stringBuilder = new StringBuilder();
            num1 = 0;
          }
        }
        if (num1 > 0)
        {
          SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
          cmd.CommandText = stringBuilder.ToString();
          sqlDmlResponse.Init(cmd);
          GlassBusinessTransactionManager.Logger.Trace(Logging.DbCmdString(cmd));
          int num2 = cmd.ExecuteNonQuery();
          sqlDmlResponse.RowsAffected = num2;
          sqlDmlResponses.Add(sqlDmlResponse);
        }
        GlassBusinessTransactionManager.Logger.Info("ROWS: " + (object) num1);
      }
      catch (Exception ex)
      {
        GlassBusinessTransactionManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponses;
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
        cmd.CommandText = "UPDATE EBS.GlassBusinessTransaction SET TransferStatus = '" + postStatus + "' WHERE TransferStatus = '" + preStatus + "'";
        GlassBusinessTransactionManager.Logger.Trace(Logging.DbCmdString(cmd));
        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        GlassBusinessTransactionManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public SqlDmlResponse InsertGSBusinessTransactions(SqlConnection cn)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.Connection = cn;
      sqlCommand.CommandType = CommandType.Text;
      SqlCommand cmd = sqlCommand;
      try
      {
        cmd.CommandText = "EXEC [EBS].[usp_Insert_GlassBusinessTransaction_GS]";
        sqlDmlResponse.Init(cmd);
        GlassBusinessTransactionManager.Logger.Trace(Logging.DbCmdString(cmd));
        int num = cmd.ExecuteNonQuery();
        sqlDmlResponse.RowsAffected = num;
        sqlDmlResponse.Done();
      }
      catch (Exception ex)
      {
        GlassBusinessTransactionManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public SqlDmlResponse InsertPPBusinessTransactions(SqlConnection cn)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.Connection = cn;
      sqlCommand.CommandType = CommandType.Text;
      SqlCommand cmd = sqlCommand;
      try
      {
        cmd.CommandText = "EXEC [EBS].[usp_Insert_GlassBusinessTransaction_PP]";
        sqlDmlResponse.Init(cmd);
        GlassBusinessTransactionManager.Logger.Trace(Logging.DbCmdString(cmd));
        int num = cmd.ExecuteNonQuery();
        sqlDmlResponse.RowsAffected = num;
        sqlDmlResponse.Done();
      }
      catch (Exception ex)
      {
        GlassBusinessTransactionManager.Logger.Error("ERROR: " + cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public F580030s Transform(GlassBusinessTransactions records)
    {
      F580030s f580030s = new F580030s();
      try
      {
        int num = 0;
        foreach (GlassBusinessTransaction record in (List<GlassBusinessTransaction>) records)
        {
          F580030 f580030 = new F580030();
          ++num;
          f580030.SFUKID = record.TransferId;
          f580030.SFEDBT = record.TransferBatchNum.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          f580030.SFEDTN = num.ToString().PadLeft(10, '0').PadRight(22);
          f580030.SFEDLN = new Decimal(1000);
          f580030.SFEDUS = "SOASVC";
          f580030.SFTYTN = "IGMTS";
          f580030.SFEDFT = "IGMTS";
          f580030.SFDRIN = "1";
          f580030.SFEDSP = "";
          f580030.SFPNID = "IGGlassTran";
          f580030.SFTNAC = "02";
          f580030.SFMMCU = record.PlantID.PadLeft(12);
          f580030.SFEDCT = record.TransactionType;
          f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFSTTM = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) record.TransactionDate);
          f580030.SFDTMS = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) record.ProductionDate);
          f580030.SFLOTN = "";
          f580030.SFDOCO = record.WOrderNum;
          f580030.SFDCTO = record.WOrderNum > Decimal.Zero ? "WO" : "";
          f580030.SFZ58SN = record.SerialNum;
          f580030.SFZ58SCN = record.ScheduleNum;
          f580030.SFZ58L1CLN = (Decimal) record.LiteNum;
          f580030.SFMCU = record.E1WorkcenterCode.PadLeft(12);
          f580030.SFZ58LWSTN = record.E1OperationCode;
          f580030.SFUORG = record.Quantity * new Decimal(1000);
          f580030.SFZ58UID = record.GlassBusinessTransactionId.ToString();
          f580030.SFZ58RCC = record.RejectReason;
          f580030.SFZ58RCP = record.RejectCode;
          f580030.SFZ58PT = record.Comments;
          f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
          f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
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
