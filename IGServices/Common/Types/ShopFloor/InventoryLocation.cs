using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class InventoryLocations : List<InventoryLocation>
    {
    }

    public class InventoryLocation
    {

        public InventoryLocation()
        {
            LocationCode = "";
            Aisle = "";
            Bin = "";
            CategoryCode = "";
            DetailLevel = "";
            StagingFlag = "";
            PutawayZone = "";
            Category3 = "";
            Category4 = "";
        }

        [DataMember(Order = 1)]
        public string LocationCode { get; set; }
        [DataMember(Order = 2)]
        public string Aisle { get; set; }
        [DataMember(Order = 3)]
        public string Bin { get; set; }
        [DataMember(Order = 4)]
        public string CategoryCode { get; set; }
        [DataMember(Order = 5)]
        public string DetailLevel { get; set; }
        [DataMember(Order = 6)]
        public string PutawayZone { get; set; }
        [DataMember(Order = 7)]
        public string StagingFlag { get; set; }
        [DataMember(Order = 8)]
        public string Category3 { get; set; }
        [DataMember(Order = 9)]
        public string Category4 { get; set; }
        [DataMember(Order = 10)]
        public DateTime UpdateDate { get; set; }
    }
}
