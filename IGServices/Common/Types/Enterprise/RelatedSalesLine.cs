using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class RelatedSalesLines : List<RelatedSalesLine>
    {
    }

    [DataContract(Namespace = "")]
    public class RelatedSalesLine
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Decimal OrderNum { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public String OrderType { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public Decimal OrderLineNum { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public DateTime OrderDate { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public DateTime RequestedDate { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public String VolumeIndicator { get; set; }
        [DataMember(Order = 32, EmitDefaultValue = false)]
        public Decimal OrderQty { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public Customer SoldToCustomer { get; set; }
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public Customer ShipToCustomer { get; set; }
        [DataMember(Order = 54, EmitDefaultValue = false)]
        public Customer OriginalSoldToCustomer { get; set; }
        [DataMember(Order = 56, EmitDefaultValue = false)]
        public Customer OriginalShipToCustomer { get; set; }


        [DataMember(Order = 100, EmitDefaultValue = false)]
        public String CustomerSequenceNum { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public String GenericSequenceNum { get; set; }
        [DataMember(Order = 104, EmitDefaultValue = false)]
        public String ProductType { get; set; }
        [DataMember(Order = 106, EmitDefaultValue = false)]
        public String CustomerPO { get; set; }
        [DataMember(Order = 108, EmitDefaultValue = false)]
        public String CustomerPart2 { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public String CustomerPart3 { get; set; }
        [DataMember(Order = 112, EmitDefaultValue = false)]
        public String CustomerPart4 { get; set; }

        [DataMember(Order = 150, EmitDefaultValue = false)]
        public String ShopFloorMsg1 { get; set; }
        [DataMember(Order = 152, EmitDefaultValue = false)]
        public String ShopFloorMsg2 { get; set; }
        [DataMember(Order = 154, EmitDefaultValue = false)]
        public String PickNotes { get; set; }
        [DataMember(Order = 156, EmitDefaultValue = false)]
        public String PackNotes { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public String Department { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public String ReasonCode { get; set; }
        [DataMember(Order = 204, EmitDefaultValue = false)]
        public String PriorityCode { get; set; }
        [DataMember(Order = 206, EmitDefaultValue = false)]
        public String Reference2 { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public bool TubesAdded { get; set; }
        [DataMember(Order = 302, EmitDefaultValue = false)]
        public bool ScoringAdded { get; set; }
        [DataMember(Order = 304, EmitDefaultValue = false)]
        public bool PreserveAdded { get; set; }
        [DataMember(Order = 306, EmitDefaultValue = false)]
        public bool Flippable { get; set; }
        [DataMember(Order = 308, EmitDefaultValue = false)]
        public bool CrateAdded { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public EdiMessages EdiMessages { get; set; }

        [DataMember(Order = 500, EmitDefaultValue = false)]
        public String Crating { get; set; }
        [DataMember(Order = 502, EmitDefaultValue = false)]
        public String CratingDesc { get; set; }
        [DataMember(Order = 504, EmitDefaultValue = false)]
        public Crates Crates { get; set; }

    }
}
