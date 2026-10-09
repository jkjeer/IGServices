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
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.ScheduleSvc
{
    public class MTOScheduleProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadMTOSchedule(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            // This is the method called by the MTO_SCHEDULE subscription type
            var parallel = Data.UnboxParm.To(parms, "Parallel", false);
            var serviceContext = Data.UnboxParm.To<ServiceContext>(parms, "ServiceContext", null);
            var response = MTOScheduleDownloadManager.DownloadMTOIncomingOrders(session, sqlCns, plant, serviceContext, parallel);
            //response.Close();
            return response;
        }

        public static MTOScheduleEvents GetUnprocessedMTOScheduleEvents(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            MTOScheduleEvents mtoScheduleEvents;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                ProcessResponse childResponse;
                mtoScheduleEvents = MTOScheduleEventsManager.GetUnprocessedMTOScheduleEvents(e1Session, plant, out childResponse);
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
            return mtoScheduleEvents;
        }

        public static ProcessResponse CancelMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Cancel MTO Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                cnSF.Open();
                response = MTOScheduleEventsManager.CancelMTOSchedule(e1Session, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
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

        public static ProcessResponse BoxedMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Boxed MTO Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                cnSF.Open();
                response = MTOScheduleEventsManager.BoxedMTOScheduleEvent(e1Session, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
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

        public static ProcessResponse ResetMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Reset MTO Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                cnSF.Open();
                response = MTOScheduleEventsManager.ResetMTOSchedule(e1Session, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
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

        public static ProcessResponse SuccessMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Success MTO Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                //cnSF.Open();
                response = MTOScheduleEventsManager.SuccessMTOSchedule(e1Session, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
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

        public static ProcessResponse CompleteMTOScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Complete MTO Schedule Event", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                //cnSF.Open();
                response = MTOScheduleEventsManager.CompleteMTOSchedule(e1Session, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
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

        public static ProcessResponse BackoutMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Backout MTO Schedule", true);
            var e1Session = new BusinessSystemSession();
            var cnInt = new SqlConnection();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();
                // SF Connection
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR", false);
                cnSF.Open();
                response = MTOScheduleEventsManager.BackoutMTOSchedule(e1Session, cnInt, cnSF, plant, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}"));
            }
            finally
            {
                e1Session.Close();
                cnInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static MTOSchedule GetMTOSchedule(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            MTOSchedule mtoSchedule;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                ProcessResponse childResponse;
                mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
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
            return mtoSchedule;
        }

        public static MTOIncomingOrderLines GetMTOIncomingOrderLines(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            MTOSchedule mtoSchedule;
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                ProcessResponse childResponse;
                mtoSchedule = MTOScheduleManager.BuildMTOSchedule(e1Session, plant, scheduleNum, scheduleDateE1, batchNum, out childResponse);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return mtoSchedule.MTOIncomingOrderLines;
        }

        public static ProcessResponse DownloadMTOIncomingOrdersTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, bool force)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Download MTO Incoming Orders Table", true);
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSFInt = GetSFIntSqlConnection(plant.PlantCode);
                response = MTOScheduleDownloadManager.Process(e1Session, cnSFInt, plant, scheduleNum, scheduleDateE1, batchNum, force, null, false);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                NotifyMTOScheduleDownload(response, scheduleNum);
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

        public WorkOrder GetWorkOrder(decimal orderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($@"WORKORDER: {orderNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var e1Session = new BusinessSystemSession();
            WorkOrder order;
            try
            {
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                var manager = new WorkOrderManager(e1Session);
                order = manager.GetWorkOrder(e1Session, orderNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                e1Session.Close();
                Logger.Info($@"WORKORDER: {orderNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return order;
        }

        public static void NotifyMTOScheduleDownload(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Downloads - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleCancel(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Cancel - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleBoxed(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Boxed - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleReset(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Reset - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleSuccess(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Success - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleComplete(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Complete - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        public static void NotifyMTOScheduleBackout(ProcessResponse response, decimal scheduleNum)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Schedule Back Out - MTO Schedule {scheduleNum}";
                if (response.HasErrors()) subject = $"Error - {subject}";

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

        private static SqlConnection GetSFIntSqlConnection(string plantNum)
        {
            var cnSFInt = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnSFInt = ConnectionManager.GetSqlConnectionFromKey($@"{plantNum}.INTEGRATION", false);
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
