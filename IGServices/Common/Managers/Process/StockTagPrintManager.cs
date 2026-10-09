// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Process.StockTagPrintManager
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
  public class StockTagPrintManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public StockTagPrintManager(BusinessSystemSession e1Session)
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
      Decimal rackId,
      List<Decimal> workOrders)
    {
      ProcessResponse processResponse = new ProcessResponse("IG Stock Tag Print Manager", plant, false);
      processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
      foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
        processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
      StockTagManager stockTagManager = new StockTagManager(session);
      F580030Manager f580030Manager = new F580030Manager(session);
      F580030AManager f580030Amanager = new F580030AManager(session);
      try
      {
        ProcessStep step1 = new ProcessStep("Get Shop Floor Records");
        StockTagItems stockTagItems = new StockTagItems();
        List<StockTagItem> stockTagItemList = new List<StockTagItem>();
        try
        {
          stockTagItems = stockTagManager.Extract(session, sqlCns["PLANT.SHOPFLOOR"], plant, scheduleNum, rackId, workOrders);
          step1.RowsAffected = stockTagItems.Count;
          processResponse.Messages.Add(new Message("IG Stock Orders", stockTagItems.Count.ToString()));
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
        if (stockTagItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Validate");
          try
          {
            foreach (StockTagItem stockTagItem in (List<StockTagItem>) stockTagItems)
              ;
            stockTagItemList = stockTagItems.Where<StockTagItem>((Func<StockTagItem, bool>) (x => x.TransferStatus == "E")).ToList<StockTagItem>();
            stockTagItems.RemoveAll((Predicate<StockTagItem>) (x => x.TransferStatus == "E"));
            processResponse.SerializedObjects.Add<List<StockTagItem>>("ValidationItems", stockTagItemList);
            step2.RowsAffected = stockTagItemList.Count;
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
        if (stockTagItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Assign Ids");
          try
          {
            long range = SequenceManager.GetRange(sqlCns["CORP.INTEGRATION"], "E1.SFUKID", stockTagItems.Count);
            num1 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.SFEDBT");
            num2 = SequenceManager.GetNextVal(sqlCns["CORP.INTEGRATION"], "E1.QAUKID");
            foreach (StockTagItem stockTagItem in (List<StockTagItem>) stockTagItems)
            {
              stockTagItem.TransferId = (Decimal) range;
              stockTagItem.TransferBatchNum = num1;
              ++range;
            }
            step2.RowsAffected = stockTagItems.Count;
            processResponse.SerializedObjects.Add<StockTagItems>("StockTagItems", stockTagItems);
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
        F580030s recs = new F580030s();
        if (stockTagItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Build Objects");
          try
          {
            recs = stockTagManager.Transform(stockTagItems);
            step2.RowsAffected = recs.Count;
            processResponse.RowsAffected = recs.Count;
            processResponse.Messages.Add(new Message("E1 F580030 Records", recs.Count.ToString()));
            processResponse.SerializedObjects.Add<F580030s>("F580030s", recs);
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
        if (stockTagItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Save F580030 Records to E1");
          iDB2Transaction txn = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            step2.SqlDmlResponses.AddRange((IEnumerable<SqlDmlResponse>) f580030Manager.Save(session, txn, recs));
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
        if (stockTagItems.Count > 0 && !processResponse.HasErrors())
        {
          ProcessStep step2 = new ProcessStep("Submit E1 F580030A Trigger Processing Record");
          iDB2Transaction e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
          try
          {
            F580030A record = f580030Amanager.Create("WC", (Decimal) num2, (Decimal) num1, plant.PlantNum);
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
        if (stockTagItems.Count > 0 || stockTagItemList.Count > 0)
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
        if (stockTagItems.Count > 0 || stockTagItemList.Count > 0)
        {
          ProcessStep step2 = new ProcessStep("Send Mail");
          try
          {
            string xml = processResponse.Serialize();
            StockTagPrintManager.Logger.Info("Serialized Process: " + xml);
            string body = XsltManager.Transform(xml, "IGServices.Common.Xslt.StockTagPrintReport.xslt", Encoding.UTF8);
            StockTagPrintManager.Logger.Info("Report XML: " + body);
            List<string> to = new List<string>();
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG_PRINT", "ADMIN", (string) null, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG_PRINT", "PUBLIC", plant.PlantCode, (string) null));
            to.AddRange((IEnumerable<string>) Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_STOCK_TAG_PRINT", "USER", plant.PlantCode, (string) null));
            Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], to, (string) null, string.Format("{0}{1} [{2}] Stock Tag Print Request {3}", processResponse.HasErrors() ? (object) "ERROR - " : (object) string.Empty, (object) plant.PlantCode, (object) ConfigurationManager.AppSettings["PrimaryEnvironment"], (object) rackId), body);
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
