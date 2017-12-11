using CPMS.Configuration;

namespace GenerateData
{
    public class EventConnection
    {
        public ConfigurationEventCode Parent { get; set; }
        public ConfigurationEventCode Child { get; set; }
        public ConfigurationEventCode? EventForDateReferenceForTarget { get; set; }
        public int? TargetNumberOfDays { get; set; }
    }
}
