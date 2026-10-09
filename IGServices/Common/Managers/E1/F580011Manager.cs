// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.E1.F580011Manager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.E1
{
  public class F580011Manager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public iDB2Command GetStatus()
    {
      iDB2Command iDb2Command1 = new iDB2Command();
      iDb2Command1.CommandType = CommandType.Text;
      iDb2Command1.CommandText = "\r\n                    SELECT \r\n                        TIZ58RID,\r\n                        TIDOCO,\r\n                        TIDCTO,\r\n                        TIMCU,\r\n                        TILOTN,\r\n                        TIUORG,\r\n                        TITORG,\r\n                        TIURDT,\r\n                        TIURAT,\r\n                        TIURCD,\r\n                        TIURAB,\r\n                        TIURRF,\r\n                        TIUPMJ,\r\n                        TITDAY,\r\n                        TIUSER,\r\n                        TIPID,\r\n                        TIEDSP\r\n                    FROM \r\n\t                    F580011\r\n\t                WHERE \r\n                        TIMCU = ? AND\r\n                        TIEDSP = ?\r\n                    FOR READ ONLY";
      iDB2Command iDb2Command2 = iDb2Command1;
      iDb2Command2.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
      iDb2Command2.Parameters.Add(new iDB2Parameter("EDSP", iDB2DbType.iDB2Graphic, 1));
      return iDb2Command2;
    }

    public static iDB2Command GetCurrentTags(List<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in filters)
        stringBuilder.Append(filter);
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandText = string.Format("\r\n                    SELECT \r\n                        TIZ58RID,\r\n                        TIDOCO,\r\n                        TIDCTO,\r\n                        TIMCU,\r\n                        TILOTN,\r\n                        TIUORG,\r\n                        TITORG,\r\n                        TIURDT,\r\n                        TIURAT,\r\n                        TIURCD,\r\n                        TIURAB,\r\n                        TIURRF,\r\n                        TIUPMJ,\r\n                        TITDAY,\r\n                        TIUSER,\r\n                        TIPID,\r\n                        TIEDSP\r\n                    FROM \r\n\t                    F580011\r\n\t                WHERE \r\n                        {0}\r\n                        1=1\r\n                    FOR READ ONLY", (object) stringBuilder);
      return iDb2Command;
    }

    public static IDataReader GetCurrentTags(
      BusinessSystemSession session,
      ProcessRunSpan processRunSpan,
      string plantNum,
      Decimal rackId,
      string serialNum)
    {
      List<string> filters = new List<string>();
      List<iDB2Parameter> source = new List<iDB2Parameter>();
      try
      {
        filters.Add("TIMCU = ? AND \r\n");
        source.Add(new iDB2Parameter("TIMCU", iDB2DbType.iDB2Graphic, 12));
        source.Last<iDB2Parameter>().Value = (object) plantNum.PadLeft(12);
        if (processRunSpan.HighWaterMark.Exists)
          filters.Add("TIEDSP = 'R' AND ");
        else if (!processRunSpan.IsEmpty)
          filters.Add(E1TimeManager.GetE1DateTimeQueryString("TIUPMJ", "TITDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
        if (rackId > Decimal.Zero)
        {
          filters.Add("TIZ58RID = ? AND \r\n");
          source.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
          source.Last<iDB2Parameter>().Precision = (byte) 15;
          source.Last<iDB2Parameter>().Scale = (byte) 0;
          source.Last<iDB2Parameter>().Value = (object) rackId;
        }
        if (!string.IsNullOrEmpty(serialNum))
        {
          filters.Add("TILOTN = ? AND \r\n");
          source.Add(new iDB2Parameter("LOTN", iDB2DbType.iDB2Graphic, 30));
          source.Last<iDB2Parameter>().Value = (object) serialNum;
        }
        iDB2Command currentTags = F580011Manager.GetCurrentTags(filters);
        foreach (iDB2Parameter iDb2Parameter in source)
          currentTags.Parameters.Add(iDb2Parameter);
        currentTags.Connection = session.DbConnection;
        F580011Manager.Logger.Debug(Logging.DbCmdString(currentTags));
        return (IDataReader) currentTags.ExecuteReader(CommandBehavior.SingleResult);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    private static iDB2Command UpdateStatus()
    {
      iDB2Command iDb2Command1 = new iDB2Command();
      iDb2Command1.CommandType = CommandType.Text;
      iDb2Command1.CommandText = "\r\n                UPDATE \r\n                    F580011\r\n                SET \r\n                    TIEDSP = ?\r\n                WHERE \r\n                    TIMCU = ? AND\r\n                    TIEDSP != 'Y'";
      iDB2Command iDb2Command2 = iDb2Command1;
      iDb2Command2.Parameters.Add(new iDB2Parameter("EDSP1", iDB2DbType.iDB2Graphic, 1));
      iDb2Command2.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
      return iDb2Command2;
    }

    public static SqlDmlResponse UpdateStatus(
      BusinessSystemSession session,
      Plant plant,
      string preStatus,
      string postStatus)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      try
      {
        iDB2Command cmd = F580011Manager.UpdateStatus();
        cmd.Connection = session.DbConnection;
        cmd.Parameters["EDSP1"].Value = (object) postStatus;
        cmd.Parameters["MCU"].Value = (object) plant.PlantNum.PadLeft(12);
        F580011Manager.Logger.Trace(Logging.DbCmdString(cmd));
        sqlDmlResponse.Init(cmd);
        int rowsAffected = cmd.ExecuteNonQuery();
        F580011Manager.Logger.Trace(string.Format("ROWS: {0}", (object) rowsAffected));
        sqlDmlResponse.Done(rowsAffected);
      }
      catch (Exception ex)
      {
        F580011Manager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(session.DbConnection.ConnectionString + "  " + (object) ex);
      }
      return sqlDmlResponse;
    }
  }
}
