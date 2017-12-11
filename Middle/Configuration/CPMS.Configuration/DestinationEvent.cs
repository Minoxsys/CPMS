namespace CPMS.Configuration
{
    public class DestinationEvent
    {
        public int Id { get; set; }

        public ConfigurationEventCode DestinationCode { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public ConfigurationEventCode? EventForDateReferenceForTarget { get; set; }

        public ClockType ClockType { get; set; }
    }
}
