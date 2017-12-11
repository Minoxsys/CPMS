namespace CPMS.Patient.Presentation
{
    public class PatientFilterInputModel
    {
        public string PatientName { get; set; }

        public string Hospital { get; set; }

        public string PpiNumber { get; set; }

        public string NhsNumber { get; set; }

        public string PeriodType { get; set; }
    }
}
