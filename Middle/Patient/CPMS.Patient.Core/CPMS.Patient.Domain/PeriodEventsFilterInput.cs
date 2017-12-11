using System.Collections.Generic;

namespace CPMS.Patient.Domain
{
    public class PeriodEventsFilterInput
    {
        public List<EventCode?> EventsCode { get; set; }

        public string EventCodeFilterValue { get; set; }

        public string Specialty { get; set; }

        public int? TargetYear { get; set; }

        public int? ActualYear { get; set; }

        public int? Breaches { get; set; }

        public string PeriodType { get; set; }
    }
}
