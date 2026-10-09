// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.RackDetailDownloadManager
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
  public class RackDetailDownloadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RackDetailDownloadManager(BusinessSystemSession e1Session)
    {
      Dictionary<string, iDB2Command> dictionary = new Dictionary<string, iDB2Command>();
      foreach (string key in dictionary.Keys.Where<string>((Func<string, bool>) (keyName => !e1Session.DbCommands.ContainsKey(keyName))))
        e1Session.DbCommands.Add(key, dictionary[key]);
    }

    public ProcessResponse Process(
      BusinessSystemSession session,
      Dictionary<string, SqlConnection> sqlCns,
      Plant plant,
      TimeSpan timeSpan,
      bool useHighWaterMark,
      Decimal rackId,
      List<string> statusCodes)
    {
      ProcessResponse response = new ProcessResponse("Rack Detail Download Manager", false, (string) null);
      response.Plant = plant;
      response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
      foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
        response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
      ProcessStep step1 = new ProcessStep("Resolve Run Spans");
      ProcessRunSpan processRunSpan = new ProcessRunSpan();
      HighWaterMark mark = new HighWaterMark();
      try
      {
        mark = useHighWaterMark ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "RackDetail") : (HighWaterMark) null;
        processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, timeSpan, E1TimeManager.GetE1TimeStamp(session, (string) null));
        step1.Messages.Add(new Message("RunSpanRackItem", processRunSpan.Serialize()));
      }
      catch (Exception ex)
      {
        step1.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
      }
      finally
      {
        step1.Done();
        response.AddStep(step1);
      }
      if (!response.HasErrors())
      {
        RackItemTable rackItemTable = new RackItemTable();
        ProcessStep step2 = new ProcessStep("Get Source Data");
        try
        {
          if (useHighWaterMark)
          {
            IDataReader rackItemLedger = RackDetailDownloadManager.GetRackItemLedger(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
            rackItemTable.Add(rackItemLedger, plant);
          }
          else
          {
            IDataReader rackItems = RackDetailDownloadManager.GetRackItems(session, processRunSpan, plant.PlantNum, rackId, statusCodes);
            rackItemTable.Add(rackItems, plant);
          }
          step2.RowsAffected = rackItemTable.Rows.Count;
          response.RowsAffected = rackItemTable.Rows.Count;
        }
        catch (Exception ex)
        {
          step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        step2.Done();
        response.AddStep(step2);
        if (rackItemTable.Rows.Count > 0)
        {
          ProcessStep step3 = new ProcessStep("Bulk Copy");
          SqlBulkCopyResponse bulkCopyResponse = new SqlBulkCopyResponse();
          try
          {
            if (rackItemTable.Rows.Count > 0)
              bulkCopyResponse = BulkCopyManager.BulkTable(rackItemTable.ExtendedProperties[(object) "Target"].ToString(), (DataTable) rackItemTable, sqlCns["PLANT.INTEGRATION"], (SqlTransaction) null);
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.RowsAffected = (int) bulkCopyResponse.RowsCopied;
            step3.SqlBulkCopyResponses.Add(bulkCopyResponse);
            step3.Done();
            response.RowsAffected = step3.RowsAffected;
            response.AddStep(step3);
          }
        }
        if (useHighWaterMark && !response.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Update High Water Marks");
          try
          {
            if (rackItemTable.Rows.Count > 0)
            {
              mark.LocalDateTime = rackItemTable.MaxDate.AddHours((double) plant.TimeOffset);
              mark.EnterpriseDateTime = rackItemTable.MaxDate;
              if (rackItemTable.MaxId > Decimal.Zero)
                mark.UniqueID = (long) rackItemTable.MaxId;
            }
            step3.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], mark));
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            response.AddStep(step3);
          }
        }
      }
      response.Done();
      Logging.InsertProcessLog(response, false);
      return response;
    }

    public static IDataReader GetRackItems(
      BusinessSystemSession session,
      ProcessRunSpan processRunSpan,
      string plantNum,
      Decimal rackId,
      List<string> statusCodes)
    {
      List<string> stringList = new List<string>();
      List<iDB2Parameter> source = new List<iDB2Parameter>();
      try
      {
        if (!statusCodes.Any<string>() && processRunSpan.IsEmpty && rackId == Decimal.Zero)
          stringList.Add("0 = 1 AND \r\n");
        stringList.Add("RDMCU = ? AND \r\n");
        source.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
        source.Last<iDB2Parameter>().Value = (object) plantNum.PadLeft(12);
        if (rackId > Decimal.Zero)
        {
          stringList.Add("RDZ58RID = ? AND \r\n");
          source.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
          source.Last<iDB2Parameter>().Precision = (byte) 8;
          source.Last<iDB2Parameter>().Scale = (byte) 0;
          source.Last<iDB2Parameter>().Value = (object) rackId;
        }
        if (statusCodes.Any<string>())
        {
          string str = "";
          foreach (string statusCode in statusCodes)
          {
            str += "?,";
            source.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
            source.Last<iDB2Parameter>().Value = (object) statusCode;
          }
          stringList.Add("RDZ58DST IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
        }
        if (!processRunSpan.IsEmpty)
          stringList.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
        iDB2Command rackItems = RackDetailDownloadManager.GetRackItems((IReadOnlyCollection<string>) stringList);
        for (int index = 0; index < 1; ++index)
        {
          foreach (iDB2Parameter iDb2Parameter in source)
            rackItems.Parameters.Add(iDb2Parameter);
        }
        rackItems.Connection = session.DbConnection;
        RackDetailDownloadManager.Logger.Debug(Logging.DbCmdString(rackItems));
        return (IDataReader) rackItems.ExecuteReader(CommandBehavior.SingleResult);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    private static iDB2Command GetRackItems(IReadOnlyCollection<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in (IEnumerable<string>) filters)
        stringBuilder.Append(filter);
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandTimeout = 0;
      iDb2Command.CommandText = string.Format("\r\nSELECT \r\n\t*\r\nFROM (\r\n\tSELECT\r\n\t\tRDLOTN AS LOTN,\r\n\t\tRDZ58SN AS Z58SN,\r\n\t\tRDITM AS ITM,\r\n\t\tRDMCU AS MCU,\r\n\t\tRDZ58DST AS Z58DST,\r\n\t\tRDZ58RID AS Z58RID,\r\n\t\tRDZ58MDT AS Z58MDT,\r\n\t\tRDZ58PDT AS Z58PDT,\r\n\t\tRDPQOH AS PQOH,\r\n        RDZ58CQTY AS RQOH,\r\n\t\tRDZ58PS AS Z58PS,\r\n\t\tRDZ58SL AS Z58SL,\r\n\t\tRDZ58STK AS Z58STK,\r\n\t\tRDZ58ROW AS Z58ROW,\r\n\t\tRDZ58SD AS Z58SD,\r\n\t\tRDZ58BXRO AS Z58BXRO,\r\n\t\tRDDOC1 AS DOC1,\r\n\t\tRDPKCO AS PKCO,\r\n\t\tRDDOC2 AS DOC2,\r\n\t\tRDXCTO AS XCTO,\r\n\t\tRDLNIC AS LNIC,\r\n\t\tRDKCOO AS KCOO,\r\n\t\tRDDOCO AS DOCO,\r\n\t\tRDDCTO AS DCTO,\r\n\t\tRDLNID AS LNID,\r\n\t\tRDUPMJ AS UPMJ,\r\n\t\tRDTDAY AS TDAY,\r\n\t\tCAST(0 AS DECIMAL(15,0)) AS UKID,\r\n\t\tCAST('Init' AS VARCHAR(10)) AS IOPR,\r\n        COALESCE(DLMUPM, 0) AS MUPM,\r\n        COALESCE(DLUPMT, 0) AS UPMT,\r\n        RDUSER AS USERID,\r\n        RDPID AS PID,\r\n\t\tIBSTKT AS STKT,\r\n\t\tIBGLPT AS GLPT,\r\n\t\tCOALESCE(PDMCU, '') AS PDMCU,\r\n        COALESCE(PDAN8, 0) AS PDAN8,\r\n        COALESCE(PCUST.ABALPH, '') AS PDALPH,\r\n\t\tCOALESCE(SDMCU, '') AS SDMCU,\r\n        COALESCE(SDAN8, 0) AS SDAN8,\r\n        COALESCE(SCUST.ABALPH, '') AS SDALPH,\r\n\t\tCOALESCE(WAMCU, '') AS WAMCU,\r\n        COALESCE(WAAN8, 0) AS WAAN8,\r\n        COALESCE(WCUST.ABALPH, '') AS WAALPH,\r\n        COALESCE(CMCFGID, 0) AS CFGID,\r\n\t\tROW_NUMBER() OVER (PARTITION BY RDLOTN, RDZ58SN ORDER BY CMCFGID DESC) AS RN\r\n\tFROM\r\n\t\tF580056\r\n\t\tINNER JOIN F4102\r\n\t\tON\tIBMCU = RDMCU AND\r\n\t\t\tIBITM = RDITM\r\n\t\tLEFT OUTER JOIN F4311 LEFT OUTER JOIN F0101 PCUST ON PCUST.ABAN8 = PDAN8\r\n\t\tON  PDKCOO = RDPKCO AND\r\n\t\t\tPDDOCO = RDDOC2 AND\r\n\t\t\tPDDCTO = RDXCTO AND\r\n\t\t\tPDLNID = RDLNIC AND\r\n\t\t\tPDMCU = RDMCU\r\n\t\tLEFT OUTER JOIN F4211 LEFT OUTER JOIN F0101 SCUST ON SCUST.ABAN8 = SDAN8\r\n\t\tON  SDKCOO = RDKCOO AND\r\n\t\t\tSDDOCO = RDDOCO AND\r\n\t\t\tSDDCTO = RDDCTO AND\r\n\t\t\tSDLNID = RDLNID AND\r\n\t\t\tSDMCU = RDMCU\r\n\t\tLEFT OUTER JOIN F4801 LEFT OUTER JOIN F0101 WCUST ON WCUST.ABAN8 = WAAN8\r\n\t\tON\tWADOCO = RDDOC1 AND\r\n\t\t\tWAMCU = RDMCU\r\n\t\tLEFT OUTER JOIN F3201\r\n\t\tON\tCMKCOO = COALESCE(PDKCOO, SDKCOO) AND\r\n\t\t\tCMDOCO = COALESCE(PDDOCO, SDDOCO) AND\r\n\t\t\tCMDCTO = COALESCE(PDDCTO, SDDCTO) AND\r\n\t\t\tCMLNID = COALESCE(PDLNID, SDLNID) \r\n        LEFT OUTER JOIN F580569\r\n        ON  DLMCU = RDMCU AND\r\n            DLLOTN = RDLOTN AND\r\n            DLITM = RDITM AND\r\n            DLZ58SN = RDZ58SN AND\r\n            DLIOPR = 'Insert'\r\n\tWHERE\r\n        {0} 1=1\r\n\t) A\r\nWHERE \r\n\tA.RN = 1\r\nFOR READ ONLY", (object) stringBuilder);
      return iDb2Command;
    }

    public static IDataReader GetRackItemLedger(
      BusinessSystemSession session,
      ProcessRunSpan processRunSpan,
      string plantNum,
      Decimal rackId,
      List<string> statusCodes)
    {
      List<string> stringList = new List<string>();
      List<iDB2Parameter> source = new List<iDB2Parameter>();
      try
      {
        if (processRunSpan.HighWaterMark.Exists && processRunSpan.HighWaterMark.UniqueID == 0L)
          stringList.Add("0 = 1 AND \r\n");
        stringList.Add("DLMCU = ? AND \r\n");
        source.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
        source.Last<iDB2Parameter>().Value = (object) plantNum.PadLeft(12);
        if (rackId > Decimal.Zero)
        {
          stringList.Add("DLZ58RID = ? AND \r\n");
          source.Add(new iDB2Parameter("RID", iDB2DbType.iDB2Decimal));
          source.Last<iDB2Parameter>().Precision = (byte) 8;
          source.Last<iDB2Parameter>().Scale = (byte) 0;
          source.Last<iDB2Parameter>().Value = (object) rackId;
        }
        if (statusCodes.Any<string>())
        {
          string str = "";
          foreach (string statusCode in statusCodes)
          {
            str += "?,";
            source.Add(new iDB2Parameter("DST", iDB2DbType.iDB2Graphic, 1));
            source.Last<iDB2Parameter>().Value = (object) statusCode;
          }
          stringList.Add("DLZ58DST IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
        }
        if (processRunSpan.HighWaterMark.Exists)
        {
          stringList.Add("DLUKID > ? AND \r\n");
          source.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
          source.Last<iDB2Parameter>().Precision = (byte) 15;
          source.Last<iDB2Parameter>().Scale = (byte) 0;
          source.Last<iDB2Parameter>().Value = (object) processRunSpan.HighWaterMark.UniqueID;
        }
        else if (!processRunSpan.IsEmpty)
          stringList.Add(E1TimeManager.GetE1DateTimeQueryString("DLMUPM", "DLUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
        iDB2Command rackItemLedger = RackDetailDownloadManager.GetRackItemLedger((IReadOnlyCollection<string>) stringList);
        for (int index = 0; index < 2; ++index)
        {
          foreach (iDB2Parameter iDb2Parameter in source)
            rackItemLedger.Parameters.Add(iDb2Parameter);
        }
        rackItemLedger.Connection = session.DbConnection;
        RackDetailDownloadManager.Logger.Debug(Logging.DbCmdString(rackItemLedger));
        return (IDataReader) rackItemLedger.ExecuteReader(CommandBehavior.SingleResult);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    private static iDB2Command GetRackItemLedger(IReadOnlyCollection<string> filters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string filter in (IEnumerable<string>) filters)
        stringBuilder.Append(filter);
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandTimeout = 0;
      iDb2Command.CommandText = string.Format("\r\nSELECT \r\n\t*\r\nFROM (\r\n\tSELECT\r\n\t\tDLLOTN AS LOTN,\r\n\t\tDLZ58SN AS Z58SN,\r\n\t\tDLITM AS ITM,\r\n\t\tDLMCU AS MCU,\r\n\t\tDLZ58DST AS Z58DST,\r\n\t\tDLZ58RID AS Z58RID,\r\n\t\tDLZ58MDT AS Z58MDT,\r\n\t\tDLZ58PDT AS Z58PDT,\r\n\t\tDLPQOH AS PQOH,\r\n        DLZ58CQTY AS RQOH,\r\n\t\tDLZ58PS AS Z58PS,\r\n\t\tDLZ58SL AS Z58SL,\r\n\t\tDLZ58STK AS Z58STK,\r\n\t\tDLZ58ROW AS Z58ROW,\r\n\t\tDLZ58SD AS Z58SD,\r\n\t\tDLZ58BXRO AS Z58BXRO,\r\n\t\tDLDOC1 AS DOC1,\r\n\t\tDLPKCO AS PKCO,\r\n\t\tDLDOC2 AS DOC2,\r\n\t\tDLXCTO AS XCTO,\r\n\t\tDLLNIC AS LNIC,\r\n\t\tDLKCOO AS KCOO,\r\n\t\tDLDOCO AS DOCO,\r\n\t\tDLDCTO AS DCTO,\r\n\t\tDLLNID AS LNID,\r\n\t\tDLMUPM AS UPMJ,\r\n\t\tDLUPMT AS TDAY,\r\n\t\tDLUKID AS UKID,\r\n\t\tDLIOPR AS IOPR,\r\n\t\tDLMUPM AS MUPM,\r\n\t\tDLUPMT AS UPMT,\r\n        DLUSER AS USERID,\r\n        DLPID AS PID,\r\n\t\tIBSTKT AS STKT,\r\n\t\tIBGLPT AS GLPT,\r\n\t\tCOALESCE(PDMCU, '') AS PDMCU,\r\n        COALESCE(PDAN8, 0) AS PDAN8,\r\n        COALESCE(PCUST.ABALPH, '') AS PDALPH,\r\n\t\tCOALESCE(SDMCU, '') AS SDMCU,\r\n        COALESCE(SDAN8, 0) AS SDAN8,\r\n        COALESCE(SCUST.ABALPH, '') AS SDALPH,\r\n\t\tCOALESCE(WAMCU, '') AS WAMCU,\r\n        COALESCE(WAAN8, 0) AS WAAN8,\r\n        COALESCE(WCUST.ABALPH, '') AS WAALPH,\r\n        COALESCE(CMCFGID, 0) AS CFGID,\r\n\t\tROW_NUMBER() OVER (PARTITION BY DLLOTN, DLZ58SN ORDER BY DLUKID DESC) AS RN\r\n\tFROM\r\n\t\tF580569\r\n\t\tINNER JOIN F4102\r\n\t\tON\tIBMCU = DLMCU AND\r\n\t\t\tIBITM = DLITM\r\n\t\tLEFT OUTER JOIN F4311 LEFT OUTER JOIN F0101 PCUST ON PCUST.ABAN8 = PDAN8\r\n\t\tON  PDKCOO = DLPKCO AND\r\n\t\t\tPDDOCO = DLDOC2 AND\r\n\t\t\tPDDCTO = DLXCTO AND\r\n\t\t\tPDLNID = DLLNIC AND\r\n\t\t\tPDMCU = DLMCU\r\n\t\tLEFT OUTER JOIN F4211 LEFT OUTER JOIN F0101 SCUST ON SCUST.ABAN8 = SDAN8\r\n\t\tON  SDKCOO = DLKCOO AND\r\n\t\t\tSDDOCO = DLDOCO AND\r\n\t\t\tSDDCTO = DLDCTO AND\r\n\t\t\tSDLNID = DLLNID AND\r\n\t\t\tSDMCU = DLMCU\r\n\t\tLEFT OUTER JOIN F4801 LEFT OUTER JOIN F0101 WCUST ON WCUST.ABAN8 = WAAN8\r\n\t\tON\tWADOCO = DLDOC1 AND\r\n\t\t\tWAMCU = DLMCU\r\n\t\tLEFT OUTER JOIN F3201\r\n\t\tON\tCMKCOO = COALESCE(PDKCOO, SDKCOO) AND\r\n\t\t\tCMDOCO = COALESCE(PDDOCO, SDDOCO) AND\r\n\t\t\tCMDCTO = COALESCE(PDDCTO, SDDCTO) AND\r\n\t\t\tCMLNID = COALESCE(PDLNID, SDLNID) \r\n\tWHERE\r\n        {0} \r\n\t\tDLIOPR = 'Insert'\r\n\t) A\r\nWHERE\r\n\tA.RN = 1\r\n\r\nUNION ALL\r\n\r\nSELECT \r\n\t*\r\nFROM (\r\n\tSELECT\r\n\t\tDLLOTN AS LOTN,\r\n\t\tDLZ58SN AS Z58SN,\r\n\t\tDLITM AS ITM,\r\n\t\tDLMCU AS MCU,\r\n\t\tDLZ58DST AS Z58DST,\r\n\t\tDLZ58RID AS Z58RID,\r\n\t\tDLZ58MDT AS Z58MDT,\r\n\t\tDLZ58PDT AS Z58PDT,\r\n\t\tDLPQOH AS PQOH,\r\n        DLZ58CQTY AS RQOH,\r\n\t\tDLZ58PS AS Z58PS,\r\n\t\tDLZ58SL AS Z58SL,\r\n\t\tDLZ58STK AS Z58STK,\r\n\t\tDLZ58ROW AS Z58ROW,\r\n\t\tDLZ58SD AS Z58SD,\r\n\t\tDLZ58BXRO AS Z58BXRO,\r\n\t\tDLDOC1 AS DOC1,\r\n\t\tDLPKCO AS PKCO,\r\n\t\tDLDOC2 AS DOC2,\r\n\t\tDLXCTO AS XCTO,\r\n\t\tDLLNIC AS LNIC,\r\n\t\tDLKCOO AS KCOO,\r\n\t\tDLDOCO AS DOCO,\r\n\t\tDLDCTO AS DCTO,\r\n\t\tDLLNID AS LNID,\r\n\t\tDLMUPM AS UPMJ,\r\n\t\tDLUPMT AS TDAY,\r\n\t\tDLUKID AS UKID,\r\n\t\tDLIOPR AS IOPR,\r\n\t\tDLMUPM AS MUPM,\r\n\t\tDLUPMT AS UPMT,\r\n        DLUSER AS USERID,\r\n        DLPID AS PID,\r\n\t\tIBSTKT AS STKT,\r\n\t\tIBGLPT AS GLPT,\r\n\t\tCAST(NULL AS VARCHAR(12))  AS PDMCU,\r\n        CAST(NULL AS DECIMAL(8,0)) AS PDAN8,\r\n        CAST(NULL AS VARCHAR(40)) AS PDALPH,\r\n\t\tCAST(NULL AS VARCHAR(12))  AS SDMCU,\r\n        CAST(NULL AS DECIMAL(8,0)) AS SDAN8,\r\n        CAST(NULL AS VARCHAR(40)) AS SDALPH,\r\n\t\tCAST(NULL AS VARCHAR(12))  AS WAMCU,\r\n        CAST(NULL AS DECIMAL(8,0)) AS WAAN8,\r\n        CAST(NULL AS VARCHAR(40)) AS WAALPH,\r\n        CAST(NULL AS DECIMAL(8,0)) AS CFGID,\r\n\t\tROW_NUMBER() OVER (PARTITION BY DLLOTN, DLZ58SN ORDER BY DLUKID DESC) AS RN\r\n\tFROM\r\n\t\tF580569\r\n\t\tINNER JOIN F4102\r\n\t\tON\tIBMCU = DLMCU AND\r\n\t\t\tIBITM = DLITM\r\n\tWHERE\r\n        {1} \r\n\t\tDLIOPR <> 'Insert'\r\n\t) A\r\nWHERE\r\n\tA.RN = 1\r\nORDER BY \r\n\tUKID\r\nFOR READ ONLY", (object) stringBuilder, (object) stringBuilder);
      return iDb2Command;
    }
  }
}
