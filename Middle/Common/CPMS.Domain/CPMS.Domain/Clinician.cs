namespace CPMS.Domain
{
    public class Clinician
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Hospital Hospital { get; set; }

        public Specialty Specialty { get; set; }
    }
}
