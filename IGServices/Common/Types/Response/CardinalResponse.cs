using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "", Name = "CardinalResponseOf{0}")]
    public class CardinalResponse<T>
    {
        public CardinalResponse()
        {
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            SqlDmlResponses = new SqlDmlResponses();
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
            Errors = new Errors();
            Message = "";
            Lines = 0;
            PlantNum = "";
            PlantCode = "";
            PlantTimeOffset = 0;
            ProcessName = typeof(T).ToString();
        }

        [DataMember(Order = 1)]
        public CardinalCollection<T> Results { get; set; }

        [DataMember(Order = 100)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120)]
        public string PlantCode { get; set; }
        [DataMember(Order = 130)]
        public int PlantTimeOffset { get; set; }

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
        [DataMember(Order = 702)]
        public SqlBulkCopyResponses SqlBulkCopyResponses { get; set; }

        [DataMember(Order = 20)]
        public string Environment { get; set; }
        [DataMember(Order = 21)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 70)]
        public string Message { get; set; }
        [DataMember(Order = 88)]
        public int Lines { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }
}
