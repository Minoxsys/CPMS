using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public class PlannedEventInfo
    {
        public int PlannedEventId { get; set; }

        public ConfigurationEventCode PlannedEventCode { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public ConfigurationEventCode ParentEventCode { get; set; }

        public ConfigurationEventCode? EventForDateReferenceForTarget { get; set; }

        public ClockType ClockType { get; set; }
    }
}
