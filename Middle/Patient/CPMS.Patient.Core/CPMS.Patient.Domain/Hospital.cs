using System.Collections.Generic;

namespace CPMS.Patient.Domain
{
    public class Hospital
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Specialty> Specialties { get; set; }
    }
}
