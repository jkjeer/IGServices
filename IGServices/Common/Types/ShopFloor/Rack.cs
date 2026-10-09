using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class Racks : List<Rack>
    {
    }

    [DataContract(Namespace = "")]
    public class Rack
    {

        public Rack()
        {
            RackId = 0;
            PlantId = 0;
            Status = "";
            StatusDate = null;
            Location = "";
            ReasonCode = "";
            RackType = "";
            ScheduleNum = 0;
            BoxNum = 0;
            LoadNum = 0;
            CustomerNum = 0;
            CustomerShipNum = 0;
            SupplierNum = 0;
            GlassLbs = 0;
            RackLbs = 0;
            ReceivedDate = null;
        }

        [DataMember(Order = 2)]
        public decimal RackId { get; set; }
        [DataMember(Order = 3)]
        public int PlantId { get; set; }
        [DataMember(Order = 4)]
        public string Status { get; set; }
        [DataMember(Order = 5)]
        public DateTime? StatusDate { get; set; }
        [DataMember(Order = 7)]
        public string Location { get; set; }
        [DataMember(Order = 8)]
        public string ReasonCode { get; set; }
        [DataMember(Order = 9)]
        public string RackType { get; set; }
        [DataMember(Order = 10)]
        public decimal ScheduleNum { get; set; }
        [DataMember(Order = 11)]
        public decimal BoxNum { get; set; }
        [DataMember(Order = 12)]
        public decimal LoadNum { get; set; }
        [DataMember(Order = 13)]
        public decimal CustomerNum { get; set; }
        [DataMember(Order = 14)]
        public decimal CustomerShipNum { get; set; }
        [DataMember(Order = 15)]
        public decimal SupplierNum { get; set; }
        [DataMember(Order = 16)]
        public decimal GlassLbs { get; set; }
        [DataMember(Order = 17)]
        public decimal RackLbs { get; set; }

        [DataMember(Order = 18)]
        public DateTime? ReceivedDate { get; set; }

    }
}
