// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.PartListDownloadManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
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

namespace IGServices.Common.Managers.Process
{
  public class PartListDownloadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static ProcessResponse Process(
      BusinessSystemSession session,
      Dictionary<string, SqlConnection> sqlCns,
      Plant plant,
      TimeSpan timeSpan,
      bool useHighWaterMark,
      Decimal orderNum)
    {
      ProcessResponse processResponse = new ProcessResponse("Part List Download Manager", false, (string) null);
      processResponse.Plant = plant;
      processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
      foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
        processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
      PartListTable partListTable = new PartListTable();
      ProcessRunSpan processRunSpan = new ProcessRunSpan();
      HighWaterMark mark = new HighWaterMark();
      ProcessStep step1 = new ProcessStep("Resolve Run Span");
      try
      {
        mark = useHighWaterMark ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "PartList") : (HighWaterMark) null;
        processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, timeSpan, E1TimeManager.GetE1TimeStamp(session, (string) null));
        step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
      }
      catch (Exception ex)
      {
        step1.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
      }
      finally
      {
        step1.Done();
        processResponse.AddStep(step1);
      }
      if (!processResponse.HasErrors())
      {
        ProcessStep step2 = new ProcessStep("Get Source Data");
        try
        {
          IDataReader data = PartListDownloadManager.GetData(session, processRunSpan, plant.PlantNum, orderNum);
          partListTable.Add(data, plant);
          step2.RowsAffected = partListTable.Rows.Count;
          processResponse.RowsAffected = partListTable.Rows.Count;
        }
        catch (Exception ex)
        {
          step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        step2.Done();
        processResponse.AddStep(step2);
      }
      if (partListTable.Rows.Count > 0)
      {
        ProcessStep step2 = new ProcessStep("Bulk Copy");
        SqlBulkCopyResponse bulkCopyResponse = new SqlBulkCopyResponse();
        try
        {
          if (partListTable.Rows.Count > 0)
          {
            processResponse.RowsAffected = partListTable.Rows.Count;
            bulkCopyResponse = BulkCopyManager.BulkTable(partListTable.ExtendedProperties[(object) "Target"].ToString(), (DataTable) partListTable, sqlCns["PLANT.INTEGRATION"], (SqlTransaction) null);
          }
        }
        catch (Exception ex)
        {
          step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          step2.RowsAffected = (int) bulkCopyResponse.RowsCopied;
          step2.SqlBulkCopyResponses.Add(bulkCopyResponse);
          step2.Done();
          processResponse.RowsAffected = step2.RowsAffected;
          processResponse.AddStep(step2);
        }
      }
      if (useHighWaterMark)
      {
        ProcessStep step2 = new ProcessStep("Update High Water Mark");
        try
        {
          if (partListTable.Rows.Count > 0)
          {
            mark.LocalDateTime = partListTable.MaxDate;
            mark.EnterpriseDateTime = partListTable.MaxDate.AddHours((double) -plant.TimeOffset);
            mark.UniqueID = (long) partListTable.MaxId;
          }
          step2.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], mark));
        }
        catch (Exception ex)
        {
          step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          step2.Done();
          processResponse.AddStep(step2);
        }
      }
      processResponse.Close();
      return processResponse;
    }

    private static iDB2Command GetData(IReadOnlyCollection<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in (IEnumerable<string>) filters)
        stringBuilder.Append(filter);
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandText = string.Format("\r\nSELECT DISTINCT\r\n    WM2.WMMCU,\r\n\tWM2.WMDOCO,\r\n\tWM2.WMDCTO,\r\n\tWM2.WMOPSQ,\r\n\tWM2.WMCPNB,\r\n\tWM2.WMCPIT,\r\n\tWM2.WMLOCN,\r\n\tWM2.WMUORG,\r\n\tWM2.WMTRQT,\r\n\tWM2.WMUM,\r\n\tWM2.WMUKID,\r\n\tWM2.WMUPMJ,\r\n    WM2.WMTDAY,\r\n    IMGLPT,\r\n    IMPRP1,\r\n    IMPRP2\r\nFROM \r\n\tF3111 WM1\r\n    INNER JOIN F4801\r\n    ON WADOCO = WM1.WMDOCO\r\n\tINNER JOIN F3111 WM2\r\n\tON WM2.WMDOCO = WM1.WMDOCO\r\n    INNER JOIN F4101\r\n    ON IMITM = WM2.WMCPIT\r\nWHERE \r\n    {0}\r\n\tWATYPS IN ('1', 'S')\r\nORDER BY\r\n\tWM2.WMDOCO, \r\n    WM2.WMCPNB\r\nFOR READ ONLY", (object) stringBuilder);
      return iDb2Command;
    }

    public static IDataReader GetData(
      BusinessSystemSession session,
      ProcessRunSpan processRunSpan,
      string plantNum,
      Decimal orderNum)
    {
      List<string> stringList = new List<string>();
      List<iDB2Parameter> source = new List<iDB2Parameter>();
      try
      {
        stringList.Add("WM1.WMMCU = ? AND \r\n");
        source.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
        source.Last<iDB2Parameter>().Value = (object) plantNum.PadLeft(12);
        if (orderNum > Decimal.Zero)
        {
          stringList.Add("WM1.WMDOCO = ? AND \r\n");
          source.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
          source.Last<iDB2Parameter>().Precision = (byte) 8;
          source.Last<iDB2Parameter>().Scale = (byte) 0;
          source.Last<iDB2Parameter>().Value = (object) orderNum;
        }
        if (!processRunSpan.IsEmpty)
          stringList.Add(E1TimeManager.GetE1DateTimeQueryString("WM1.WMUPMJ", "WM1.WMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
        iDB2Command data = PartListDownloadManager.GetData((IReadOnlyCollection<string>) stringList);
        foreach (iDB2Parameter iDb2Parameter in source)
          data.Parameters.Add(iDb2Parameter);
        data.Connection = session.DbConnection;
        PartListDownloadManager.Logger.Debug(Logging.DbCmdString(data));
        return (IDataReader) data.ExecuteReader(CommandBehavior.SingleResult);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}
