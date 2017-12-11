using System.Collections.Generic;

namespace CPMS.Report.Manager
{
    public class EventBreachesInfo
    {
        public IEnumerable<EventBreachInfo> EventBreaches { get; set; }

        public int TotalNumberOfEventBreaches { get; set; }
    }
}
