using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class SpacerConsumptions : List<SpacerConsumption>
    {
    }

    [DataContract(Namespace = "")]
    public class SpacerConsumption
    {
        public SpacerConsumption()
        {
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = 0;
            TransferDate = DateTime.Now.ToLocalTime();
            IsValid = true;
            ValidationStatus = "Valid";
            UnitOfMeasure = "";
        }

        [DataMember(Order = 10)]
        public string PlantNum { get; set; }

        [DataMember(Order = 22)]
        public int ConsumptionAggregateId { get; set; }
        [DataMember(Order = 23)]
        public int SpacerItemId { get; set; }

        [DataMember(Order = 30)]
        public string ItemNum { get; set; }

        [DataMember(Order = 40)]
        public int SourceRows { get; set; }

        [DataMember(Order = 50)]
        public int Quantity { get; set; }

        [DataMember(Order = 54)]
        public DateTime ProductionDate { get; set; }
        [DataMember(Order = 55)]
        public DateTime MinWrittenDate { get; set; }
        [DataMember(Order = 56)]
        public DateTime MaxWrittenDate { get; set; }

        [DataMember(Order = 60)]
        public bool IsValid { get; set; }
        [DataMember(Order = 61)]
        public string ValidationStatus { get; set; }

        [DataMember(Order = 70)]
        public bool Transferred { get; set; }
        [DataMember(Order = 72)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 76)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 77, EmitDefaultValue = false)]
        public string TransferDateString { get; set; }
        [DataMember(Order = 78)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 80)]
        public long TransferBatchNum { get; set; }
        [DataMember(Order = 82)]
        public decimal TransferUniqueID { get; set; }

        [DataMember(Order = 83)]
        public string UnitOfMeasure { get; set; }

    }
}
