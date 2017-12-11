using System.Collections.Generic;

namespace CPMS.Patient.Presentation
{
    public class PatientsViewModel
    {
        public IEnumerable<PatientViewModel> PatientsInfo { get; set; }

        public int TotalNumberOfPatients { get; set; }
    }
}
