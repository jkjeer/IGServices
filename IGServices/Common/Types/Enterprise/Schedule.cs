using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;

namespace IGGServices.Common.Types.Enterprise
{
    [CollectionDataContract(Namespace = "")]
    public class Schedules : List<Schedule>
    {
    }

    [DataContract(Namespace = "")]
    public class Schedule
    {

        public Schedule()
        {
            f580021s = new F580021s();
            f580024s = new F580024s();
            f580220s = new F580220s();
            f580232s = new F580232s();
            f4715s = new F4715s();
            f584715s = new F584715s();
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public Plant Plant { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public long ScheduleId { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public decimal ScheduleDateE1 { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public DateTime? ScheduleDate { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public MTOScheduleEvent MTOScheduleEvent { get; set; }

        [DataMember(Order = 51, EmitDefaultValue = false)]
        public ScheduleEvent ScheduleEvent { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public ScheduleLines ScheduleLines { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public F580020 f580020 { get; set; }

        [DataMember(Order = 210, EmitDefaultValue = false)]
        public F5834A01 f5834A01 { get; set; }

        [DataMember(Order = 220, EmitDefaultValue = false)]
        public F580021s f580021s { get; set; }

        [DataMember(Order = 230, EmitDefaultValue = false)]
        public F580024s f580024s { get; set; }

        [DataMember(Order = 240, EmitDefaultValue = false)]
        public F580220s f580220s { get; set; }

        [DataMember(Order = 250, EmitDefaultValue = false)]
        public F580232s f580232s { get; set; }

        [DataMember(Order = 260, EmitDefaultValue = false)]
        public F4715s f4715s { get; set; }

        [DataMember(Order = 262, EmitDefaultValue = false)]
        public F584715s f584715s { get; set; }
    }
}
