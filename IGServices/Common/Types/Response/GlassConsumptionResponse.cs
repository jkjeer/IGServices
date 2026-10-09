using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using IGServices.Common.Managers.Response;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.ShopFloor;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class GlassConsumptionResponse
    {
        public GlassConsumptionResponse()
        {
            ResponseTimes = new ResponseTimes();
            var responseTimesMgr = new ResponseTimesManager();
            responseTimesMgr.OpenResponseTime(ResponseTimes, ProcessName, "Job", "");
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            Errors = new Errors();
            Message = "";
            IsValid = false;
            TransferDate = DateTime.Now.ToLocalTime();
            SFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
            GlassConsumptionInterops = new GlassConsumptionInterops();
            GlassConsumptionControlTrigger = new GlassConsumptionControlTrigger();
            GlassConsumptions = new GlassConsumptions();
        }

        public GlassConsumptionResponse(string processName, string queryString)
        {
            ProcessName = processName;
            ResponseTimes = new ResponseTimes();
            var responseTimesMgr = new ResponseTimesManager();
            responseTimesMgr.OpenResponseTime(ResponseTimes, ProcessName, "Job", "");
            ServiceContext = new ServiceContext
            {
                QueryString = queryString
            };
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            Errors = new Errors();
            Message = "";
            IsValid = false;
            TransferDate = DateTime.Now.ToLocalTime();
            SFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
            GlassConsumptionInterops = new GlassConsumptionInterops();
            GlassConsumptionControlTrigger = new GlassConsumptionControlTrigger();
            GlassConsumptions = new GlassConsumptions();
        }

        [DataMember(Order = 100)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120)]
        public string PlantCode { get; set; }

        [DataMember(Order = 200)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 400)]
        public ConnectionStrings ConnectionStrings { get; set; }
        //[DataMember(Order = 500)]
        //public ResponseTimes ResponseTimes { get; set; }
        [DataMember(Order = 620)]
        public Errors Errors { get; set; }

        [DataMember(Order = 800)]
        public string Environment { get; set; }
        [DataMember(Order = 801)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 802)]
        public string Message { get; set; }
        [DataMember(Order = 803)]
        public int Lines { get; set; }

        [DataMember(Order = 1160)]
        public short LockExpireMinutes { get; set; }

        [DataMember(Order = 1200)]
        public int VerbosityMode { get; set; }
        [DataMember(Order = 1220)]
        public bool Write { get; set; }

        [DataMember(Order = 1400)]
        public string ValidationStatus { get; set; }
        [DataMember(Order = 1410)]
        public bool IsValid { get; set; }

        [DataMember(Order = 1500)]
        public bool Transferred { get; set; }
        [DataMember(Order = 1502)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 1504)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 1506)]
        public long TransferBatchNum { get; set; }
        [DataMember(Order = 1508)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 1515)]
        public short SFUpdateBatchSize { get; set; }

        [DataMember(Order = 1600)]
        public GlassConsumptionControlTrigger GlassConsumptionControlTrigger { get; set; }

        [DataMember(Order = 1700)]
        public GlassConsumptionInterops GlassConsumptionInterops { get; set; }

        [DataMember(Order = 2100)]
        public GlassConsumptions GlassConsumptions { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }
}
