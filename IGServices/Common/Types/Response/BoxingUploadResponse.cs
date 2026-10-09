using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using IGServices.Common.Managers.Response;
using IGServices.Common.Types.Enterprise;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class BoxingUploadResponse
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BoxingUploadResponse()
        {
            //ResponseTimes = new ResponseTimes();
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.OpenResponseTime(ResponseTimes, ProcessName, "Job", "");
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            BoxingSchedule = new BoxingSchedule();
            BoxingSummary = new BoxingSummary();
            Errors = new Errors();
            Message = "";
            SFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
        }

        public BoxingUploadResponse(string processName, string queryString)
        {
            ProcessName = processName;
            //ResponseTimes = new ResponseTimes();
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.OpenResponseTime(ResponseTimes, ProcessName, "Job", "");
            ServiceContext = new ServiceContext
            {
                QueryString = queryString
            };
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            BoxingSchedule = new BoxingSchedule();
            BoxingSummary = new BoxingSummary();
            Errors = new Errors();
            Message = "";
            SFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
            Logger.Info("QueryString: " + ServiceContext.QueryString);
        }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public ConnectionStrings ConnectionStrings { get; set; }
        //[DataMember(Order = 500, EmitDefaultValue = false)]
        //public ResponseTimes ResponseTimes { get; set; }
        [DataMember(Order = 620)]
        public Errors Errors { get; set; }

        [DataMember(Order = 800, EmitDefaultValue = false)]
        public string Environment { get; set; }
        [DataMember(Order = 801, EmitDefaultValue = false)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 802)]
        public string Message { get; set; }

        [DataMember(Order = 1150, EmitDefaultValue = false)]
        public short SFUpdateBatchSize { get; set; }

        [DataMember(Order = 1160, EmitDefaultValue = false)]
        public short LockExpireMinutes { get; set; }

        [DataMember(Order = 1200, EmitDefaultValue = false)]
        public int VerbosityMode { get; set; }
        [DataMember(Order = 1220, EmitDefaultValue = false)]
        public bool Write { get; set; }

        [DataMember(Order = 2000, EmitDefaultValue = false)]
        public BoxingSchedule BoxingSchedule { get; set; }

        [DataMember(Order = 2100, EmitDefaultValue = false)]
        public BoxingSummary BoxingSummary { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }

    public class BoxingSummary
    {
        public BoxingSummary()
        {
            Units = 0;
            WorkOrders = 0;
            Boxes = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public short Units { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public short WorkOrders { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public short Boxes { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public BoxCustomerLines BoxCustomerLines { get; set; }
    }

    public class BoxingSummaryLines : List<BoxingSummaryLine>
    {
    }

    public class BoxingSummaryLine
    {
        public BoxingSummaryLine()
        {
            BoxNum = 0;
            BoxType = "";
            RackTypeDesc = "";
            UnitsQty = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public short BoxNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string BoxType { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string RackTypeDesc { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public short UnitsQty { get; set; }
    }

    public class BoxCustomerLines : List<BoxCustomerLine>
    {
    }

    public class BoxCustomerLine
    {
        public BoxCustomerLine()
        {
            BoxNum = 0;
            BoxType = "";
            RackTypeDesc = "";
            CustomerName = "";
            MinUnitID = "";
            MaxUnitID = "";
            MinWorkOrderNum = 0;
            MaxWorkOrderNum = 0;
            UnitsQty = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public short BoxNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string BoxType { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string RackTypeDesc { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string CustomerName { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public short UnitsQty { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string MinUnitID { get; set; }
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public string MaxUnitID { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public DateTime MinDueDate { get; set; }
        [DataMember(Order = 61, EmitDefaultValue = false)]
        public DateTime MaxDueDate { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal MinWorkOrderNum { get; set; }
        [DataMember(Order = 71, EmitDefaultValue = false)]
        public decimal MaxWorkOrderNum { get; set; }
    }
}