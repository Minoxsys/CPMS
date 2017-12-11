using System.Collections.Generic;

namespace CPMS.Domain
{
    public class Hospital
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Specialty> Specialties { get; set; }
    }
}
