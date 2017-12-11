namespace CPMS.Patient.Presentation
{
    public class PeriodEventsFilterInputModel
    {
        public string EventDescription { get; set; }

        public string Specialty { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }

        public int? Breaches { get; set; }

        public string PeriodType { get; set; }
    }
}
