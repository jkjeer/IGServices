// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.E1.ScheduleBackoutEventManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.E1
{
  public static class ScheduleBackoutEventManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static SqlCommand GetEvents(List<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in filters)
        stringBuilder.Append(filter);
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandType = CommandType.Text;
      sqlCommand.CommandText = string.Format("\r\n                SELECT \r\n                    ScheduleBackoutTransferLogID,\r\n                    ScheduleId,\r\n                    ScheduleNum,\r\n                    ScheduleDate\r\n                FROM\r\n                    EBS.ScheduleBackoutTransferLog\r\n                WHERE \r\n                    {0}\r\n                    1=1", (object) stringBuilder);
      return sqlCommand;
    }

    public static ScheduleEvents GetEvents(
      SqlConnection cn,
      Plant plant,
      string status,
      Decimal scheduleNum)
    {
      ScheduleEvents scheduleEvents = new ScheduleEvents();
      List<string> filters = new List<string>();
      List<SqlParameter> source = new List<SqlParameter>();
      try
      {
        filters.Add("ISNULL(TransferStatus, 'N') = @TransferStatus AND\r\n");
        source.Add(new SqlParameter("@TransferStatus", SqlDbType.VarChar, 10));
        source.Last<SqlParameter>().Value = (object) "N";
        if (scheduleNum > Decimal.Zero)
        {
          filters.Add("ScheduleNum = @ScheduleNum AND\r\n");
          source.Add(new SqlParameter("@ScheduleNum", SqlDbType.Int));
          source.Last<SqlParameter>().Value = (object) scheduleNum;
        }
        SqlCommand events = ScheduleBackoutEventManager.GetEvents(filters);
        foreach (SqlParameter sqlParameter in source)
          events.Parameters.Add(sqlParameter);
        events.Connection = cn;
        ScheduleBackoutEventManager.Logger.Trace(Logging.DbCmdString(events));
        IDataReader dataReader = (IDataReader) events.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
          scheduleEvents.Add(new ScheduleEvent()
          {
            Plant = plant,
            ScheduleId = IGServices.Common.Utilities.Data.CastDbNull.To<int>(dataReader["ScheduleId"], 0),
            ScheduleNum = IGServices.Common.Utilities.Data.CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
            ScheduleDate = IGServices.Common.Utilities.Data.CastDbNull.To<DateTime>(dataReader["ScheduleDate"], DateTime.MinValue),
            TransferLogId = IGServices.Common.Utilities.Data.CastDbNull.To<int>(dataReader["ScheduleBackoutTransferLogID"], 0)
          });
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.ToString());
      }
      return scheduleEvents;
    }

    public static SqlCommand GetScheduleEvent()
    {
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandType = CommandType.Text;
      sqlCommand.CommandText = "\r\n                SELECT\r\n                    --BatchNum,\r\n                    ScheduleId,\r\n                    ScheduleNum,\r\n                    ScheduleDate\r\n                FROM\r\n                (\r\n                    SELECT\r\n                        --BatchNum,\r\n                        ScheduleId,\r\n                        ScheduleNum,\r\n                        ScheduleDate,\r\n                        ROW_NUMBER() OVER (PARTITION BY\r\n                                               ScheduleNum\r\n                                           ORDER BY\r\n                                               ScheduleDate DESC\r\n                                          ) AS RN\r\n                    FROM dbo.Schedule\r\n                        --EBS.Schedule\r\n                    WHERE\r\n                        IsCurrentVersion = 1 AND\r\n                        ScheduleNum = @ScheduleNum\r\n                ) A\r\n                WHERE\r\n                    A.RN = 1;";
      sqlCommand.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Int));
      return sqlCommand;
    }

    public static ScheduleEvents GetEvent(
      SqlConnection cn,
      Plant plant,
      Decimal scheduleNum)
    {
      ScheduleEvents scheduleEvents = new ScheduleEvents();
      try
      {
        SqlCommand scheduleEvent = ScheduleBackoutEventManager.GetScheduleEvent();
        scheduleEvent.Parameters["@ScheduleNum"].Value = (object) scheduleNum;
        scheduleEvent.Connection = cn;
        ScheduleBackoutEventManager.Logger.Trace(Logging.DbCmdString(scheduleEvent));
        IDataReader dataReader = (IDataReader) scheduleEvent.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
          scheduleEvents.Add(new ScheduleEvent()
          {
            Plant = plant,
            ScheduleId = IGServices.Common.Utilities.Data.CastDbNull.To<int>(dataReader["ScheduleId"], 0),
            ScheduleNum = IGServices.Common.Utilities.Data.CastDbNull.To<Decimal>(dataReader["ScheduleNum"], Decimal.Zero),
            ScheduleDate = IGServices.Common.Utilities.Data.CastDbNull.To<DateTime>(dataReader["ScheduleDate"], DateTime.MinValue),
            TransferLogId = 0
          });
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.ToString());
      }
      return scheduleEvents;
    }

    public static SqlDmlResponse ReserveStatus(SqlConnection cn, ScheduleEvent evt)
    {
      SqlDmlResponse sqlDmlResponse;
      try
      {
        sqlDmlResponse = ScheduleBackoutEventManager.UpdateStatus(cn, evt, "R");
      }
      catch (Exception ex)
      {
        ScheduleBackoutEventManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }

    public static SqlDmlResponse UpdateBeforeStatus(
      SqlConnection cn,
      ScheduleEvent evt)
    {
      SqlDmlResponse sqlDmlResponse;
      try
      {
        sqlDmlResponse = ScheduleBackoutEventManager.UpdateStatus(cn, evt, "U");
      }
      catch (Exception ex)
      {
        ScheduleBackoutEventManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }

    public static SqlDmlResponse UpdateAfterFail(SqlConnection cn, ScheduleEvent evt)
    {
      SqlDmlResponse sqlDmlResponse;
      try
      {
        sqlDmlResponse = ScheduleBackoutEventManager.UpdateStatus(cn, evt, "E");
      }
      catch (Exception ex)
      {
        ScheduleBackoutEventManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }

    public static SqlDmlResponse UpdateAfterSuccess(
      SqlConnection cn,
      ScheduleEvent evt)
    {
      SqlDmlResponse sqlDmlResponse;
      try
      {
        sqlDmlResponse = ScheduleBackoutEventManager.UpdateStatus(cn, evt, "Y");
      }
      catch (Exception ex)
      {
        ScheduleBackoutEventManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }

    private static SqlCommand UpdateStatus()
    {
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandType = CommandType.Text;
      sqlCommand1.CommandText = "\r\n                UPDATE \r\n                    EBS.ScheduleBackoutTransferLog\r\n                SET \r\n                    TransferStatus = @StatusCode\r\n                WHERE \r\n                    ScheduleBackoutTransferLogId = @ScheduleBackoutTransferLogId";
      SqlCommand sqlCommand2 = sqlCommand1;
      sqlCommand2.Parameters.Add(new SqlParameter("@StatusCode", SqlDbType.VarChar, 10));
      sqlCommand2.Parameters.Add(new SqlParameter("@ScheduleBackoutTransferLogId", SqlDbType.Int));
      return sqlCommand2;
    }

    public static SqlDmlResponse UpdateStatus(
      SqlConnection cn,
      ScheduleEvent evt,
      string statusCode)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      try
      {
        SqlCommand cmd = ScheduleBackoutEventManager.UpdateStatus();
        cmd.Connection = cn;
        cmd.Parameters["@StatusCode"].Value = (object) statusCode;
        cmd.Parameters["@ScheduleBackoutTransferLogId"].Value = (object) evt.TransferLogId;
        ScheduleBackoutEventManager.Logger.Trace(Logging.DbCmdString(cmd));
        sqlDmlResponse.Init(cmd);
        int rowsAffected = cmd.ExecuteNonQuery();
        sqlDmlResponse.Done(rowsAffected);
      }
      catch (Exception ex)
      {
        ScheduleBackoutEventManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(cn.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }

    public static SqlDmlResponse SaveTransfer(SqlConnection cn, ScheduleEvent evt)
    {
      SqlDmlResponse sqlDmlResponse;
      try
      {
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = cn;
        sqlCommand.CommandType = CommandType.Text;
        sqlCommand.CommandText = string.Format("\r\n                    UPDATE\r\n                        EBS.ScheduleBackoutTransferLog\r\n                    SET     \r\n                        TransferTS = GETDATE(),\r\n                        TransferID = SUSER_NAME(),\r\n                        TransferWS = HOST_NAME(),\r\n                        TransferBatchNum = '{0}',\r\n                        TransferTransactionId = {1},\r\n                        TransferMessage = '{2}',\r\n                        TransferStatus = '{3}'\r\n                    WHERE\r\n                        ScheduleBackoutTransferLogId = {4}", (object) evt.TransferBatchNum, (object) evt.TransferTransactionId, (object) evt.TransferMessage, (object) evt.TransferStatusCode, (object) evt.TransferLogId);
        SqlCommand cmd = sqlCommand;
        sqlDmlResponse = new SqlDmlResponse(cmd);
        ScheduleBackoutEventManager.Logger.Trace(Logging.DbCmdString(cmd));
        int num = cmd.ExecuteNonQuery();
        sqlDmlResponse.RowsAffected = num;
        sqlDmlResponse.Done();
      }
      catch (Exception ex)
      {
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }
  }
}
