namespace CPMS.Patient.Presentation
{
    public class BreachFilterInputModel
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public string EventCode { get; set; }

        public string Specialty { get; set; }

        public string Hospital { get; set; }

        public string Clinician { get; set; }

        public string PeriodType { get; set; }

        public int? AdvancementOrDelay { get; set; }
    }
}
