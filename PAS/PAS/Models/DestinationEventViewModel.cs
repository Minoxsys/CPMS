namespace PAS.Models
{
    public class DestinationEventViewModel
    {
        public int DestinationEventId { get; set; }

        public string DestinationEventCode { get; set; }

        public int? TargetNumberOfDays { get; set; }

        public bool IsMandatory { get; set; }

        public string SourceEventCode { get; set; }

        public string EventForDateReferenceForTarget { get; set; }
    }
}