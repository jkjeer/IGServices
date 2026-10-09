using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class BoxingEvents : List<BoxingEvent>
    {
    }

    [DataContract(Namespace = "")]
    public class BoxingEvent
    {
        [DataMember(Order = 10)]
        public string PlantNum { get; set; }
        [DataMember(Order = 12)]
        public string PlantCode { get; set; }

        [DataMember(Order = 20)]
        public short ScheduleNum { get; set; }

        [DataMember(Order = 22)]
        public int BoxHoldHeaderID { get; set; }

        [DataMember(Order = 30)]
        public DateTime HeaderWrittenDate { get; set; }

        [DataMember(Order = 40)]
        public bool IsCompleted { get; set; }

        [DataMember(Order = 50)]
        public bool HeaderTransferred { get; set; }

        [DataMember(Order = 52)]
        public string HeaderTransferStatus { get; set; }

        [DataMember(Order = 60)]
        public int Units { get; set; }

        [DataMember(Order = 70)]
        public int WorkOrders { get; set; }

        [DataMember(Order = 80)]
        public int Boxes { get; set; }

        [DataMember(Order = 90)]
        public int RackTypes { get; set; }

        [DataMember(Order = 100)]
        public int Customers { get; set; }

        [DataMember(Order = 110)]
        public string MinUnitID { get; set; }

        [DataMember(Order = 111)]
        public string MaxUnitID { get; set; }

        [DataMember(Order = 120)]
        public DateTime MinDueDate { get; set; }

        [DataMember(Order = 121)]
        public DateTime MaxDueDate { get; set; }
    }
}
