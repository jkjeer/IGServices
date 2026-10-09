using System;

namespace IGServices.Common.Utilities
{
    public class E1TimeSpan
    {
        public decimal DateComponent1 { get; set; }
        public decimal TimeComponent1 { get; set; }
        public decimal DateComponent2 { get; set; }
        public decimal DateComponent3 { get; set; }

        public E1TimeSpan()
        {
            DateComponent1 = 0;
            TimeComponent1 = 0;
            DateComponent2 = 0;
            DateComponent3 = 0;
        }

        public void SetTimeSpan(TimeSpan ts)
        {
            var nd = DateTime.Now.Subtract(ts);
            DateComponent1 = Common.CastToE1Date(nd);
            TimeComponent1 = Common.CastToE1Time(nd);
            DateComponent2 = Common.CastToE1Date(nd.AddDays(1));
            DateComponent3 = Common.CastToE1Date(DateTime.Now);
        }

        public void SetTimeSpan(DateTime nd)
        {
            DateComponent1 = Common.CastToE1Date(nd);
            TimeComponent1 = Common.CastToE1Time(nd);
            DateComponent2 = Common.CastToE1Date(nd.AddDays(1));
            DateComponent3 = Common.CastToE1Date(DateTime.Now);
            if (DateComponent1 < 0) { DateComponent1 = 0; }
            if (TimeComponent1 < 0) { TimeComponent1 = 0; }
            if (DateComponent2 < 0) { DateComponent2 = 0; }
            if (DateComponent3 < 0) { DateComponent3 = 0; }
        }

    }
}
