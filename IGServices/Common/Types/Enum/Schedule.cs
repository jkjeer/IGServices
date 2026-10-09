namespace IGServices.Common.Types.Enum
{
    public class Schedule
    {
        public enum MTOScheduleEnvironmentEnum
        {
            Fuse,
            Karl,
            Both
        };

        public enum StatusUpdateEvents
        {
            Reset,
            Cancel,
            Success,
            Complete
        };

        public enum StatusUpdateEvent
        {
            Default,
            Reset,
            Cancel,
            Success,
            Complete
        };
    }
}
