using System;
using System.Runtime.Serialization;
using System.Configuration;
using System.Linq;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    [KnownType("GetKnownTypes")]
    public class BaseResponse
    {
        // ReSharper disable once UnusedMember.Local
        private static Type[] GetKnownTypes()
        {
            return new[]
            {
                typeof (SpacerConsumptionResponse),
                typeof (SpacerItemsResponse)
            };
        }

        public BaseResponse()
        {
            //RetrievalTimes = new RetrievalTimes {StartTimeJob = DateTime.Now.ToLocalTime()};
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            Message = "";
            Lines = 0;
            PlantNum = "";
            PlantCode = "";
            PlantTimeOffset = 0;
            Errors = new Errors();
        }

        [DataMember(Order = 1)]
        public string ProcessName { get; set; }

        [DataMember(Order = 10)]
        public string PlantNum { get; set; }
        [DataMember(Order = 11)]
        public string PlantCode { get; set; }
        [DataMember(Order = 12)]
        public int PlantTimeOffset { get; set; }

        [DataMember(Order = 20)]
        public string Environment { get; set; }
        [DataMember(Order = 21)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 22)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 25)]
        public RetrievalTimes RetrievalTimes { get; set; }
        [DataMember(Order = 28)]
        public ConnectionStrings ConnectionStrings { get; set; }
        [DataMember(Order = 41)]
        public Errors Errors { get; set; }
        [DataMember(Order = 45)]
        public string ValidationStatus { get; set; }

        [DataMember(Order = 70)]
        public string Message { get; set; }

        [DataMember(Order = 88)]
        public int Lines { get; set; }

        //[DataMember(Order = 100)]
        //public Transfer Transfer { get; set; }

        [DataMember(Order = 120)]
        public int VerbosityMode { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }
}
