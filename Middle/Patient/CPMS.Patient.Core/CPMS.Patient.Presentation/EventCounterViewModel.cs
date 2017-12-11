namespace CPMS.Patient.Presentation
{
    public class EventCounterViewModel
    {
        public string EventCode { get; set; }
        
        public int BreachedEventsNumber { get; set; }

        public int BreachedPlannedEventsNumber { get; set; }

        public int TotalEventsNumber { get; set; }

        public int TotalPlannedEventsNumber { get; set; }
    }
}
