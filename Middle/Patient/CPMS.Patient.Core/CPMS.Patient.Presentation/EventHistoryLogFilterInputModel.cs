namespace CPMS.Patient.Presentation
{
    public class EventHistoryLogFilterInputModel
    {
        public int? ImportYear { get; set; }

        public string EventCode { get; set; }

        public string Description { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }
    }
}
