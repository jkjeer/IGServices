using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class MTOSchedules : List<MTOSchedule>
    {
    }

    [DataContract(Namespace = "")]
    public class MTOSchedule
    {
        public MTOSchedule(string plantNum, decimal scheduleNum)
        {
            PlantNum = plantNum;
            ScheduleNum = scheduleNum;
            ValidationErrors = new ValidationErrors();
            ValidationStatus = "";
            E1ScheduleData = new E1ScheduleData();
            MTOScheduleEvent = new MTOScheduleEvent();
            MTOIncomingOrderLines = new MTOIncomingOrderLines();
            TransferDate = DateTime.Now.ToLocalTime();
            OriginalSalesLineAttributes = new SalesLineAttributes();
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 230)]
        public ValidationErrors ValidationErrors { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public DateTime ScheduleDate { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal ScheduleDateE1 { get; set; }
        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string BatchNum { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public DateTime DueDate { get; set; }
        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string ShopFloorCode { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string ShopFloorName { get; set; }
        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string ScheduleStatus { get; set; }
        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string ScheduleType { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string ValidationStatus { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 65, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public E1ScheduleData E1ScheduleData { get; set; }

        [DataMember(Order = 101, EmitDefaultValue = false)]
        public MTOScheduleEvent MTOScheduleEvent { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public MTOIncomingOrderLines MTOIncomingOrderLines { get; set; }

        [DataMember(Order = 500, EmitDefaultValue = false)]
        public MTOScheduleLines MTOScheduleLines { get; set; }

        [DataMember(Order = 600, EmitDefaultValue = false)]
        public ShopFloorMTOOrders ShopFloorMTOOrders { get; set; }

        [DataMember(Order = 264, EmitDefaultValue = false)]
        public SalesLineAttributes OriginalSalesLineAttributes { get; set; }


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
