using System.Collections.Generic;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class EventHistoryLogFilterInputInfo
    {
        public List<EventCode> EventsCode { get; set; }

        public string EventCodeFilterValue { get; set; }

        public int? ImportYear { get; set; }

        public string Description { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }
    }
}
