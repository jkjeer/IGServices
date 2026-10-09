using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class GenericResponse<T>
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GenericResponse()
        {
            //ResponseTimes = new ResponseTimes();
            ServiceContext = new ServiceContext();
            AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Environment = ConfigurationManager.AppSettings["Environment"];
            ConnectionStrings = new ConnectionStrings();
            Errors = new Errors();
            Message = "";
            Lines = 0;
            PlantNum = "";
            PlantCode = "";
            PlantTimeOffset = 0;
            ProcessName = typeof(T).Name + "s";
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public List<T> Results { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public string ProcessName { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public int PlantTimeOffset { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ServiceContext ServiceContext { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public int VerbosityMode { get; set; }
        [DataMember(Order = 400, EmitDefaultValue = false)]
        public ConnectionStrings ConnectionStrings { get; set; }
        //[DataMember(Order = 500, EmitDefaultValue = false)]
        //public ResponseTimes ResponseTimes { get; set; }
        [DataMember(Order = 620, EmitDefaultValue = false)]
        public Errors Errors { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Environment { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string AssemblyVersion { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string Message { get; set; }
        [DataMember(Order = 88, EmitDefaultValue = false)]
        public int Lines { get; set; }

        [DataMember(Order = 1000, EmitDefaultValue = false)]
        public Transfer Transfer { get; set; }

        public bool HasErrors()
        {
            return Errors.Any();
        }
    }
}
