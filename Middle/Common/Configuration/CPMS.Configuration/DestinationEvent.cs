using CPMS.Domain;

namespace CPMS.Configuration
{
    public class DestinationEvent
    {
        public int Id { get; set; }

        public EventName DestinationName { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public EventName EventForDateReferenceForTarget { get; set; }

        public ClockType ClockType { get; set; }
    }
}
