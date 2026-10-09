// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.StockRackDownloadManager
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
  public class StockRackDownloadManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public StockRackDownloadManager(BusinessSystemSession e1Session)
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
      bool highWaterMark,
      Decimal scheduleNum,
      List<Decimal> workOrders)
    {
      ProcessResponse processResponse = new ProcessResponse("IG Stock Rack Request Manager", plant, false);
      processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
      foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
        processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
      StockRackManager stockRackManager = new StockRackManager(session);
      F580022Manager f580022Manager = new F580022Manager(session);
      F580030AManager f580030Amanager = new F580030AManager(session);
      try
      {
        ProcessStep step1 = new ProcessStep("Get Shop Floor Records");
        StockRackItems stockRackItems = new StockRackItems();
        List<StockRackItem> stockRackItemList = new List<StockRackItem>();
        try
        {
          stockRackItems = stockRackManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, scheduleNum, workOrders);
          step1.RowsAffected = stockRackItems.Count;
          processResponse.Messages.Add(new Message("IG Stock Orders", stockRackItems.Count.ToString()));
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
        if (stockRackItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Validate");
          try
          {
            foreach (StockRackItem stockRackItem in (List<StockRackItem>) stockRackItems)
              ;
            stockRackItemList = stockRackItems.Where<StockRackItem>((Func<StockRackItem, bool>) (x => x.TransferStatus == "E")).ToList<StockRackItem>();
            stockRackItems.RemoveAll((Predicate<StockRackItem>) (x => x.TransferStatus == "E"));
            processResponse.SerializedObjects.Add<List<StockRackItem>>("ValidationItems", stockRackItemList);
            step2.RowsAffected = stockRackItemList.Count;
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
        long num1 = 0;
        long num2 = 0;
        if (stockRackItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Assign Ids");
          try
          {
            num1 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
            num2 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
            foreach (StockRackItem stockRackItem in (List<StockRackItem>) stockRackItems)
              stockRackItem.TransferBatchNum = num1;
            step2.RowsAffected = stockRackItems.Count;
            processResponse.SerializedObjects.Add<StockRackItems>("StockRackItems", stockRackItems);
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
        F580022s recs = new F580022s();
        if (stockRackItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Build Objects");
          try
          {
            recs = stockRackManager.Transform(stockRackItems);
            step2.RowsAffected = recs.Count;
            processResponse.RowsAffected = recs.Count;
            processResponse.Messages.Add(new Message("E1 F580022 Records", recs.Count.ToString()));
            processResponse.SerializedObjects.Add<F580022s>("F580022s", recs);
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
        if (stockRackItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Save F580022 Records to E1");
          iDB2Transaction txn = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            step2.SqlDmlResponses.AddRange((IEnumerable<SqlDmlResponse>) f580022Manager.Save(session, txn, recs));
            txn.Commit();
            step2.RowsAffected = recs.Count;
            processResponse.RowsAffected = recs.Count;
          }
          catch (Exception ex)
          {
            txn.Rollback();
            step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step2.Done();
            processResponse.AddStep(step2);
          }
        }
        if (stockRackItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
          iDB2Transaction e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            F580030A record = f580030Amanager.Create("CT", (Decimal) num2, (Decimal) num1, plant.PlantNum);
            step2.SqlDmlResponses.Add(f580030Amanager.Save(session, e1Trans, record));
            e1Trans.Commit();
            step2.RowsAffected = 1;
          }
          catch (Exception ex)
          {
            e1Trans.Rollback();
            step2.Errors.Add(new IGServices.Common.Types.Response.Error(ex, true));
          }
          finally
          {
            step2.Done();
            processResponse.AddStep(step2);
          }
        }
        if (stockRackItems.Count > 0 || stockRackItemList.Count > 0)
        {
          ProcessStep step2 = new ProcessStep("Batch Summary Collection");
          try
          {
            var data = new
            {
              Plant = plant.PlantCode,
              BatchNum = num1,
              Transactions = processResponse.RowsAffected,
              StartDate = processResponse.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
              EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
              ElapsedSeconds = (DateTime.Now - processResponse.StartDate).Seconds
            };
            processResponse.SerializedObjects.Add("Header", (object) data, (string) null);
            step2.RowsAffected = 1;
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
        if (stockRackItems.Count > 0 || stockRackItemList.Count > 0)
        {
          ProcessStep step2 = new ProcessStep("Send Mail");
          try
          {
            string xml = processResponse.Serialize();
            StockRackDownloadManager.Logger.Info("Serialized Process: " + xml);
            string body = XsltManager.Transform(xml, "IGServices.Common.Xslt.StockRackDownloadReport.xslt", Encoding.UTF8);
            StockRackDownloadManager.Logger.Info("Report XML: " + body);
            List<string> to = new List<string>();
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG", "ADMIN", (string) null, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG", "PUBLIC", plant.PlantCode, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG", "USER", plant.PlantCode, (string) null));
            Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], to, (string) null, string.Format("{0}{1} [{2}] Schedule {3} Rack Request ", processResponse.HasErrors() ? (object) "ERROR - " : (object) string.Empty, (object) plant.PlantCode, (object) ConfigurationManager.AppSettings["PrimaryEnvironment"], (object) scheduleNum), body);
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
