using System.Collections.Generic;

namespace CPMS.Patient.Presentation
{
    public class EventsHistoryLogViewModel
    {
        public IEnumerable<EventHistoryLogViewModel> EventsInfo { get; set; }

        public int TotalNumberOfEvents { get; set; }
    }
}
