
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F580232s : List<F580232>
    {
    }

    [DataContract(Namespace = "")]
    public class F580232
    {
        [DataMember(Order = 10)] public string BDMCU { get; set; }
        [DataMember(Order = 20)] public decimal BDZ58JOBID { get; set; }
        [DataMember(Order = 30)] public decimal BDZ58BNUM { get; set; }
        [DataMember(Order = 40)] public decimal BDZ58WODC { get; set; }
        [DataMember(Order = 50)] public string BDZ58WOTY { get; set; }
        [DataMember(Order = 60)] public decimal BDDOCO { get; set; }
        [DataMember(Order = 70)] public string BDDCTO { get; set; }
        [DataMember(Order = 80)] public string BDKCOO { get; set; }
        [DataMember(Order = 90)] public decimal BDLNID { get; set; }
        [DataMember(Order = 100)] public string BDZ58SFS { get; set; }
        [DataMember(Order = 110)] public string BDZ58IGLN { get; set; }
        [DataMember(Order = 120)] public string BDZ58FLEX1 { get; set; }
        [DataMember(Order = 130)] public string BDZ58FLEX2 { get; set; }
        [DataMember(Order = 140)] public string BDZ58FLEX3 { get; set; }
        [DataMember(Order = 150)] public string BDZ58FLEX4 { get; set; }
        [DataMember(Order = 160)] public string BDUSER { get; set; }
        [DataMember(Order = 170)] public string BDPID { get; set; }
        [DataMember(Order = 180)] public string BDJOBN { get; set; }
        [DataMember(Order = 190)] public decimal BDUPMJ { get; set; }
        [DataMember(Order = 200)] public decimal BDUPMT { get; set; }
        [DataMember(Order = 210)] public decimal BDSEQ { get; set; }
        [DataMember(Order = 220)] public string BDZ58BGEE { get; set; }
        [DataMember(Order = 230)] public decimal BDZ58BGSZ { get; set; }
        [DataMember(Order = 240)] public string BDZ58BOXP { get; set; }
        [DataMember(Order = 250)] public string BDZ58CCV01 { get; set; }
        [DataMember(Order = 260)] public string BDZ58CCV02 { get; set; }
        [DataMember(Order = 270)] public string BDZ58CCV03 { get; set; }
        [DataMember(Order = 280)] public string BDZ58CCV04 { get; set; }
        [DataMember(Order = 290)] public string BDZ58CCV05 { get; set; }
        [DataMember(Order = 300)] public string BDZ58CCV06 { get; set; }
        [DataMember(Order = 310)] public string BDZ58CCV07 { get; set; }
        [DataMember(Order = 320)] public string BDZ58CCV08 { get; set; }
        [DataMember(Order = 330)] public string BDZ58CCV09 { get; set; }
        [DataMember(Order = 340)] public string BDZ58CCV10 { get; set; }
        [DataMember(Order = 350)] public string BDZ58CCV11 { get; set; }
        [DataMember(Order = 360)] public string BDZ58CCV12 { get; set; }
        [DataMember(Order = 370)] public string BDZ58CCV13 { get; set; }
        [DataMember(Order = 380)] public string BDZ58CCV14 { get; set; }
        [DataMember(Order = 390)] public string BDZ58CCV15 { get; set; }
        [DataMember(Order = 400)] public decimal BDDRQJ { get; set; }
        [DataMember(Order = 410)] public decimal BDZ58CRIT { get; set; }
        [DataMember(Order = 500)] public string BHDS01 { get; set; }
        [DataMember(Order = 510)] public string BHZ58GDES { get; set; }
        [DataMember(Order = 520)] public string BHZ58BDES { get; set; }

        [DataMember(Order = 530)] public string SHDEL1 { get; set; }
        [DataMember(Order = 540)] public string SHDEL2 { get; set; }
    }
}
