namespace CPMS.Patient.Presentation
{
    public class PeriodBreachViewModel
    {
        public string PatientNHSNumber { get; set; }

        public string PatientName { get; set; }

        public string EventCode { get; set; }

        public string Specialty { get; set; }

        public string Clinician { get; set; }

        public string PPINumber { get; set; }

        public int PostBreachDays { get; set; }

        public int DaysInPeriod { get; set; }

        public int DaysRemainingInPeriod { get; set; }

        public string PeriodType { get; set; }

        public string EventType { get; set; }

        public int PeriodId { get; set; }
    }
}
