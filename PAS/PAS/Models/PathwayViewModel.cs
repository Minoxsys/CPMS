using CPMS.Patient.Domain;

namespace PAS.Models
{
    public class PathwayViewModel
    {
        public string PPINumber { get; set; }

        public PathwayType Type { get; set; }

        public string NHSNumber { get; set; }

        public string OrganizationCode { get; set; }
    }
}