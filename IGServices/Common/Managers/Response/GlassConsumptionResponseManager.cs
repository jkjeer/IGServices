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
    public class GlassConsumptionResponseManager
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Init(GlassConsumptionResponse response, string plantNum, int verbosityMode, bool write)
        {
            var plantAttribute = new PlantAttribute(plantNum);
            response.PlantNum = plantAttribute.NumericCode;
            response.PlantCode = plantAttribute.Code;
            response.VerbosityMode = verbosityMode;
            response.Write = write;
        }

        public void Close(GlassConsumptionResponse response)
        {
            response.Lines = response.GlassConsumptionInterops.Count;
            foreach (var spacerConsumption in response.GlassConsumptions.Where(glassConsumption => !glassConsumption.IsValid))
            {
                response.Errors = ErrorManager.AddError(response.Errors, spacerConsumption.TransferMessage);
            }
            var responseTimesMgr = new ResponseTimesManager();
            responseTimesMgr.CloseResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            if (response.HasErrors()) Notify(response);
            SetVerbosity(response);
        }

        public bool Notify(GlassConsumptionResponse response)
        {
            try
            {
                var xml = XmlSerializationUtility.SerializeObject(Encoding.UTF8, response);
                var stream = new MemoryStream(Encoding.Default.GetBytes(xml));
                var document = new XPathDocument(stream);
                var strmXsltGood = Assembly.GetExecutingAssembly().GetManifestResourceStream("IGServices.ProductionSvc.GlassConsumptionResponseBody.xslt");
                var transform = new XslCompiledTransform();
                if (strmXsltGood != null)
                {
                    var reader1 = XmlReader.Create(strmXsltGood);
                    transform.Load(reader1);
                }
                var writer1 = new StringWriter();
                transform.Transform(document, null, writer1);
                var toPlantAddress = "";
                //if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = response.PlantCode + ConfigurationManager.AppSettings["PlantEmailFormat"];
                var sc = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Timeout = Convert.ToInt16(ConfigurationManager.AppSettings["SmtpHostTimeout"])
                };
                //if (response.Lines > 0)
                //{
                var msg1 = new MailMessage();
                var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                msg1.To.Add(toAddrs1);
                msg1.From = new MailAddress(ConfigurationManager.AppSettings["MailFromAddress"]);
                msg1.Subject = response.PlantCode + " " +
                               "[" + response.Environment + "] " + "Glass Consumption Uploads";
                msg1.IsBodyHtml = true;
                msg1.Body = writer1.ToString();
                //sc.Send(msg1);
                sc.SendAsync(msg1, null);
                //}
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            return true;
        }

        public void SetVerbosity(GlassConsumptionResponse response)
        {
            switch (response.VerbosityMode)
            {
                case 0:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    response.ResponseTimes = null;
                    response.GlassConsumptions = null;
                    response.GlassConsumptionInterops = null;
                    response.GlassConsumptionControlTrigger = null;
                    break;
                case 1:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    response.ResponseTimes = null;
                    response.GlassConsumptionInterops = null;
                    response.GlassConsumptionControlTrigger = null;
                    break;
                case 2:
                    response.GlassConsumptionInterops = null;
                    response.GlassConsumptionControlTrigger = null;
                    break;
                case 3:
                    break;
            }
        }
    }
}
