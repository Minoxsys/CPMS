using System.Collections.Generic;

namespace CPMS.Report.Presentation
{
    public class EventBreachesViewModel
    {
        public IEnumerable<EventBreachViewModel> EventBreaches { get; set; }

        public int TotalNumberOfEventBreaches { get; set; }
    }
}
