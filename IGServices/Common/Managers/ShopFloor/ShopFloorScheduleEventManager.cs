// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.ShopFloor.ShopFloorScheduleEventManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IGServices.Common.Managers.ShopFloor
{
  public static class ShopFloorScheduleEventManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static SqlDmlResponse Save(
      SqlConnection cn,
      ScheduleEvent evt,
      string eventCode,
      string message)
    {
      SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
      try
      {
        SqlCommand cmd = ShopFloorScheduleEventManager.Insert();
        cmd.Parameters["@ScheduleId"].Value = (object) evt.ScheduleId;
        cmd.Parameters["@ScheduleNum"].Value = (object) evt.ScheduleNum;
        cmd.Parameters["@ScheduleDate"].Value = (object) evt.ScheduleDate;
        cmd.Parameters["@EventCode"].Value = (object) eventCode;
        cmd.Parameters["@EventDate"].Value = (object) DateTime.Now;
        cmd.Parameters["@EventMessage"].Value = (object) message;
        cmd.Connection = cn;
        sqlDmlResponse.Init(cmd);
        ShopFloorScheduleEventManager.Logger.Trace(Logging.DbCmdString(cmd));
        int num = cmd.ExecuteNonQuery();
        sqlDmlResponse.RowsAffected = num;
      }
      catch (Exception ex)
      {
                Logger.Error($@"ERROR: {cn.ConnectionString}  {ex.Message}: {ex.StackTrace}");
                sqlDmlResponse.Errors.Add(new Error(ex, true));
        throw new Exception(cn.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return sqlDmlResponse;
    }

    public static SqlCommand Insert()
    {
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandType = CommandType.Text;
      sqlCommand1.CommandText = "INSERT INTO EBS.ScheduleEventLedger " +
                                "([ScheduleId],[ScheduleNum],[ScheduleDate],[EventCode],[EventDate],[EventMessage],[WrittenDate]) " +
                                "VALUES (@ScheduleId,@ScheduleNum,@ScheduleDate,@EventCode,@EventDate,@EventMessage, GETDATE())";
      SqlCommand sqlCommand2 = sqlCommand1;
      sqlCommand2.Parameters.Add(new SqlParameter("@ScheduleId", SqlDbType.Int));
      sqlCommand2.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
      sqlCommand2.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
      sqlCommand2.Parameters.Add(new SqlParameter("@EventCode", SqlDbType.VarChar, 12));
      sqlCommand2.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.DateTime));
      sqlCommand2.Parameters.Add(new SqlParameter("@EventMessage", SqlDbType.VarChar, 1000));
      return sqlCommand2;
    }
  }
}
