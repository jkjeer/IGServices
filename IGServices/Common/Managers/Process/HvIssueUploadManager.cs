using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class HvIssueUploadManager
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvIssueUploadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"GetF580200Single", GetF580200Single()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, SqlConnection cnInt, Plant plant, string status)
        {
            var response = new ProcessResponse("HV Issue Upload Manager");
            var sfManager = new HvIssueItemManager(session);
            var f580030Mgr = new F580030Manager(session);
            var f580030AMgr = new F580030AManager(session);
            try
            {
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("INT", cnInt.ConnectionString));

                // Get Shop Floor Records
                //response.SqlDmlResponses.Add(sfManager.SetTransferStatus(cn, "N", "Q"));
                HvIssueItems items = sfManager.Extract(session, cn, plant, "N");
                response.Messages.Add(new Message("SF HV Issue Records", items.Count.ToString()));

                if (items.Count > 0)
                {
                    // Validate
                    Validate(session, plant, items, f580030Mgr);

                    // Get E1 Required Sequences
                    var sfukid = SequenceManager.GetRange(cnInt, "E1.SFUKID", items.Count);
                    var sfedbt = SequenceManager.GetNextVal(cnInt, "E1.SFEDBT");
                    var qaukid = SequenceManager.GetNextVal(cnInt, "E1.QAUKID");
                    foreach (var item in items)
                    {
                        item.TransferId = sfukid;
                        item.TransferBatchId = sfedbt;
                        sfukid++;
                    }
                    response.SerializedObjects.Add(new SerializedObject("HvIssueItems",
                        XmlSerializationUtility.SerializeObject(Encoding.UTF8, items)));

                    // Build E1 Interface Records
                    F580030s f580030s = sfManager.Transform(items);
                    response.Messages.Add(new Message("E1 F580030 Records", f580030s.Count.ToString()));
                    response.SerializedObjects.Add(new SerializedObject("F580030s",
                        XmlSerializationUtility.SerializeObject(Encoding.UTF8, f580030s)));

                    // Save to E1
                    var e1Trans = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        response.SqlDmlResponses.AddRange(f580030Mgr.Save(session, e1Trans, f580030s));
                        e1Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        e1Trans.Rollback();
                        throw new ApplicationException(e.Message);
                    }

                    // Submit E1 processing record
                    var f580030A = f580030AMgr.Create("WC", qaukid, sfedbt, plant.PlantNum);
                    var e1Trans1 = session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                    try
                    {
                        response.SqlDmlResponses.Add(f580030AMgr.Save(session, e1Trans1, f580030A));
                        e1Trans1.Commit();
                    }
                    catch (Exception e)
                    {
                        e1Trans1.Rollback();
                        throw new ApplicationException(e.Message);
                    }

                    // Update Shop Floor Completed
                    response.SqlDmlResponses.AddRange(sfManager.SaveTransferProperties(items, 100, cn));

                    // Send Mail
                    XElement root = new XElement("root", XElement.Load(new StringReader(XmlSerializationUtility.SerializeObject(Encoding.UTF8, response))));
                    foreach (var serializedObject in response.SerializedObjects)
                    {
                        root.Add(XElement.Load(new StringReader(serializedObject.Value)));
                    }
                    var serializedXml = root.ToString();
                    Logger.Info("Serialized Process: " + serializedXml);
                    var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.HvIssueUploadReport.xslt", Encoding.UTF8);
                    Logger.Info("Report XML: " + xslOutput);
                    Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        System.Configuration.ConfigurationManager.AppSettings["MailBizToAddress"],
                        null,
                        plant.PlantCode + " [" + System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"] + "] " +
                        "HV Issue Upload ",
                        xslOutput);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return response;
        }

        public void Validate(BusinessSystemSession session, Plant plant, HvIssueItems items, F580030Manager mgr)
        {
            foreach (var item in items)
            {
                if (mgr.Exists(session, plant.PlantNum, item.HVIssuesID.ToString(), "HI"))
                {
                    //item.Transferred = true;
                    //item.TransferStatus = "E";
                    //item.TransferMessage = "Previously Uploaded to F580030";
                }
            }
        }
    }
}
