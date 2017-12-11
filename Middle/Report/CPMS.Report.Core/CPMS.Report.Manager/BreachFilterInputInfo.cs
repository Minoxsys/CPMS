namespace CPMS.Report.Manager
{
    public class BreachFilterInputInfo
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public string EventDescription { get; set; }

        public string Specialty { get; set; }

        public string Clinician { get; set; }

        public string Hospital { get; set; }

        public string PeriodType { get; set; }
        
        public int? AdvancementOrDelay { get; set; }
    }
}
