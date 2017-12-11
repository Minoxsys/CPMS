using System.Collections.Generic;

namespace CPMS.Patient.Manager
{
    public class EventBreachesInfo
    {
        public IEnumerable<EventBreachInfo> EventsInfo { get; set; }

        public int TotalNumberOfPlannedEvents { get; set; }
    }
}
