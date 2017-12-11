using System.Collections.Generic;

namespace CPMS.Notification.Manager
{
    public class BreachesInfo
    {
        public IEnumerable<EventBreachInfo> EventsBreaches { get; set; }

        public IEnumerable<PeriodBreachInfo> PeriodsBreaches { get; set; }
    }
}