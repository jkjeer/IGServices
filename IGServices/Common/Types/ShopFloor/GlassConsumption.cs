using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class GlassConsumptions : List<GlassConsumption>
    {
    }

    [DataContract(Namespace = "")]
    public class GlassConsumption
    {
        public GlassConsumption()
        {
            TransferMessage = "";
            TransferStatus = "";
            TransferBatchNum = 0;
            TransferDate = DateTime.Now.ToLocalTime();
            IsValid = true;
            ValidationStatus = "Valid";
        }

        [DataMember(Order = 10)]
        public string PlantNum { get; set; }

        [DataMember(Order = 22)]
        public int ConsumptionId { get; set; }

        [DataMember(Order = 30)]
        public string RawCrateNumber { get; set; }
        [DataMember(Order = 40)]
        public string Shift { get; set; }
        [DataMember(Order = 50)]
        public DateTime ProductionDate { get; set; }
        [DataMember(Order = 60)]
        public int Quantity { get; set; }
        [DataMember(Order = 80)]
        public int SheetsConsumed { get; set; }
        [DataMember(Order = 90)]
        public int SheetsRejected { get; set; }
        [DataMember(Order = 100)]
        public string CuttingTable { get; set; }
        [DataMember(Order = 110)]
        public string RawCTCFCode { get; set; }
        [DataMember(Order = 120)]
        public string Crew { get; set; }
        [DataMember(Order = 130)]
        public string E1WorkCenter { get; set; }
        [DataMember(Order = 140)]
        public string E1Operation { get; set; }
        [DataMember(Order = 150)]
        public byte RawGlassBaseL { get; set; }
        [DataMember(Order = 160)]
        public byte RawGlassLeftL { get; set; }


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
        [DataMember(Order = 78)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 80)]
        public long TransferBatchNum { get; set; }
        [DataMember(Order = 82)]
        public decimal TransferId { get; set; }
    }
}
