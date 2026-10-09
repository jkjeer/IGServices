using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using IBM.Data.DB2.iSeries;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class RawGlassItems : List<RawGlassItem>
    {
    }

    [DataContract(Namespace = "")]
    public class RawGlassItem
    {
        [DataMember(Order = 10)]
        public decimal WMCPIT { get; set; }
        [DataMember(Order = 20)]
        public string WMCMCU { get; set; }
        [DataMember(Order = 40)]
        public decimal WMDOCO { get; set; }
        [DataMember(Order = 50)]
        public string WMDCTO { get; set; }
        [DataMember(Order = 60)]
        public string WMDSC1 { get; set; }
        [DataMember(Order = 70)]
        public string WMDSC2 { get; set; }
        [DataMember(Order = 80)]
        public string WMLOCN { get; set; }
        [DataMember(Order = 90)]
        public decimal WMUORG { get; set; }
        [DataMember(Order = 100)]
        public decimal WMTRQT { get; set; }
        [DataMember(Order = 110)]
        public decimal WMOPSQ { get; set; }
        [DataMember(Order = 120)]
        public Int64 WMOPSQ_RN { get; set; }
        [DataMember(Order = 130)]
        public decimal WMUKID { get; set; }
        [DataMember(Order = 140)]
        public decimal WMUPMJ { get; set; }
        [DataMember(Order = 150)]
        public decimal WMTDAY { get; set; }

        [DataMember(Order = 200)]
        public decimal CRZ58L1TK { get; set; }
        [DataMember(Order = 210)]
        public string CRZ58L1HT { get; set; }
        [DataMember(Order = 220)]
        public string CRZ58L1CL { get; set; }
        [DataMember(Order = 230)]
        public string CRZ58L11C { get; set; }
        [DataMember(Order = 240)]
        public string CRZ58L12C { get; set; }
        [DataMember(Order = 250)]
        public string CRZ58L1PX { get; set; }
        [DataMember(Order = 260)]
        public decimal CRZ58L1BDM { get; set; }
        [DataMember(Order = 280)]
        public decimal CRZ58L1BNM { get; set; }
        [DataMember(Order = 290)]
        public decimal CRZ58L1BDN { get; set; }
        [DataMember(Order = 300)]
        public decimal CRZ58L1LDM { get; set; }
        [DataMember(Order = 310)]
        public decimal CRZ58L1LNM { get; set; }
        [DataMember(Order = 320)]
        public decimal CRZ58L1LDN { get; set; }
        [DataMember(Order = 330)]
        public decimal CRZ58L1RDM { get; set; }
        [DataMember(Order = 340)]
        public decimal CRZ58L1RNM { get; set; }
        [DataMember(Order = 350)]
        public decimal CRZ58L1RDN { get; set; }
        [DataMember(Order = 360)]
        public decimal CRZ58L1TDM { get; set; }
        [DataMember(Order = 370)]
        public decimal CRZ58L1TNM { get; set; }
        [DataMember(Order = 380)]
        public decimal CRZ58L1TDN { get; set; }
        [DataMember(Order = 390)]
        public string CRZ58SHAPE { get; set; }

        [DataMember(Order = 500)]
        public string IBGLPT { get; set; }

        [DataMember(Order = 600)]
        public string S7EDBT { get; set; }
        [DataMember(Order = 610)]
        public decimal S7ITM { get; set; }
        [DataMember(Order = 620)]
        public decimal S7Z58SCN { get; set; }

        [DataMember(Order = 700)]
        public string IBSRP4 { get; set; }
    }
}
