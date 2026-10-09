using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
//using NLog;

namespace IGServices.Common.Managers.Response
{
    public class SpacerConsumptionResponseManager
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Init(SpacerConsumptionResponse response, string plantNum, int verbosityMode, bool write)
        {
            var plantAttribute = new PlantAttribute(plantNum);
            response.PlantNum = plantAttribute.NumericCode;
            response.PlantCode = plantAttribute.Code;
            response.VerbosityMode = verbosityMode;
            response.Write = write;
        }

        public void SuccessE1(SpacerConsumptionResponse response)
        {
            try
            {
                response.Transferred = true;
                response.TransferStatus = "Y";
                foreach (var spacerConsumption in response.SpacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                {
                    spacerConsumption.Transferred = true;
                    spacerConsumption.TransferStatus = "Y";
                    spacerConsumption.TransferDate = response.TransferDate;
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void FailE1(SpacerConsumptionResponse response)
        {
            try
            {
                response.Transferred = false;
                response.TransferStatus = "N";
                foreach (var spacerConsumption in response.SpacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                {
                    spacerConsumption.Transferred = false;
                    spacerConsumption.TransferStatus = "E";
                    spacerConsumption.TransferDate = response.TransferDate;
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void Close(SpacerConsumptionResponse response)
        {
            response.Lines = response.SpacerConsumptions.Count;
            foreach (var spacerConsumption in response.SpacerConsumptions.Where(spacerConsumption => !spacerConsumption.IsValid))
            {
                response.Errors = ErrorManager.AddError(response.Errors, spacerConsumption.TransferMessage);
            }
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.CloseResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            if ( response.HasErrors()) Notify(response);
            SetVerbosity(response);
        }

        /******************************************************************
        * Notification
        ******************************************************************/
        public bool Notify(SpacerConsumptionResponse response)
        {
            try
            {
                //var x = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                var xml = XmlSerializationUtility.SerializeObject(Encoding.UTF8, response);
                var stream = new MemoryStream(Encoding.Default.GetBytes(xml));
                var document = new XPathDocument(stream);
                var strmXsltGood = Assembly.GetExecutingAssembly().GetManifestResourceStream("IGServices.ProductionSvc.SpacerConsumptionResponseBody.xslt");
                var transform = new XslCompiledTransform();
                if (strmXsltGood != null)
                {
                    var reader1 = XmlReader.Create(strmXsltGood);
                    transform.Load(reader1);
                }
                var writer1 = new StringWriter();
                transform.Transform(document, null, writer1);
                var toPlantAddress = "";
                var sc = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Timeout = Convert.ToInt16(ConfigurationManager.AppSettings["SmtpHostTimeout"])
                };
                var msg1 = new MailMessage();
                var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                msg1.To.Add(toAddrs1);
                msg1.From = new MailAddress(ConfigurationManager.AppSettings["MailFromAddress"]);
                msg1.Subject = response.PlantCode + " " + "[" + response.Environment + "] " + "Spacer Consumption Uploads";
                msg1.IsBodyHtml = true;
                msg1.Body = writer1.ToString();
                sc.SendAsync(msg1, null);
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            return true;
        }

        public void SetVerbosity(SpacerConsumptionResponse response)
        {
            switch (response.VerbosityMode)
            {
                case 0:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    //response.ResponseTimes = null;
                    response.SpacerConsumptions = null;
                    response.SpacerConsumptionInterops = null;
                    response.SpacerConsumptionControlTrigger = null;
                    break;
                case 1:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    //response.ResponseTimes = null;
                    response.SpacerConsumptionInterops = null;
                    response.SpacerConsumptionControlTrigger = null;
                    break;
                case 2:
                    response.SpacerConsumptionInterops = null;
                    response.SpacerConsumptionControlTrigger = null;
                    break;
                case 3:
                    break;
            }
        }
    }
}