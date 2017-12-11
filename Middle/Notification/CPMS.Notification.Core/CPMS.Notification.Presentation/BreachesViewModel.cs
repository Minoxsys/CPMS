using System.Collections.Generic;

namespace CPMS.Notification.Presentation
{
    public class BreachesViewModel
    {
        public IEnumerable<EventBreachViewModel> EventsBreaches { get; set; }

        public IEnumerable<PeriodBreachViewModel> PeriodsBreaches { get; set; }
    }
}
