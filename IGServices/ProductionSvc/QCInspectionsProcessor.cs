using System;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace IGServices.ProductionSvc
{
    public class QCInspectionsProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse GetQCInspections(string plantCode)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Get QC Inspections ({plantCode})", true);
            var cnSFInt = new SqlConnection();
            var cnSF = new SqlConnection();
            var cnCorpInt = new SqlConnection();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                response.Plant = plant;
                cnSFInt = GetSFIntSqlConnection(plant.PlantNum);
                cnSF = GetSFSqlConnection(plant.PlantNum);
                cnCorpInt = GetCorpIntSqlConnection();
                response = QCInspectionManager.Process(cnSFInt, cnSF, cnCorpInt, plant);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}");
                response.Errors.Add(new Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}"));
                NotifyQCInspections(response);
            }
            finally
            {
                cnSFInt.Close();
                cnSF.Close();
                cnCorpInt.Close();
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        public static void NotifyQCInspections(ProcessResponse response)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                if (string.Equals(ConfigurationManager.AppSettings["SendPlantMail"], "true", StringComparison.CurrentCultureIgnoreCase)) toConfigAddresses += $",{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.QCInspectionsResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = $@"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] QC Inspections";
                if (response.HasErrors()) subject = $@"Error - {subject}";
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
                    Logger.Error($@"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($@"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSFInt;
        }

        private static SqlConnection GetSFSqlConnection(string plantNum)
        {
            var cnSF = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnSF = ConnectionManager.GetSqlConnectionFromKey($@"{plantNum}.SHOPFLOOR", false);
                    cnSF.Open();
                }
                catch (Exception e)
                {
                    Logger.Error($@"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($@"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
            }
            return cnSF;
        }

        private static SqlConnection GetCorpIntSqlConnection()
        {
            var cnCorpInt = new SqlConnection();
            try
            {
                cnCorpInt = ConnectionManager.GetSqlConnectionFromKey(@"CORP.INTEGRATION", false);
                cnCorpInt.Open();
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return cnCorpInt;
        }
    }
}