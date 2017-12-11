using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class EventCounterInfo
    {
        public EventCode EventCode { get; set; }

        public int BreachedEventsNumber { get; set; }

        public int BreachedPlannedEventsNumber { get; set; }

        public int TotalEventsNumber { get; set; }

        public int TotalPlannedEventsNumber { get; set; }
    }
}
