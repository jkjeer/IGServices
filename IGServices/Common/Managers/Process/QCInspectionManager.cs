using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class QCInspectionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(SqlConnection cnSFInt, SqlConnection cnSF, SqlConnection cnCorpInt, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse("Get QC Inspections", plant, true);
            var qcInspections = new QCInspections();
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("cnSFInt", cnSFInt.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnCorpInt", cnCorpInt.ConnectionString));
                var step1GetMountedSchedules = new ProcessStep("Step 1. Get Mounted Schedules");
                var mountedSchedules = new MTOSchedules();
                try
                {
                    mountedSchedules = GetMountedSchedules(cnSF);
                    step1GetMountedSchedules.RowsAffected = mountedSchedules.Count;
                    if (mountedSchedules.Count == 0) response.Messages.Add(new Message("", "No Mounted Schedules found at ShopFloor"));
                }
                catch (Exception e)
                {
                    step1GetMountedSchedules.Errors.Add(new Error(e));
                }
                finally
                {
                    step1GetMountedSchedules.Done();
                    response.ProcessSteps.Add(step1GetMountedSchedules);
                }
                if (!step1GetMountedSchedules.HasErrors() && mountedSchedules.Count > 0)
                {
                    var step2 = new ProcessStep("Step 2. ");
                    try
                    {
                        // To do: look for remakes or E1-indicated QC Inspections.
                        step2.RowsAffected = mountedSchedules.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2.Done();
                        response.ProcessSteps.Add(step2);
                    }
                } // if (!step1GetMountedSchedules.HasErrors() && mountedSchedules.Count > 0)
                //response.Done();
                SummarizeQCInspections(response, qcInspections);
                NotifyQCInspections(response, cnCorpInt);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static MTOSchedules GetMountedSchedules(SqlConnection cnSF)
        {
            var mountedSchedules = new MTOSchedules();
            return mountedSchedules;
        }

        public static void SummarizeQCInspections(ProcessResponse response, QCInspections qcInspections)
        {
            var stepSummarizeProcess = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    ////Spacers = Convert.ToInt16(spacerConsumptions?.Sum(sc => sc.Quantity)),
                    ////Items = Convert.ToInt16(spacerConsumptions?.Count),
                    HasErrors = response.HasErrors(), // || validationErrors.Any(),
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                response.SerializedObjects.Add("QCInspections", qcInspections);
                stepSummarizeProcess.RowsAffected = qcInspections.Count;
                response.RowsAffected = qcInspections.Count;
            }
            catch (Exception e)
            {
                stepSummarizeProcess.Errors.Add(new Error(e));
            }
            stepSummarizeProcess.Done();
            response.ProcessSteps.Add(stepSummarizeProcess);
        }

        public static void NotifyQCInspections(ProcessResponse response, SqlConnection cnCorpInt)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");

                var adminRecipients = new List<string>();
                // Recipients
                adminRecipients.AddRange(Mail.GetRecipients(cnCorpInt, "TOIG_RBA_KANBAN", "PUBLIC", response.Plant.PlantCode));
                adminRecipients.AddRange(Mail.GetRecipients(cnCorpInt, "TOIG_RBA_KANBAN", "ADMIN"));

                //var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                //var toPlantAddress = "";
                //if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = response.Plant.PlantCode + ConfigurationManager.AppSettings["PlantEmailFormat"];
                //var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.QCInspectionsResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] QC Inspections";
                if (response.HasErrors()) subject = $@"Error - {subject}";
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    adminRecipients,
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
    }
}
