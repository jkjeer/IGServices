using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class CurrentTagsDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal rackId, string serialNum)
        {
            var response = new ProcessResponse("Current Tags Download Manager");
            var table = new CurrentTagsTable();
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "CurrentTag") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {

                /********************************************************/
                var step2a = new ProcessStep("Reserve");
                try
                {
                    var dmlResponse = F580011Manager.UpdateStatus(session, plant, "N", "R");
                    step2a.RowsAffected = dmlResponse.RowsAffected;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                step2a.Done();
                response.AddStep(step2a);

                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = F580011Manager.GetCurrentTags(session, processRunSpan, plant.PlantNum, rackId, serialNum);
                    table.Add(reader, plant);
                    Logger.Trace($@"ROWS: {table.Rows.Count}");
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (response.RowsAffected > 0 && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Update EDSP");
                    try
                    {
                        var dmlResponse = F580011Manager.UpdateStatus(session, plant, "R", "Y");
                        step4.RowsAffected = dmlResponse.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }

                if (useHighWaterMark && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate;
                            highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }

                /********************************************************/
                // Summary
                var stepSummarizeUpload = new ProcessStep("Summary Collections");
                try
                {
                    var scheduleHeaderSummary = new
                    {
                        Location = response.Plant.PlantCode + " - " + response.Plant.PlantNum,
                        TransferStatus = "E",
                        HasErrors = true,
                        response.StartDate,
                        StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        response.EndDate,
                        EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds
                    };
                    response.SerializedObjects.Add(new SerializedObject("ScheduleSummaryNode",
                        XmlSerializationUtility.SerializeAnonymousType(scheduleHeaderSummary, "ScheduleHeader")));
                }
                catch (Exception e)
                {
                    stepSummarizeUpload.Errors.Add(new Error(e));
                }
                finally
                {
                    stepSummarizeUpload.Done();
                    response.ProcessSteps.Add(stepSummarizeUpload);
                }

                /********************************************************/
                // Send Mail
                Logger.Info("Serialized Process: " + XmlSerializationUtility.SerializeObject(Encoding.UTF8, response));
                if (table.Rows.Count > 0 || response.HasErrors() || response.SerializedObjects.Exists(x => x.Name == "ValidationErrors"))
                {
                    var step12 = new ProcessStep("Send Mail");
                    try
                    {
                        var serializedXml = response.Serialize();
                        //Logger.Info("Serialized Process: " + serializedXml);
                        var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.CurrentTagsDownloadReport.xslt", Encoding.UTF8);
                        //Logger.Info("Report XML: " + xslOutput);

                        var subject = $"{(response.HasErrors() ? "ERROR - " : string.Empty)}{plant.PlantCode} [{System.Configuration.ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Current Tags Download";
                        Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                        var recipients = new List<string>();
                        recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_CURRENT_TAGS", "ADMIN", null, null));
                        //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_CURRENT_TAGS", "PUBLIC", plant.PlantCode, null));
                        //recipients.AddRange(Mail.GetRecipients(sqlCns["CORP.INTEGRATION"], "IG_CURRENT_TAGS", "USER", plant.PlantCode, null));

                        //Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        //    System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        //    recipients,
                        //    null,
                        //    subject,
                        //    xslOutput);
                    }
                    catch (Exception e)
                    {
                        step12.Errors.Add(new Error(e));
                    }
                    step12.Done();
                    response.ProcessSteps.Add(step12);
                }
            }
            response.Close();
            return response;
        }

    }
}
