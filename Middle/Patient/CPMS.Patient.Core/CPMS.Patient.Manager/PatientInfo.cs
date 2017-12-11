using System;

namespace CPMS.Patient.Manager
{
    public class PatientInfo
    {
        public string NHSNumber { get; set; }

        public string PatientName { get; set; }

        public string Hospital { get; set; }

        public string PpiNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }

        public int PeriodId { get; set; }
    }
}
