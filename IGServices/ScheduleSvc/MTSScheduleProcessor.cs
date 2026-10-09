using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace IGServices.ScheduleSvc
{
    public class MTSScheduleProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadMTSSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var parallel = Data.UnboxParm.To(parms, "Parallel", false);
            var serviceContext = Data.UnboxParm.To<ServiceContext>(parms, "ServiceContext", null);
            var response = MTSScheduleDownloadManager.DownloadMTSIncomingOrders(session, sqlCns, plant, serviceContext, parallel);
            response.Close();
            return response;
        }

        public static MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            MTSScheduleEvents mtsScheduleEvents;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                ProcessResponse childResponse;
                mtsScheduleEvents = MTSScheduleEventsManager.GetUnprocessedMTSScheduleEvents(e1Session, plant, out childResponse);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: Plant: {plantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"Plant: {plantCode} {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return mtsScheduleEvents;
        }

        public static ProcessResponse CancelMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Cancel MTS Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                response = MTSScheduleEventsManager.CancelMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Reset MTS Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                response = MTSScheduleEventsManager.ResetMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static ProcessResponse SuccessMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Success MTS Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                response = MTSScheduleEventsManager.SuccessMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static ProcessResponse CompleteMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Success MTS Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                response = MTSScheduleEventsManager.CompleteMTSSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadScheduleInfoMTSTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Download Schedule Info MTS Table", true);
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSFInt = GetSFIntSqlConnection(plant.PlantCode);
                response = MTSScheduleDownloadManager.Process(e1Session, cnSFInt, plant, scheduleNum, scheduleDateE1, batchNum, force, null, false);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                NotifyMTSScheduleDownload(response, scheduleNum);
            }
            finally
            {
                e1Session.Close();
                cnSFInt.Close();
                Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static void NotifyMTSScheduleDownload(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Downloads - MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void NotifyMTSScheduleCancel(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Cancel - MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void NotifyMTSScheduleReset(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Reset - MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void NotifyMTSScheduleSuccess(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Success - MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public static void NotifyMTSScheduleComplete(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTSScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");

                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Complete - MTS Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $@"Error - {subject}";

                //var plantPathName = $@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}";
                //Directory.CreateDirectory(plantPathName);
                //var fileName = $@"{plantPathName}\{subject} {DateTime.Now:hhmmss}.htm";
                //File.Delete(fileName);
                //File.WriteAllText(fileName, xslOutput);

                Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        private static SqlConnection GetSFIntSqlConnection(string plantCode)
        {
            var cnSFInt = new SqlConnection();
            if (plantCode != "0")
            {
                try
                {
                    cnSFInt = ConnectionManager.GetSqlConnectionFromKey($@"{plantCode}.INTEGRATION", false);
                    cnSFInt.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{cnSFInt.ConnectionString }  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSFInt;
        }
    }
}
