using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.BoxingSvc
{
    public class BoxingProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static BoxingEvent GetBoxingEvent(string plantCode, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            BoxingEvent boxingEvent;
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                cnSF = GetSFSqlConnection(plant.PlantCode);
                boxingEvent = BoxingScheduleManager.GetBoxingEvent(cnSF, plant, scheduleNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                cnSF.Close();
                Logger.Info($"PLANT: {plantCode} SCHEDULE: {scheduleNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return boxingEvent;
        }

        public static BoxingEvents GetUnprocessedBoxingEvents(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            BoxingEvents boxingEvents;
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                cnSF = GetSFSqlConnection(plant.PlantCode);
                boxingEvents = BoxingScheduleManager.GetUnprocessedBoxingEvents(cnSF, plant);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                cnSF.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return boxingEvents;
        }

        public static BoxingSchedule GetBoxingSchedule(string plantCode, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            BoxingSchedule boxingSchedule;
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = GetSFSqlConnection(plant.PlantCode);
                boxingSchedule = BoxingScheduleManager.GetBoxingSchedule(e1Session, cnSF, plant, scheduleNum);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                cnSF.Close();
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return boxingSchedule;
        }

        public static ProcessResponse UploadBoxingSchedule(string plantCode, decimal scheduleNum, bool force)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Upload Boxing Schedule", true);
            var e1Session = new BusinessSystemSession();
            var cnSF = new SqlConnection();
            var cnCorpInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                cnSF = GetSFSqlConnection(plant.PlantCode);
                cnCorpInt = GetCorpIntSqlConnection();
                response = BoxingScheduleUploadManager.Process(e1Session, cnSF, cnCorpInt, plant, scheduleNum, force);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}");
                response.Errors.Add(new Error($"ERROR: {cnSF.ConnectionString}  {e.Message}"));
                response.Close();
                SummarizeBoxingSchedule(response, scheduleNum, force);
                NotifyBoxingSchedule(response, scheduleNum, force);
            }
            finally
            {
                cnSF.Close();
                cnCorpInt.Close();
                e1Session.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void SummarizeBoxingSchedule(ProcessResponse response, decimal scheduleNum, bool force)
        {
            var stepSummarizeUpload = new ProcessStep("Schedule Summary Collections");
            try
            {
                var scheduleHeaderSummary = new
                {
                    Location = response.Plant.PlantCode + " - " + response.Plant.PlantNum,
                    ScheduleNum = scheduleNum,
                    TransferStatus = "E",
                    Force = force,
                    HasErrors = true,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    response.EndDate,
                    EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("ScheduleSummaryNode", XmlSerializationUtility.SerializeAnonymousType(scheduleHeaderSummary, "ScheduleHeader")));

                var actionLinks = new ActionLinks();
                actionLinks.Add(new ActionLink("Force", "Force Upload ... Upload with Validation Override", $"http://{Dns.GetHostName()}/IGServices/IGServices.BoxingRestService.svc/UploadBoxingSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={scheduleNum}&Force=true"));
                actionLinks.Add(new ActionLink("Backout", "Backout Schedule ... Initiate E1 Backout Process ... F580030 BX", $"http://{Dns.GetHostName()}/IGServices/IGServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={scheduleNum}"));
                response.SerializedObjects.Add("ActionLinks", actionLinks);
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
        }

        public static void NotifyBoxingSchedule(ProcessResponse response, decimal scheduleNum, bool force)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.BoxingScheduleUploadReport.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");

                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Boxing Uploads - MTO Schedule {scheduleNum}";
                if (force) subject = $@"Force - {subject}";
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
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        private static SqlConnection GetCorpIntSqlConnection()
        {
            var cnCorpInt = new SqlConnection();
            try
            {
                cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION", false);
                cnCorpInt.Open();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return cnCorpInt;
        }

        private static SqlConnection GetSFSqlConnection(string plantCode)
        {
            var cnSF = new SqlConnection();
            if (plantCode != "0")
            {
                try
                {
                    cnSF = ConnectionManager.GetSqlConnectionFromKey($"{plantCode}.SHOPFLOOR", false);
                    cnSF.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSF;
        }
    }
}
