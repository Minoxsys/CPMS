using System.Collections.Generic;

namespace CPMS.Patient.Manager
{
    public class PatientsInfo
    {
        public IEnumerable<PatientInfo> PatientInfo { get; set; }

        public int TotalNumberOfPatients { get; set; }
    }
}
