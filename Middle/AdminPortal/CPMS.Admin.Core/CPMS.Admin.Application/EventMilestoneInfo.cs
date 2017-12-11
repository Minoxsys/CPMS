using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public class EventMilestoneInfo
    {
        public int EventMilestoneId { get; set; }

        public string EventMilestoneDescription { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public string ParentEventDescription { get; set; }

        public string EventForDateReferenceForTarget { get; set; }

        public ClockType ClockType { get; set; }
    }
}
