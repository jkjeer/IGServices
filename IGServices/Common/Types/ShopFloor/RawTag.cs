using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.ShopFloor
{

    [CollectionDataContract(Namespace = "")]
    public class RawTags : List<RawTag>
    {
    }

    [DataContract(Namespace = "")]
    public class RawTag
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public decimal RackId { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string SerialNum { get; set; }
        [DataMember(Order = 14, EmitDefaultValue = false)]
        public string ItemNum { get; set; }
        [DataMember(Order = 16, EmitDefaultValue = true)]
        public string Location { get; set; }
        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string Status { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = true)]
        public decimal QtyOnHand { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)]
        public DateTime Writtendate { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string ReasonCode { get; set; }
        [DataMember(Order = 42, EmitDefaultValue = false)]
        public string RackType { get; set; }
        [DataMember(Order = 44, EmitDefaultValue = false)]
        public decimal LitesPerRack { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }
        [DataMember(Order = 62, EmitDefaultValue = false)]
        public decimal BaseL { get; set; }
        [DataMember(Order = 64, EmitDefaultValue = false)]
        public decimal BaseN { get; set; }
        [DataMember(Order = 66, EmitDefaultValue = false)]
        public decimal BaseD { get; set; }
        [DataMember(Order = 68, EmitDefaultValue = false)]
        public decimal LeftL { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public decimal LeftN { get; set; }
        [DataMember(Order = 72, EmitDefaultValue = false)]
        public decimal LeftD { get; set; }
        [DataMember(Order = 74, EmitDefaultValue = false)]
        public decimal RightL { get; set; }
        [DataMember(Order = 76, EmitDefaultValue = false)]
        public decimal RightN { get; set; }
        [DataMember(Order = 78, EmitDefaultValue = false)]
        public decimal RightD { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public decimal TopL { get; set; }
        [DataMember(Order = 82, EmitDefaultValue = false)]
        public decimal TopN { get; set; }
        [DataMember(Order = 84, EmitDefaultValue = false)]
        public decimal TopD { get; set; }
        [DataMember(Order = 86, EmitDefaultValue = false)]
        public decimal Side1L { get; set; }
        [DataMember(Order = 88, EmitDefaultValue = false)]
        public decimal Side1N { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal Side1D { get; set; }
        [DataMember(Order = 92, EmitDefaultValue = false)]
        public decimal Side2L { get; set; }
        [DataMember(Order = 94, EmitDefaultValue = false)]
        public decimal Side2N { get; set; }
        [DataMember(Order = 96, EmitDefaultValue = false)]
        public decimal Side2D { get; set; }
        [DataMember(Order = 98, EmitDefaultValue = false)]
        public decimal SqftActual { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal SqftBillable { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal Lite1_Thick { get; set; }
        [DataMember(Order = 202, EmitDefaultValue = false)]
        public string Lite1_Color { get; set; }
        [DataMember(Order = 204, EmitDefaultValue = false)]
        public string Lite1_Heat { get; set; }
        [DataMember(Order = 206, EmitDefaultValue = false)]
        public string Lite1_1stSrfCoating { get; set; }
        [DataMember(Order = 208, EmitDefaultValue = false)]
        public string Lite1_2ndSrfCoating { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public string Pattern { get; set; }
        [DataMember(Order = 212, EmitDefaultValue = false)]
        public string Lite1_ProdQualityLevel { get; set; }
        [DataMember(Order = 214, EmitDefaultValue = false)]
        public string Lite1_Lami { get; set; }
        [DataMember(Order = 216, EmitDefaultValue = false)]
        public string AnsiiLogoType { get; set; }
        [DataMember(Order = 218, EmitDefaultValue = false)]
        public string RawCTCFCode { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string VenderID { get; set; }
        [DataMember(Order = 302, EmitDefaultValue = false)]
        public string CustomerName { get; set; }
        [DataMember(Order = 304, EmitDefaultValue = false)]
        public DateTime ReceivedDate { get; set; }
        [DataMember(Order = 306, EmitDefaultValue = false)]
        public decimal ReceivedQty { get; set; }
        [DataMember(Order = 308, EmitDefaultValue = false)]
        public DateTime ExpirationDate { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public DateTime ProductionDate { get; set; }
        [DataMember(Order = 312, EmitDefaultValue = false)]
        public string ProcessID { get; set; }
        [DataMember(Order = 314, EmitDefaultValue = false)]
        public decimal SequenceNum { get; set; }
        [DataMember(Order = 316, EmitDefaultValue = false)]
        public string GLCategory { get; set; }

        [DataMember(Order = 400, EmitDefaultValue = false)]
        public int RawTagsID { get; set; }

    }
}
