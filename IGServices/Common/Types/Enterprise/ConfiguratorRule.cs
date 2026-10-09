using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class ConfiguratorRules : List<ConfiguratorRule>
    {
    }

    [DataContract(Namespace = "")]
    public class ConfiguratorRule
    {
        [DataMember(Order = 1)]
        public string RulesTableType { get; set; }
        [DataMember(Order = 2)]
        public string RulesTableName { get; set; }
        [DataMember(Order = 3)]
        public string CostCenter { get; set; }
        [DataMember(Order = 4)]
        public string ConfiguredTableKeyID { get; set; }
        [DataMember(Order = 5)]
        public string TableSegmentValue1 { get; set; }
        [DataMember(Order = 6)]
        public string TableSegmentValue2 { get; set; }
        [DataMember(Order = 7)]
        public string TableSegmentValue3 { get; set; }
        [DataMember(Order = 8)]
        public string TableSegmentValue4 { get; set; }
        [DataMember(Order = 9)]
        public string TableSegmentValue5 { get; set; }
        [DataMember(Order = 10)]
        public string TableSegmentValue6 { get; set; }
        [DataMember(Order = 11)]
        public string TableSegmentValue7 { get; set; }
        [DataMember(Order = 12)]
        public string TableSegmentValue8 { get; set; }
        [DataMember(Order = 13)]
        public string TableSegmentValue9 { get; set; }
        [DataMember(Order = 14)]
        public string TableSegmentValue0 { get; set; }
        [DataMember(Order = 15)]
        public string TableSegmentValue11 { get; set; }
        [DataMember(Order = 16)]
        public string TableSegmentValue12 { get; set; }
        [DataMember(Order = 17)]
        public string TableSegmentValue13 { get; set; }
        [DataMember(Order = 18)]
        public string TableSegmentValue14 { get; set; }
        [DataMember(Order = 19)]
        public string TableSegmentValue15 { get; set; }
        [DataMember(Order = 20)]
        public string TableSegmentValue16 { get; set; }
        [DataMember(Order = 21)]
        public string TableSegmentValue17 { get; set; }
        [DataMember(Order = 22)]
        public string TableSegmentValue18 { get; set; }
        [DataMember(Order = 23)]
        public string TableSegmentValue19 { get; set; }
        [DataMember(Order = 24)]
        public string TableSegmentValue20 { get; set; }
        [DataMember(Order = 25)]
        public decimal BubbleSequence { get; set; }
        [DataMember(Order = 26)]
        public decimal EffectiveFromDate { get; set; }
        [DataMember(Order = 27)]
        public decimal EffectiveThruDate { get; set; }
        [DataMember(Order = 28)]
        public string RulesTableValue { get; set; }
        [DataMember(Order = 29)]
        public decimal UnitPriceEntered { get; set; }
        [DataMember(Order = 30)]
        public decimal IdentifierShortItem { get; set; }
        [DataMember(Order = 31)]
        public decimal QtyRequiredStandard { get; set; }
        [DataMember(Order = 32)]
        public string UnitOfMeasure { get; set; }
        [DataMember(Order = 33)]
        public string UserReservedCode { get; set; }
        [DataMember(Order = 34)]
        public decimal UserReservedDate { get; set; }
        [DataMember(Order = 35)]
        public decimal UserReservedAmount { get; set; }
        [DataMember(Order = 36)]
        public decimal UserReservedNumber { get; set; }
        [DataMember(Order = 37)]
        public string UserReservedReference { get; set; }
        [DataMember(Order = 38)]
        public string UserId { get; set; }
        [DataMember(Order = 39)]
        public string ProgramId { get; set; }
        [DataMember(Order = 40)]
        public string WorkStationId { get; set; }
        [DataMember(Order = 100)]
        public DateTime UpdateDateTime { get; set; }
    }
}
