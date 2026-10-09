using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class ScheduleInfoMTSDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnCP, SqlConnection cnSFInt, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force)
        {
            var response = new ProcessResponse($"Download MTO Incoming Orders (Plant: {plant.PlantCode} Schedule: {scheduleNum})", plant, true);
            var scheduleInfoMTS = new ScheduleInfoMTS(plant.PlantNum, scheduleNum);
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnCP", cnCP.ConnectionString));

                var step1GetScheduleInfoMTS = new ProcessStep($@"Step 1. Get Schedule Info MTS ({scheduleNum})");
                try
                {
                    scheduleInfoMTS = MTSScheduleManager.BuildScheduleInfoMTS(e1Session, cnCP, cnSFInt, plant, scheduleNum, scheduleDateE1, batchNum);
                    scheduleInfoMTS.TransferStatus = "N";
                    step1GetScheduleInfoMTS.RowsAffected = scheduleInfoMTS.ScheduleInfoMTSLines.Count;
                }
                catch (Exception e)
                {
                    step1GetScheduleInfoMTS.Errors.Add(new Error(e));
                }
                step1GetScheduleInfoMTS.Done();
                response.ProcessSteps.Add(step1GetScheduleInfoMTS);

                if (scheduleInfoMTS.ScheduleInfoMTSLines.Any() && !step1GetScheduleInfoMTS.HasErrors() && !scheduleInfoMTS.ValidationErrors.Any())
                {
                    var step2UpdateBeforeProcessingF580020 = new ProcessStep("Step 2. Update Before Processing F580020");
                    try
                    {
                        //To do: F580020Manager.UpdateBeforeProcessingF580020(e1Session, plant.PlantNum, scheduleInfoMTS.ScheduleNum, scheduleInfoMTS.ScheduleDateE1);
                        step2UpdateBeforeProcessingF580020.RowsAffected = scheduleInfoMTS.ScheduleInfoMTSLines.Count;
                    }
                    catch (Exception e)
                    {
                        step2UpdateBeforeProcessingF580020.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2UpdateBeforeProcessingF580020.Done();
                        response.ProcessSteps.Add(step2UpdateBeforeProcessingF580020);
                    }

                    if (!step2UpdateBeforeProcessingF580020.HasErrors() && !scheduleInfoMTS.ValidationErrors.Any())
                    {
                        var step3DownloadIncomingOrdersTable = new ProcessStep("Step 3. Download Incoming Orders Table");
                        try
                        {
                            DownloadScheduleInfoMTSTable(cnSFInt, scheduleInfoMTS.ScheduleInfoMTSLines);
                            step3DownloadIncomingOrdersTable.RowsAffected = scheduleInfoMTS.ScheduleInfoMTSLines.Count;
                        }
                        catch (Exception e)
                        {
                            step3DownloadIncomingOrdersTable.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3DownloadIncomingOrdersTable.Done();
                            response.ProcessSteps.Add(step3DownloadIncomingOrdersTable);
                        }

                        if (!step3DownloadIncomingOrdersTable.HasErrors())
                        {
                            var step4UpdateAfterProcessingSuccessF580020 = new ProcessStep("Step 4. Update After Processing Success F580020");
                            try
                            {
                                scheduleInfoMTS.TransferStatus = "Y";
                                scheduleInfoMTS.TransferDate = DateTime.Now.ToLocalTime();
                                F580020Manager.UpdateAfterProcessingSuccessF580020(e1Session, plant.PlantNum, scheduleNum, Utilities.Common.CastToE1Date(scheduleInfoMTS.TransferDate));
                                step4UpdateAfterProcessingSuccessF580020.RowsAffected = 1;
                            }
                            catch (Exception e)
                            {
                                step4UpdateAfterProcessingSuccessF580020.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4UpdateAfterProcessingSuccessF580020.Done();
                                response.ProcessSteps.Add(step4UpdateAfterProcessingSuccessF580020);
                            }
                        } // if (!step4DownloadIncomingOrdersTable.HasErrors())
                    } // if (!step2UpdateBeforeProcessingF580020.HasErrors() && !mtoSchedule.ValidationErrors.Any())
                } // if (!step1GetMTOSchedule.HasErrors() && !mtoSchedule.ValidationErrors.Any())
                else
                {
                    scheduleInfoMTS.TransferStatus = "E";
                    scheduleInfoMTS.TransferMessage = scheduleInfoMTS.ValidationStatus;
                }

                response.Done();
                SummarizeScheduleInfoMTS(response, scheduleInfoMTS);
                SummarizeScheduleInfoMTSDownload(response, scheduleInfoMTS);
                NotifyScheduleInfoMTSDownload(response, scheduleInfoMTS);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return response;
        }

        public static void NotifyScheduleInfoMTSDownload(ProcessResponse response, ScheduleInfoMTS scheduleInfoMTS)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                var toPlantAddress = "";
                if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = response.Plant.PlantCode + ConfigurationManager.AppSettings["PlantEmailFormat"];
                var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.ScheduleInfoMTSDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["Environment"]}] Get MTS Schedule - Schedule {scheduleInfoMTS.ScheduleNum}";
                if (response.HasErrors() || scheduleInfoMTS.ValidationErrors.Any()) subject = "Error - " + subject;
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toAddrs1,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void SummarizeScheduleInfoMTS(ProcessResponse response, ScheduleInfoMTS scheduleInfoMTS)
        {
            var stepSummarize = new ProcessStep("Schedule Summary Collections");
            try
            {
                var scheduleCustomerLines =
                    scheduleInfoMTS.ScheduleInfoMTSLines.GroupBy(x => new
                        {
                            x.Customer,
                            x.Construction,
                            x.Dimensions,
                            x.RackType
                        })
                        .Select(
                            ng =>
                                new
                                {
                                    ng.Key.Customer,
                                    ng.Key.Construction,
                                    ng.Key.Dimensions,
                                    ng.Key.RackType,
                                    ScheduleLines = (ng.Count()),
                                    //SalesOrders = (ng.Select(x => x.SOrderNum > 0).Distinct().Count()),
                                    WorkOrders = (ng.Select(x => x.WOrderNum).Distinct().Count()),
                                    LitesQty = (ng.Sum(x => x.ScheduledQty))
                                })
                        .ToList();
                response.SerializedObjects.Add("ScheduleCustomerLine", scheduleCustomerLines, "ScheduleCustomerLines");

                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    scheduleInfoMTS.ScheduleNum,
                    ScheduleDateE1 = scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.ScheduleDateE1).Min(),
                    scheduleInfoMTS.BatchNum,
                    Customers = Convert.ToInt16(scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.Customer).Distinct().Count()),
                    //SalesOrders = Convert.ToInt16(scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.SOrderNum > 0).Distinct().Count()),
                    Lines = Convert.ToInt16(scheduleInfoMTS.ScheduleInfoMTSLines.Count),
                    WorkOrders = Convert.ToInt16(scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Lites = Convert.ToInt32(scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.ScheduledQty).Sum()),
                    //DueDate = scheduleInfoMTS.ScheduleInfoMTSLines.Select(x => x.ScheduleDate).Min()),
                    HasErrors = response.HasErrors() || scheduleInfoMTS.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                stepSummarize.RowsAffected = scheduleInfoMTS.ScheduleInfoMTSLines.Count;
            }
            catch (Exception e)
            {
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        public static void SummarizeScheduleInfoMTSDownload(ProcessResponse response, ScheduleInfoMTS scheduleInfoMTS)
        {
            var stepSummarizeDownload = new ProcessStep("Download Summary Collections");
            try
            {
                var transferSummary = new
                {
                    scheduleInfoMTS.TransferStatus,
                    scheduleInfoMTS.TransferDate,
                    HasErrors = response.HasErrors() || scheduleInfoMTS.ValidationErrors.Any(),
                    scheduleInfoMTS.TransferMessage,
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("TransferNode", XmlSerializationUtility.SerializeAnonymousType(transferSummary, "Transfer")));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarizeDownload.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeDownload.Done();
                response.ProcessSteps.Add(stepSummarizeDownload);
            }
        }

        public static long DownloadScheduleInfoMTSTable(SqlConnection cnSfInt, ScheduleInfoMTSLines scheduleInfoMTSLines)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            long rowCount = 0;
            try
            {
                var scheduleInfoMTSTable = new ScheduleInfoMTSTable();
                scheduleInfoMTSTable.Add(scheduleInfoMTSLines);
                if (scheduleInfoMTSTable.Rows.Count > 0)
                {
                    var bulkResponse = BulkCopyManager.BulkTable(scheduleInfoMTSTable.ExtendedProperties["Target"].ToString(), scheduleInfoMTSTable, cnSfInt);
                    rowCount = bulkResponse.RowsCopied;
                }
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnSfInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnSfInt.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }
    }
}
