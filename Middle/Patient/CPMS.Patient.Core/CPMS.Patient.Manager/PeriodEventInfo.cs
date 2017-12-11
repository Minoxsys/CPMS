using System;
using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public class PeriodEventInfo
    {
        public string EventDescription { get; set; }

        public string StatusCode { get; set; }

        public string Specialty { get; set; }
        
        public DateTime? TargetDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public EventStatus? EventStatus { get; set; }

        public EventBreachStatus? BreachStatus { get; set; }

        public int? DaysPostbreach { get; set; }

        public string PeriodType { get; set; }
    }
}
