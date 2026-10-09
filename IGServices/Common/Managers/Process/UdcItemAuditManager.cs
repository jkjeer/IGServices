using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class UdcItemAuditManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse("Check Duplicate Udc Short Description");
            response.Plant = plant;
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var cnSF = new SqlConnection();
            //var cnCP = new SqlConnection();
            try
            {
                cnSF = GetSFSqlConnection(plant.PlantCode);
                response.ConnectionStrings.Add(new ConnectionString("cnSF", cnSF.ConnectionString));
                //cnCP = GetCPSqlConnection(plant.PlantCode);
                //response.ConnectionStrings.Add(new ConnectionString("cnCP", cnCP.ConnectionString));

                var sfE1ControlInfos = new E1ControlInfos();
                //var cpE1ControlInfos = new E1ControlInfos();
                var sfEBSUdcItems = new UdcItems();

                var step1GetDuplicateSFShortDescription = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get Duplicate SF E1ControlInfo ShortDescriptions");
                var duplicateSFShortDescriptions = E1ControlInfoManager.GetDuplicateSFShortDescriptions(cnSF);
                step1GetDuplicateSFShortDescription.RowsAffected = duplicateSFShortDescriptions.Count;
                step1GetDuplicateSFShortDescription.Done();
                response.ProcessSteps.Add(step1GetDuplicateSFShortDescription);
                if (duplicateSFShortDescriptions.Any())
                {
                    var stepGetSFE1ControlInfo = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get SF E1ControlInfos");
                    sfE1ControlInfos = E1ControlInfoManager.GetSFE1ControlInfoByShortDescription(cnSF, duplicateSFShortDescriptions);
                    stepGetSFE1ControlInfo.RowsAffected = sfE1ControlInfos.Count;
                    stepGetSFE1ControlInfo.Done();
                    response.ProcessSteps.Add(stepGetSFE1ControlInfo);
                }

                var step3GetDuplicateSFEBSShortDescription = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get Duplicate SF EBS UdcItem HandlingCodes");
                var duplicateSFHandlingCodes = UdcItemManager.GetDuplicateSFEBSHandlingCodes(cnSF);
                step3GetDuplicateSFEBSShortDescription.RowsAffected = duplicateSFHandlingCodes.Count;
                step3GetDuplicateSFEBSShortDescription.Done();
                response.ProcessSteps.Add(step3GetDuplicateSFEBSShortDescription);
                if (duplicateSFHandlingCodes.Any())
                {
                    var stepGetSFEBSShortDescriptions = new ProcessStep($@"Step {response.ProcessSteps.Count + 1}. Get SF EBS UdcItems");
                    sfEBSUdcItems = UdcItemManager.GetSFEBSUdcItemsByHandlingCode(cnSF, duplicateSFHandlingCodes);
                    stepGetSFEBSShortDescriptions.RowsAffected = sfEBSUdcItems.Count;
                    stepGetSFEBSShortDescriptions.Done();
                    response.ProcessSteps.Add(stepGetSFEBSShortDescriptions);
                }

                if (sfE1ControlInfos.Any() || sfEBSUdcItems.Any())
                {
                    response.TransferStatus = "Y";
                    response.Done();
                    SummarizeDuplicateUdcAuditSF(response, sfE1ControlInfos, sfEBSUdcItems);
                    NotifyDuplicateUdcAuditSF(response);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                cnSF.Close();
                response.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return response;
        }

        public static void SummarizeDuplicateUdcAuditSF(ProcessResponse response, E1ControlInfos sfE1ControlInfos, UdcItems sfEBSUdcItems)
        {
            var stepSummarizeDownload = new ProcessStep("Summary Collections");
            try
            {
                var sFE1ControlInfoLines = sfE1ControlInfos.ToList();
                response.SerializedObjects.Add("E1ControlInfoLine", sFE1ControlInfoLines, "E1ControlInfoLines");

                var sfEBSUdcItemLines = sfEBSUdcItems.ToList();
                response.SerializedObjects.Add("EBSUdcItemLine", sfEBSUdcItemLines, "EBSUdcItemLines");

                var headerSummary = new
                {
                    E1ControlInfoLines = sFE1ControlInfoLines.Count,
                    EBSUdcItemLines = sfEBSUdcItemLines.Count,
                    StartDate = response.ProcessSteps.Min(x => x.StartDate),
                    StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDate = response.ProcessSteps.Max(x => x.EndDate),
                    EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
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

        public static void NotifyDuplicateUdcAuditSF(ProcessResponse response)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.DuplicateUdcAuditSFResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = $@"BI [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] SF Duplicate Udc Audit Monitor";
                if (response.HasErrors()) subject = $@"Error - {subject}";
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"], ConfigurationManager.AppSettings["MailFromAddress"], toConfigAddresses, null, subject, xslOutput);
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

        private static SqlConnection GetCPSqlConnection(string plantCode)
        {
            var cnCP = new SqlConnection();
            if (plantCode != "0")
            {
                try
                {
                    cnCP = ConnectionManager.GetSqlConnectionFromKey($"{plantCode}.CLEARPATH", false);
                    cnCP.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {cnCP.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{cnCP.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnCP;
        }
    }
}
