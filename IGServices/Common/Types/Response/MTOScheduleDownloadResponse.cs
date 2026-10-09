using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Configuration;
using System.Linq;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class MTOScheduleDownloadResponse
    {
        public MTOScheduleDownloadResponse()
        {
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            ProcessResponses = new ProcessResponses();
            MTOSchedule = new MTOSchedule();
            ScheduleSummary = new ScheduleSummary();
            Errors = new Errors();
            Message = "";
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
        }

        public MTOScheduleDownloadResponse(string processName)
        {
            ProcessName = processName;
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            ProcessResponses = new ProcessResponses();
            MTOSchedule = new MTOSchedule();
            ScheduleSummary = new ScheduleSummary();
            Errors = new Errors();
            Message = "";
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
        }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 142, EmitDefaultValue = false)]
        public decimal ScheduleDateE1 { get; set; }
        [DataMember(Order = 144, EmitDefaultValue = false)]
        public string BatchNum { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public ConnectionStrings ConnectionStrings { get; set; }
        //[DataMember(Order = 500, EmitDefaultValue = false)]
        //public ResponseTimes ResponseTimes { get; set; }
        [DataMember(Order = 620)]
        public Errors Errors { get; set; }
        [DataMember(Order = 710, EmitDefaultValue = false)]
        public SqlBulkCopyResponses SqlBulkCopyResponses { get; set; }

        [DataMember(Order = 720)]
        public ProcessResponses ProcessResponses { get; set; }


        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Environment { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 70)]
        public string Message { get; set; }

        [DataMember(Order = 1160, EmitDefaultValue = false)]
        public short LockExpireMinutes { get; set; }

        [DataMember(Order = 1200, EmitDefaultValue = false)]
        public int VerbosityMode { get; set; }
        [DataMember(Order = 1220, EmitDefaultValue = false)]
        public bool Write { get; set; }
        [DataMember(Order = 1230, EmitDefaultValue = false)]
        public bool Force { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public bool Transferred { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 67, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public string TransferBatchNum { get; set; }

        [DataMember(Order = 2000, EmitDefaultValue = false)]
        public MTOSchedule MTOSchedule { get; set; }

        [DataMember(Order = 2100, EmitDefaultValue = false)]
        public ScheduleSummary ScheduleSummary { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }

    public class ScheduleSummary
    {
        public ScheduleSummary()
        {
            Lines = 0;
            Units = 0;
            WorkOrders = 0;
            SalesOrders = 0;
            Customers = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public short Lines { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public short Units { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public short WorkOrders { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public short Customers { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public short SalesOrders { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public ScheduleCustomerLines ScheduleCustomerLines { get; set; }
    }

    public class ScheduleCustomerLines : List<ScheduleCustomerLine>
    {
    }

    public class ScheduleCustomerLine
    {
        public ScheduleCustomerLine()
        {
            CustNum = 0;
            CustName = "";
            WorkOrders = 0;
            SalesOrders = 0;
            UnitsQty = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public int CustNum { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CustName { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public int SalesOrders { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public int WorkOrders { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public int UnitsQty { get; set; }
    }
}
