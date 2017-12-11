using System.Collections.Generic;

namespace CPMS.Patient.Presentation
{
    public class EventBreachesViewModel
    {
        public IEnumerable<EventBreachViewModel> EventsInfo { get; set; } 

        public int TotalNumberOfPlannedEvents { get; set; }
    }
}
