
namespace CPMS.Report.Manager
{
    public class ActivePeriodInfo
    {
        public string Hospital { get; set; }

        public int HospitalId { get; set; }

        public string Specialty { get; set; }

        public string SpecialtyCode { get; set; }

        public string Clinician { get; set; }

        public int ClinicianId { get; set; }

        public int NumberOfActivePeriods { get; set; }

        public int Week { get; set; }
    }
}
