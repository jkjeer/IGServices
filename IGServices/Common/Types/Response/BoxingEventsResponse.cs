using System;
using System.Runtime.Serialization;
using System.Configuration;
using System.Linq;
using IGServices.Common.Types.ShopFloor;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class BoxingEventsResponse
    {
        public BoxingEventsResponse()
        {
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            SqlDmlResponses = new SqlDmlResponses();
            Message = "";
            Errors = new Errors();
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
        }

        public BoxingEventsResponse(string processName, string queryString = null)
        {
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext
            {
                QueryString = queryString
            };
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            SqlDmlResponses = new SqlDmlResponses();
            Message = "";
            Errors = new Errors();
            LockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
            ProcessName = processName;
        }

        [DataMember(Order = 100)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120)]
        public string PlantCode { get; set; }
        [DataMember(Order = 130)]
        public short LockExpireMinutes { get; set; }

        [DataMember(Order = 200)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 400)]
        public ConnectionStrings ConnectionStrings { get; set; }
        //[DataMember(Order = 500)]
        //public ResponseTimes ResponseTimes { get; set; }
        [DataMember(Order = 620)]
        public Errors Errors { get; set; }
        [DataMember(Order = 700)]
        public SqlDmlResponses SqlDmlResponses { get; set; }

        [DataMember(Order = 20)]
        public string Environment { get; set; }
        [DataMember(Order = 21)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 70)]
        public string Message { get; set; }

        [DataMember(Order = 90)]
        public BoxingEvents BoxingEvents { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }
}
