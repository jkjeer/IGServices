using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IGServices.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class F580022s : List<F580022>
    {
    }

    [DataContract(Namespace = "")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class F580022
    {
        [DataMember(Order = 1)]
        public string CAEDUS { get; set; }
        [DataMember(Order = 2)]
        public string CAEDBT { get; set; }
        [DataMember(Order = 3)]
        public string CAEDTN { get; set; }
        [DataMember(Order = 4)]
        public decimal CAEDLN { get; set; }
        [DataMember(Order = 5)]
        public string CAEDCT { get; set; }
        [DataMember(Order = 6)]
        public string CATYTN { get; set; }
        [DataMember(Order = 7)]
        public string CAEDFT { get; set; }
        [DataMember(Order = 8)]
        public decimal CAEDDT { get; set; }
        [DataMember(Order = 9)]
        public string CADRIN { get; set; }
        [DataMember(Order = 10)]
        public decimal CAEDDL { get; set; }
        [DataMember(Order = 11)]
        public string CAEDSP { get; set; }
        [DataMember(Order = 12)]
        public string CAPNID { get; set; }
        [DataMember(Order = 13)]
        public string CATNAC { get; set; }
        [DataMember(Order = 14)]
        public string CAYST { get; set; }
        [DataMember(Order = 15)]
        public string CAMMCU { get; set; }
        [DataMember(Order = 16)]
        public decimal CADOCO { get; set; }
        [DataMember(Order = 17)]
        public string CADCTO { get; set; }
        [DataMember(Order = 18)]
        public string CALOTN { get; set; }
        [DataMember(Order = 19)]
        public string CAMCU { get; set; }
        [DataMember(Order = 20)]
        public string CAZ58RT { get; set; }
        [DataMember(Order = 21)]
        public decimal CAFRMPAK { get; set; }
        [DataMember(Order = 22)]
        public decimal CA75TLID { get; set; }
        [DataMember(Order = 23)]
        public decimal CAANPA { get; set; }
        [DataMember(Order = 24)]
        public string CASFT { get; set; }
        [DataMember(Order = 25)]
        public decimal CASOQS { get; set; }
        [DataMember(Order = 26)]
        public string CAUOM { get; set; }
        [DataMember(Order = 27)]
        public decimal CAZ58SCN { get; set; }
        [DataMember(Order = 28)]
        public string CAURCD { get; set; }
        [DataMember(Order = 29)]
        public decimal CAURDT { get; set; }
        [DataMember(Order = 30)]
        public decimal CAURAT { get; set; }
        [DataMember(Order = 31)]
        public decimal CAURAB { get; set; }
        [DataMember(Order = 32)]
        public string CAURRF { get; set; }
        [DataMember(Order = 33)]
        public string CAUSER { get; set; }
        [DataMember(Order = 34)]
        public string CAPID { get; set; }
        [DataMember(Order = 35)]
        public string CAJOBN { get; set; }
        [DataMember(Order = 36)]
        public decimal CAUPMJ { get; set; }
        [DataMember(Order = 37)]
        public decimal CATDAY { get; set; }
        [DataMember(Order = 38)]
        public string CAZ58IP01 { get; set; }
    }
}
