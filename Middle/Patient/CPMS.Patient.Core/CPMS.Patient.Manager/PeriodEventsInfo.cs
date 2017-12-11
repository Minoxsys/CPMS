using System.Collections.Generic;

namespace CPMS.Patient.Manager
{
    public class PeriodEventsInfo
    {
        public IEnumerable<PeriodEventInfo> Events { get; set; }

        public int TotalNumberOfEvents { get; set; }

        public bool IsBreached { get; set; }
    }
}
