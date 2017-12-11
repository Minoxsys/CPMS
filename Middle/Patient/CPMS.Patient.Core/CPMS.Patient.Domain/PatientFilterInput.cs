namespace CPMS.Patient.Domain
{
    public class PatientFilterInput
    {
        public string PatientName { get; set; }

        public string Hospital { get; set; }

        public string PpiNumber { get; set; }

        public string NhsNumber { get; set; }
    }
}
