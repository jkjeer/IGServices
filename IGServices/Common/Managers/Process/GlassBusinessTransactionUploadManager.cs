// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.GlassBusinessTransactionUploadManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IGServices.Common.Managers.Process
{
  public class GlassBusinessTransactionUploadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public GlassBusinessTransactionUploadManager(BusinessSystemSession e1Session)
    {
      Dictionary<string, iDB2Command> dictionary = new Dictionary<string, iDB2Command>();
      foreach (string key in dictionary.Keys.Where<string>((Func<string, bool>) (keyName => !e1Session.DbCommands.ContainsKey(keyName))))
        e1Session.DbCommands.Add(key, dictionary[key]);
    }

    public ProcessResponse Process(
      BusinessSystemSession session,
      Dictionary<string, SqlConnection> sqlCns,
      Plant plant,
      string status)
    {
      ProcessResponse processResponse = new ProcessResponse("IG Glass Transaction Upload Manager", plant, false);
      processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
      foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
        processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
      GlassBusinessTransactionManager transactionManager = new GlassBusinessTransactionManager(session);
      F580030Manager f580030Manager = new F580030Manager(session);
      F580030AManager f580030Amanager = new F580030AManager(session);
      try
      {
        ProcessStep step1 = new ProcessStep("SF Set Transfer Status");
        try
        {
          processResponse.SqlDmlResponses.Add(transactionManager.SetTransferStatus(sqlCns["PLANT.SHOPFLOOR"], "N", "R"));
          step1.RowsAffected = processResponse.SqlDmlResponses.Last<SqlDmlResponse>().RowsAffected;
          processResponse.RowsAffected = step1.RowsAffected;
        }
        catch (Exception ex)
        {
          step1.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
        }
        step1.Done();
        processResponse.AddStep(step1);
        ProcessStep step2 = new ProcessStep("Get Shop Floor Records");
        GlassBusinessTransactions businessTransactions = new GlassBusinessTransactions();
        List<GlassBusinessTransaction> businessTransactionList = new List<GlassBusinessTransaction>();
        try
        {
          businessTransactions = transactionManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, "R");
          step2.RowsAffected = businessTransactions.Count;
          processResponse.RowsAffected = businessTransactions.Count;
          processResponse.Messages.Add(new Message("IG Glass Transactions", businessTransactions.Count.ToString()));
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
        if (businessTransactions.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Validate");
          try
          {
            foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) businessTransactions)
            {
              if (f580030Manager.Exists(session, plant.PlantNum, businessTransaction.GlassBusinessTransactionId.ToString(), businessTransaction.TransactionType))
              {
                businessTransaction.Transferred = true;
                businessTransaction.TransferStatus = "E";
                businessTransaction.TransferMessage = "Previously Uploaded to F580030";
                step3.Messages.Add(new Message("Validation", "Transaction Id:" + businessTransaction.GlassBusinessTransactionId.ToString() + " Previously Uploaded to F580030", Message.SeverityType.Warning));
              }
            }
            businessTransactionList = businessTransactions.Where<GlassBusinessTransaction>((Func<GlassBusinessTransaction, bool>) (x => x.TransferStatus == "E")).ToList<GlassBusinessTransaction>();
            businessTransactions.RemoveAll((Predicate<GlassBusinessTransaction>) (x => x.TransferStatus == "E"));
            processResponse.SerializedObjects.Add<List<Message>>("Messages", step3.Messages.Warnings());
            processResponse.SerializedObjects.Add<List<GlassBusinessTransaction>>("ValidationItems", businessTransactionList);
            step3.RowsAffected = businessTransactionList.Count;
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        long num1 = 0;
        long num2 = 0;
        if (businessTransactions.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Assign Ids");
          try
          {
            long range = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", businessTransactions.Count);
            num1 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
            num2 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
            foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) businessTransactions)
            {
              businessTransaction.TransferId = (Decimal) range;
              businessTransaction.TransferBatchNum = num1;
              ++range;
            }
            step3.RowsAffected = businessTransactions.Count;
            processResponse.SerializedObjects.Add<GlassBusinessTransactions>("GlassBusinessTransactions", businessTransactions);
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        F580030s recs = new F580030s();
        if (businessTransactions.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Build Objects");
          try
          {
            recs = transactionManager.Transform(businessTransactions);
            step3.RowsAffected = recs.Count;
            processResponse.RowsAffected = recs.Count;
            processResponse.Messages.Add(new Message("E1 F580030 Records", recs.Count.ToString()));
            processResponse.SerializedObjects.Add<F580030s>("F580030s", recs);
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        if (businessTransactions.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Save F580030 Records to E1");
          iDB2Transaction txn = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            step3.SqlDmlResponses.AddRange((IEnumerable<SqlDmlResponse>) f580030Manager.Save(session, txn, recs));
            txn.Commit();
            step3.RowsAffected = recs.Count;
            processResponse.RowsAffected = recs.Count;
          }
          catch (Exception ex)
          {
            txn.Rollback();
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
            foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) businessTransactions)
            {
              businessTransaction.TransferStatus = "E";
              businessTransaction.TransferMessage = "F580030 Batch Failure";
            }
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        if (businessTransactions.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
          iDB2Transaction e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            F580030A record = f580030Amanager.Create("WC", (Decimal) num2, (Decimal) num1, plant.PlantNum);
            step3.SqlDmlResponses.Add(f580030Amanager.Save(session, e1Trans, record));
            e1Trans.Commit();
            step3.RowsAffected = 1;
          }
          catch (Exception ex)
          {
            e1Trans.Rollback();
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
            foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) businessTransactions)
            {
              businessTransaction.TransferStatus = "E";
              businessTransaction.TransferMessage = "F580030A Batch Failure";
            }
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        if ((businessTransactions.Count > 0 || businessTransactionList.Count > 0) && !processResponse.HasErrors())
        {
          ProcessStep step3 = new ProcessStep("Set Shop Floor Status");
          try
          {
            foreach (GlassBusinessTransaction businessTransaction in (List<GlassBusinessTransaction>) businessTransactions)
              businessTransaction.TransferStatus = "Y";
            GlassBusinessTransactions items = new GlassBusinessTransactions();
            items.AddRange((IEnumerable<GlassBusinessTransaction>) businessTransactionList);
            items.AddRange((IEnumerable<GlassBusinessTransaction>) businessTransactions);
            SqlDmlResponses sqlDmlResponses = transactionManager.SaveTransferProperties(items, (short) 100, sqlCns["PLANT.SHOPFLOOR"]);
            step3.RowsAffected = sqlDmlResponses[0].RowsAffected;
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        if (businessTransactions.Count > 0 || businessTransactionList.Count > 0)
        {
          ProcessStep step3 = new ProcessStep("Batch Summary Collection");
          try
          {
            var data = new
            {
              Plant = plant.PlantCode,
              BatchNum = num1,
              Transactions = processResponse.RowsAffected,
              StartDate = processResponse.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
              EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
              ElapsedSeconds = (DateTime.Now - processResponse.StartDate).TotalSeconds
            };
            processResponse.SerializedObjects.Add("Header", (object) data, (string) null);
            step3.RowsAffected = 1;
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
        if (businessTransactions.Count > 0 || businessTransactionList.Count > 0)
        {
          ProcessStep step3 = new ProcessStep("Send Mail");
          try
          {
            string xml = processResponse.Serialize();
            GlassBusinessTransactionUploadManager.Logger.Info("Serialized Process: " + xml);
            string body = XsltManager.Transform(xml, "IGServices.Common.Xslt.GlassBusinessTransactionUploadReport.xslt", Encoding.UTF8);
            GlassBusinessTransactionUploadManager.Logger.Info("Report XML: " + body);

            var subject = string.Format("{0}{1} [{2}] Glass Tran Upload Batch [{3}]", processResponse.HasErrors() ? (object)"ERROR - " : (object)string.Empty, (object)plant.PlantCode, (object)ConfigurationManager.AppSettings["PrimaryEnvironment"], (object)num1);
            Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{processResponse.Plant.PlantCode}", subject, body);

            List<string> to = new List<string>();
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_PRODUCTION", "ADMIN", (string) null, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_PRODUCTION", "PUBLIC", plant.PlantCode, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_PRODUCTION", "USER", plant.PlantCode, (string) null));
            Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], to, (string) null, string.Format("{0}{1} [{2}] Glass Tran Upload Batch [{3}]", processResponse.HasErrors() ? (object) "ERROR - " : (object) string.Empty, (object) plant.PlantCode, (object) ConfigurationManager.AppSettings["PrimaryEnvironment"], (object) num1), body);
          }
          catch (Exception ex)
          {
            step3.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step3.Done();
            processResponse.AddStep(step3);
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      processResponse.Done();
      return processResponse;
    }
  }
}
