using System.Collections.Generic;

namespace CPMS.Domain
{
    public class Specialty
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public IList<Hospital> Hospitals { get; set; }

        public IList<Clinician> Clinicians { get; set; }
    }
}
