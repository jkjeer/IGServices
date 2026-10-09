using System;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class RetrievalTimes
    {
        [DataMember(Order = 1)]
        public DateTime StartTimeJob { get; set; }
        [DataMember(Order = 2)]
        public DateTime EndTimeJob { get; set; }
        [DataMember(Order = 3)]
        public decimal ElapsedSecondsJob { get; set; }

        [DataMember(Order = 10)]
        public DateTime StartTimeRetrieveSF { get; set; }
        [DataMember(Order = 11)]
        public DateTime EndTimeRetrieveSF { get; set; }
        [DataMember(Order = 12)]
        public decimal ElapsedSecondsRetrieveSF { get; set; }

        [DataMember(Order = 20)]
        public DateTime StartTimeRetrieveSFInt { get; set; }
        [DataMember(Order = 21)]
        public DateTime EndTimeRetrieveSFInt { get; set; }
        [DataMember(Order = 22)]
        public decimal ElapsedSecondsRetrieveSFInt { get; set; }

        [DataMember(Order = 30)]
        public DateTime StartTimeValidate { get; set; }
        [DataMember(Order = 31)]
        public DateTime EndTimeValidate { get; set; }
        [DataMember(Order = 32)]
        public decimal ElapsedSecondsValidate { get; set; }

        [DataMember(Order = 40)]
        public DateTime StartTimeRetrieveE1 { get; set; }
        [DataMember(Order = 41)]
        public DateTime EndTimeRetrieveE1 { get; set; }
        [DataMember(Order = 42)]
        public decimal ElapsedSecondsRetrieveE1 { get; set; }
    }
}
