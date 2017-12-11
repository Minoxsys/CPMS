using System.Collections.Generic;

namespace CPMS.Patient.Manager
{
    public class EventsHistoryLogInfo
    {
        public IEnumerable<EventHistoryLogInfo> EventsInfo { get; set; }

        public int TotalNumberOfEvents { get; set; }
    }
}
