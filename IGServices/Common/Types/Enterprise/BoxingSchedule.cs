using System;
using System.Linq;
using System.Runtime.Serialization;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class BoxingSchedule
    {
        public BoxingSchedule(decimal scheduleNum)
        {
            ScheduleNum = scheduleNum;
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = "0";
            //IsValid = false;
            ValidationErrors = new ValidationErrors();
            ValidationStatus = "";
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantCode { get; set; }
        [DataMember(Order = 15, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        //[DataMember(Order = 220)]
        //public Errors Errors { get; set; }
        [DataMember(Order = 230)]
        public ValidationErrors ValidationErrors { get; set; }

        [DataMember(Order = 250)]
        public ProcessSteps ProcessSteps { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public int BoxHoldHeaderID { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public bool IsCompleted { get; set; }

        [DataMember(Order = 56, EmitDefaultValue = false)]
        public DateTime BatchDate { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string ValidationStatus { get; set; }

        [DataMember(Order = 62, EmitDefaultValue = false)]
        public bool Transferred { get; set; }

        [DataMember(Order = 64, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 67, EmitDefaultValue = false)]
        public string TransferDateString { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 69, EmitDefaultValue = false)]
        public string TransferBatchNum { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public int UnitBoxCount { get; set; }
        [DataMember(Order = 81, EmitDefaultValue = false)]
        public DateTime UnitBoxMaxTimeStamp { get; set; }
        [DataMember(Order = 82, EmitDefaultValue = false)]
        public int IncomingOrdersUnitCount { get; set; }
        [DataMember(Order = 83, EmitDefaultValue = false)]
        public int IncomingOrdersWorkOrderCount { get; set; }
        [DataMember(Order = 84, EmitDefaultValue = false)]
        public DateTime IncomingOrdersMaxDownLoadDate { get; set; }

        [DataMember(Order = 85, EmitDefaultValue = false)]
        public int WorkOrderCount { get; set; }
        [DataMember(Order = 86, EmitDefaultValue = false)]
        public DateTime IncomingOrdersMinScheduleDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public BoxItems BoxItems { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public BoxF580200 BoxF580200 { get; set; }

        public bool IsValid()
        {
            return !ValidationErrors.Any();
        }

        public bool IsFatal()
        {
            return ValidationErrors.Any(error => error.IsFatal);
        }
    }
}
