using CPMS.Domain;

namespace GenerateData
{
    public class EventConnection
    {
        public EventCode Parent { get; set; }
        public EventCode Child { get; set; }
        public EventCode? EventForDateReferenceForTarget { get; set; }
        public int? TargetNumberOfDays { get; set; }
    }
}
