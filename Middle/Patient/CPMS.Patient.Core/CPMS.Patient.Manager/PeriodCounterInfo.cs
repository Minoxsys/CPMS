namespace CPMS.Patient.Manager
{
    public class PeriodCounterInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int AboutToBreachNumber { get; set; }

        public int BreachedNumber { get; set; }

        public int OnTrackNumber { get; set; }
    }
}
