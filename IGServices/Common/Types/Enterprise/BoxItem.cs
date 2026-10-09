using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class BoxItems : List<BoxItem>
    {
    }

    [DataContract(Namespace = "")]
    public class BoxItem
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int BoxHoldHeaderID { get; set; }
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public int BoxHoldId { get; set; }
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string WorkOrderType { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string SalesOrderType { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public decimal SalesOrderLine { get; set; }

        [DataMember(Order = 23, EmitDefaultValue = false)]
        public string UnitBoxCustomer { get; set; }
        [DataMember(Order = 24, EmitDefaultValue = false)]
        public string IncomingOrdersCustomer { get; set; }

        [DataMember(Order = 241, EmitDefaultValue = false)]
        public int IncomingOrdersCustNum { get; set; }
        [DataMember(Order = 242, EmitDefaultValue = false)]
        public decimal IncomingOrdersShipToCustNum { get; set; }
        [DataMember(Order = 243, EmitDefaultValue = false)]
        public string Customer { get; set; }


        [DataMember(Order = 25, EmitDefaultValue = false)]
        public decimal IncomingOrdersWorkOrderNum { get; set; }
        [DataMember(Order = 26, EmitDefaultValue = false)]
        public decimal IncomingOrdersQuantity { get; set; }
        [DataMember(Order = 27, EmitDefaultValue = false)]
        public DateTime UnitBoxDueDate { get; set; }
        [DataMember(Order = 28, EmitDefaultValue = false)]
        public DateTime UnitBoxTimeStamp { get; set; }
        [DataMember(Order = 29, EmitDefaultValue = false)]
        public DateTime IncomingOrdersScheduleDate { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public DateTime IncomingOrdersDownloadDate { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string UnitId { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public string SerialNum { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public byte LineID { get; set; }
        [DataMember(Order = 340, EmitDefaultValue = false)]
        public string IGLine { get; set; }
        [DataMember(Order = 350, EmitDefaultValue = false)]
        public short RunSequence { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal RackId { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public string RackType { get; set; }
        [DataMember(Order = 46, EmitDefaultValue = false)]
        public string RackTypeDesc { get; set; }
        [DataMember(Order = 48, EmitDefaultValue = false)]
        public string OnSkid { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public bool BoxCrate { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public short BoxNum { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public bool BoxRotated { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Side { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public byte Stack { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public byte Row { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public short Slot { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string Lite1Rack { get; set; }
        [DataMember(Order = 71, EmitDefaultValue = false)]
        public int Lite1Slot { get; set; }
        [DataMember(Order = 74, EmitDefaultValue = false)]
        public string Lite2Rack { get; set; }
        [DataMember(Order = 75, EmitDefaultValue = false)]
        public int Lite2Slot { get; set; }
        [DataMember(Order = 78, EmitDefaultValue = false)]
        public string Lite3Rack { get; set; }
        [DataMember(Order = 79, EmitDefaultValue = false)]
        public int Lite3Slot { get; set; }

        [DataMember(Order = 80, EmitDefaultValue = false)]
        public DateTime UpdateDate { get; set; }

        [DataMember(Order = 90, EmitDefaultValue = false)]
        public string ValidationStatus { get; set; }

        [DataMember(Order = 92, EmitDefaultValue = false)]
        public bool Transferred { get; set; }
        [DataMember(Order = 94, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 95, EmitDefaultValue = false)]
        public string TransferDateString { get; set; }

        [DataMember(Order = 96, EmitDefaultValue = false)]
        public bool InProgress { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal UniqueID { get; set; }

        [DataMember(Order = 105, EmitDefaultValue = false)]
        public string TransferBatchNum { get; set; }
        [DataMember(Order = 106, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 107, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
    }
}
