// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.ScheduleBackoutUploadManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

//using Apache.NMS.ActiveMQ.Commands;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.Process
{
  public class ScheduleBackoutUploadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static ProcessResponse Process(
      BusinessSystemSession session,
      Dictionary<string, SqlConnection> sqlCns,
      Plant plant,
      ScheduleEvent evt)
    {
      ProcessResponse processResponse = new ProcessResponse(string.Format("Upload Backout Schedule {0}", (object) evt.ScheduleNum), true, (string) null);
      long num1 = 0;
      long num2 = 0;
      long num3 = 0;
      try
      {
        processResponse.Plant = plant;
        processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
        foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
          processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
        ProcessStep processStep1 = new ProcessStep("Lock ShopFloor");
        try
        {
          SqlDmlResponse sqlDmlResponse = ScheduleBackoutEventManager.UpdateStatus(sqlCns["PLANT.SHOPFLOOR"], evt, "U");
          processStep1.SqlDmlResponses.Add(sqlDmlResponse);
          processStep1.RowsAffected = sqlDmlResponse.RowsAffected;
        }
        catch (Exception ex)
        {
          processStep1.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          processStep1.Done();
          processResponse.ProcessSteps.Add(processStep1);
        }
        ProcessStep processStep2 = new ProcessStep("Get E1 Schedule Event");
        try
        {
          ScheduleEvents events = ScheduleEventManager.GetEvents(session, plant, new ProcessRunSpan(), (string) null, evt.ScheduleNum, new List<string>());
          if (events.Any<ScheduleEvent>())
            evt.BatchNum = events[0].BatchNum;
          processStep2.RowsAffected = events.Count;
        }
        catch (Exception ex)
        {
          processStep2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        processStep2.Done();
        processResponse.ProcessSteps.Add(processStep2);
        ProcessStep processStep3 = new ProcessStep("Validate Schedule");
        try
        {
          Messages messages = ScheduleBackoutUploadManager.Validate(session, evt);
          processStep3.Errors.Add(messages.Errors(), false);
          processResponse.Errors.Add(messages.Errors(), true);
          processResponse.Messages.Add(messages.Warnings());
          processResponse.SerializedObjects.Add<Messages>("ValidationMessages", messages);
        }
        catch (Exception ex)
        {
          processStep3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          processStep3.Done();
          processResponse.ProcessSteps.Add(processStep3);
        }
        if (!processResponse.HasErrors())
        {
          ProcessStep processStep4 = new ProcessStep("Set F580020 Status");
          try
          {
            SqlDmlResponse sqlDmlResponse = F580020Manager.UpdateStatus(session, evt, "E", "Cancelled");
            processStep4.SqlDmlResponses.Add(sqlDmlResponse);
          }
          catch (Exception ex)
          {
            processStep4.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          processStep4.Done();
          processResponse.ProcessSteps.Add(processStep4);
          ProcessStep step = new ProcessStep("Get Sequences");
          try
          {
            num1 = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", 1);
            num2 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
            num3 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
            step.RowsAffected = 1;
          }
          catch (Exception ex)
          {
            step.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          step.Done();
          processResponse.AddStep(step);
          ProcessStep processStep5 = new ProcessStep("F580030 BX Transaction");
          iDB2Transaction txn = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            int num4 = 1;
            SqlDmlResponse sqlDmlResponse = new F580030Manager(session).Save(session, txn, new F580030()
            {
              SFUKID = (Decimal) num1,
              SFEDUS = "SVCSOA",
              SFEDBT = num2.ToString(),
              SFEDTN = num4.ToString().PadLeft(10, '0').PadRight(22),
              SFEDLN = new Decimal(1000),
              SFEDCT = "BX",
              SFTYTN = "IGMTO",
              SFEDFT = "IGBX",
              SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now),
              SFDRIN = "1",
              SFEDDL = Decimal.Zero,
              SFEDSP = "",
              SFPNID = "Backout",
              SFTNAC = "01",
              SFMMCU = evt.Plant.PlantNum.PadLeft(12),
              SFACDT = E1TimeManager.CastToE1Date(DateTime.Now),
              SFSTTM = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) DateTime.Now),
              SFDTMS = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (object) DateTime.Now),
              SFZ58SCN = evt.ScheduleNum,
              SFUSER = "SVCSOA",
              SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now),
              SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now)
            });
            txn.Commit();
            processStep5.SqlDmlResponses.Add(sqlDmlResponse);
          }
          catch (Exception ex)
          {
            txn.Rollback();
            processStep5.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            processStep5.Done();
            processResponse.ProcessSteps.Add(processStep5);
          }
          ProcessStep processStep6 = new ProcessStep("F580030A Record");
          try
          {
            F580030AManager f580030Amanager = new F580030AManager(session);
            F580030A record = new F580030A();
            record.QAUKID = (Decimal) num3;
            record.QAEDCT = "BX";
            record.QAZ58SCN = evt.ScheduleNum;
            record.QAMCU = evt.Plant.PlantNum.PadLeft(12, ' ');
            record.QAEDBT = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            record.QACRTU = "SVCSOA";
            record.QACPGM = "SVCSOA";
            record.QAJOBN = "SVCSOA";
            F580030A f580030A1 = record;
            DateTime now = DateTime.Now;
            Decimal e1Date = E1TimeManager.CastToE1Date(now.ToLocalTime());
            f580030A1.QACRTDT = e1Date;
            F580030A f580030A2 = record;
            now = DateTime.Now;
            Decimal e1Time = E1TimeManager.CastToE1Time(now.ToLocalTime());
            f580030A2.QACRTM = e1Time;
            SqlDmlResponse sqlDmlResponse = f580030Amanager.Save(session, (iDB2Transaction) null, record);
            step.SqlDmlResponses.Add(sqlDmlResponse);
          }
          catch (Exception ex)
          {
            processStep6.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            processStep6.Done();
            processResponse.ProcessSteps.Add(processStep6);
          }
        }
        ProcessStep processStep7 = new ProcessStep("Update ShopFloor Event Transfer Status");
        try
        {
          evt.TransferBatchNum = num2.ToString();
          evt.TransferTransactionId = (Decimal) num1;
          if (processResponse.HasErrors())
          {
            evt.TransferStatusCode = "E";
            evt.TransferMessage = processResponse.Errors[0]?.Message;
            SqlDmlResponse sqlDmlResponse = ScheduleBackoutEventManager.SaveTransfer(sqlCns["PLANT.SHOPFLOOR"], evt);
            processStep7.RowsAffected = sqlDmlResponse.RowsAffected;
            processStep7.SqlDmlResponses.Add(sqlDmlResponse);
            ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "BACKOUT", "FAILURE");
          }
          else
          {
            evt.TransferStatusCode = "Y";
            SqlDmlResponse sqlDmlResponse = ScheduleBackoutEventManager.SaveTransfer(sqlCns["PLANT.SHOPFLOOR"], evt);
            processStep7.RowsAffected = sqlDmlResponse.RowsAffected;
            processStep7.SqlDmlResponses.Add(sqlDmlResponse);
            ShopFloorScheduleEventManager.Save(sqlCns["PLANT.INTEGRATION"], evt, "BACKOUT", "SUCCESS");
          }
        }
        catch (Exception ex)
        {
          processStep7.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          processStep7.Done();
          processResponse.ProcessSteps.Add(processStep7);
        }
        ProcessStep processStep8 = new ProcessStep("Summary Collections");
        try
        {
          var data = new
          {
            Location = evt.Plant.PlantCode + " - " + evt.Plant.PlantNum,
            ScheduleNum = evt.ScheduleNum,
            ScheduleDate = evt.ScheduleDate.ToString("MM/dd/yyyy"),
            BatchNum = evt.BatchNum,
            System = evt.ShopFloorSystem,
            StartDateString = processResponse.ProcessSteps.Min<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.StartDate)).ToString("MM/dd/yyyy hh:mm:ss tt"),
            EndDateString = processResponse.ProcessSteps.Max<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.EndDate)).ToString("MM/dd/yyyy hh:mm:ss tt"),
            ElapsedSeconds = (Decimal) (processResponse.ProcessSteps.Max<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.EndDate)) - processResponse.ProcessSteps.Min<ProcessStep, DateTime>((Func<ProcessStep, DateTime>) (x => x.StartDate))).TotalSeconds
          };
          processResponse.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType((object) data, "Header", (string) null)));
        }
        catch (Exception ex)
        {
          ScheduleBackoutUploadManager.Logger.Error("ERROR: " + ex.Message + ": " + ex.StackTrace);
          processStep8.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          processStep8.Done();
          processResponse.ProcessSteps.Add(processStep8);
        }
        processResponse.Done();
        ProcessStep processStep9 = new ProcessStep("Send Mail");
        try
        {
          string xml = processResponse.Serialize();
          ScheduleBackoutUploadManager.Logger.Info("Serialized Process: " + xml);
          string body = XsltManager.Transform(xml, "IGServices.Common.Xslt.ScheduleBackoutReport.xslt", Encoding.UTF8);
          ScheduleBackoutUploadManager.Logger.Info("Report XML: " + body);
          List<string> to = new List<string>();
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_SCHEDULE_BACKOUT", "ADMIN", (string) null, (string) null));
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_SCHEDULE_BACKOUT", "PUBLIC", evt.Plant.PlantCode, (string) null));
          to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_SCHEDULE_BACKOUT", "USER", evt.Plant.PlantCode, (string) null));

          var subject = string.Format("{0}{1} [{2}] Schedule {3} Backout", processResponse.HasErrors() ? (object)"ERROR - " : (object)string.Empty, (object)evt.Plant.PlantCode, (object)ConfigurationManager.AppSettings["PrimaryEnvironment"], (object)evt.ScheduleNum);

                    //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{evt.Plant.PlantCode}";
                    //Directory.CreateDirectory(plantPathName);
                    //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                    //File.Delete(fileName);
                    //File.WriteAllText(fileName, body);

                    Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{evt.Plant.PlantCode}", subject, body);

                    Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], 
              ConfigurationManager.AppSettings["MailFromAddress"], 
              to, 
              (string) null,
              subject,
              body);
//                    string.Format("{0}{1} [{2}] Schedule {3} Backout", processResponse.HasErrors() ? (object)"ERROR - " : (object)string.Empty,
//(object)evt.Plant.PlantCode, (object)ConfigurationManager.AppSettings["PrimaryEnvironment"],
//(object)evt.ScheduleNum),

        }
                catch (Exception ex)
        {
          processStep9.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        finally
        {
          processStep9.Done();
          processResponse.ProcessSteps.Add(processStep9);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      processResponse.Close();
      return processResponse;
    }

    public static Messages Validate(BusinessSystemSession session, ScheduleEvent evt)
    {
      Messages messages = new Messages();
      if (string.IsNullOrEmpty(evt.BatchNum))
        messages.Add("Validation", string.Format("{0} Schedule Num {1} does NOT exist in E1", (object) evt.Plant.PlantCode, (object) evt.ScheduleNum), Message.SeverityType.Error);
      return messages;
    }
  }
}
