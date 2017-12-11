using System.Collections.Generic;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class BreachFilterInputInfo
    {
        public string NhsNumber { get; set; }

        public string PatientName { get; set; }

        public List<EventCode> EventsCode { get; set; }

        public string EventCodeFilterValue { get; set; }

        public string Specialty { get; set; }

        public string Clinician { get; set; }

        public string Hospital { get; set; }

        public string PeriodType { get; set; }
        
        public int? AdvancementOrDelay { get; set; }
    }
}
