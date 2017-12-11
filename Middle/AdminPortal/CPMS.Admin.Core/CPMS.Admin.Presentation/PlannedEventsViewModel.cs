using System.Collections.Generic;

namespace CPMS.Admin.Presentation
{
    public class PlannedEventsViewModel
    {
        public IEnumerable<PlannedEventViewModel> PlannedEvents { get; set; }

        public int TotalNumberOfEvents { get; set; }
    }
}
