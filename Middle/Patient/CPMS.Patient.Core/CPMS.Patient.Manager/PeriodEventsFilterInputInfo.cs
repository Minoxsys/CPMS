using System.Collections.Generic;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class PeriodEventsFilterInputInfo
    {
        public List<EventCode> EventsCode { get; set; }

        public string EventCodeFilterValue { get; set; }

        public string Specialty { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }

        public int? Breaches { get; set; }

        public string PeriodType { get; set; }
    }
}
