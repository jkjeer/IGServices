// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.ScheduleStatusUploadManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Enum;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.Process
{
  public class ScheduleStatusUploadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static ProcessResponse Process(
      BusinessSystemSession session,
      Dictionary<string, SqlConnection> sqlCns,
      Plant plant,
      ScheduleEvent evt,
      Schedule.StatusUpdateEvent updateEvent)
    {
      ProcessResponse processResponse = new ProcessResponse(string.Format("Set {0} Schedule {1}", (object) updateEvent.ToString(), (object) evt.ScheduleNum), true, (string) null);
      try
      {
        processResponse.Plant = plant;
        processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
        foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
          processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
        ProcessStep step1 = new ProcessStep("Update E1 F580020 Status");
        try
        {
          SqlDmlResponse sqlDmlResponse = new SqlDmlResponse();
          switch (updateEvent)
          {
            case Schedule.StatusUpdateEvent.Reset:
              sqlDmlResponse = ScheduleEventManager.Reset(session, evt);
              break;
            case Schedule.StatusUpdateEvent.Cancel:
              sqlDmlResponse = ScheduleEventManager.Cancel(session, evt);
              break;
            case Schedule.StatusUpdateEvent.Success:
              sqlDmlResponse = ScheduleEventManager.Success(session, evt);
              break;
            case Schedule.StatusUpdateEvent.Complete:
              sqlDmlResponse = ScheduleEventManager.Complete(session, evt);
              break;
          }
          step1.SqlDmlResponses.Add(sqlDmlResponse);
          step1.RowsAffected = sqlDmlResponse.RowsAffected;
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
        ProcessStep step2 = new ProcessStep("Update ShopFloor Event Transfer Status");
        try
        {
          if (processResponse.HasErrors())
          {
            SqlDmlResponse sqlDmlResponse = ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, updateEvent.ToString(), "FAILURE");
            step2.RowsAffected = sqlDmlResponse.RowsAffected;
            step2.SqlDmlResponses.Add(sqlDmlResponse);
          }
          else
          {
            SqlDmlResponse sqlDmlResponse = ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, updateEvent.ToString(), "SUCCESS");
            step2.RowsAffected = sqlDmlResponse.RowsAffected;
            step2.SqlDmlResponses.Add(sqlDmlResponse);
          }
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
        ProcessStep step3 = new ProcessStep("Summary Collections");
        try
        {
          string plantCode = plant.PlantCode;
          Decimal scheduleNum = evt.ScheduleNum;
          int scheduleId = evt.ScheduleId;
          DateTime dateTime = E1TimeManager.CastToDateTime(evt.ScheduleDateE1, Decimal.Zero);
          string str1 = dateTime.ToString("MM/dd/yyyy");
          string batchNum = evt.BatchNum;
          dateTime = processResponse.ProcessSteps.Min<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.StartDate));
          string str2 = dateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
          dateTime = processResponse.ProcessSteps.Max<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.EndDate));
          string str3 = dateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
          Decimal totalSeconds = (Decimal) (processResponse.ProcessSteps.Max<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.EndDate)) - processResponse.ProcessSteps.Min<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.StartDate))).TotalSeconds;
          var data = new
          {
            Plant = plantCode,
            ScheduleNum = scheduleNum,
            ScheduleId = scheduleId,
            ScheduleDate = str1,
            BatchNum = batchNum,
            StartDate = str2,
            EndDate = str3,
            ElapsedSeconds = totalSeconds
          };
          processResponse.SerializedObjects.Add("ScheduleHeader", (object) data, (string) null);
        }
        catch (Exception ex)
        {
          ScheduleStatusUploadManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
          step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          step3.Done();
          processResponse.AddStep(step3);
        }
        ProcessStep step4 = new ProcessStep("Send Mail");
        try
        {
          string xml = processResponse.Serialize();
          ScheduleStatusUploadManager.Logger.Info("Serialized Process: " + xml);
          string body = XsltManager.Transform(xml, "IGServices.Common.Xslt.ScheduleStatusUploadReport.xslt", Encoding.UTF8);
          ScheduleStatusUploadManager.Logger.Info("Report XML: " + body);
          List<string> to = new List<string>();
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_SCHEDULE_STATUS", "ADMIN", (string) null, (string) null));
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_SCHEDULE_STATUS", "PUBLIC", evt.Plant.PlantCode, (string) null));
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "LG_SCHEDULE_STATUS", "USER", evt.Plant.PlantCode, (string) null));
          Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], to, (string) null, string.Format("{0}{1} [{2}] Schedule {3} {4}", processResponse.HasErrors() ? (object) "ERROR - " : (object) string.Empty, (object) evt.Plant.PlantCode, (object) ConfigurationManager.AppSettings["PrimaryEnvironment"], (object) evt.ScheduleNum, (object) updateEvent.ToString()), body);
        }
        catch (Exception ex)
        {
          step4.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          step4.Done();
          processResponse.AddStep(step4);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      processResponse.Close();
      return processResponse;
    }
  }
}
