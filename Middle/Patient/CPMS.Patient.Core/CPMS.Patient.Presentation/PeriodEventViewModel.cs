using System;

namespace CPMS.Patient.Presentation
{
    public class PeriodEventViewModel
    {
        public string EventCode { get; set; }

        public string StatusCode { get; set; }

        public string Specialty { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public string EventStatus { get; set; }

        public string BreachStatus { get; set; }

        public int? DaysPostbreach { get; set; }

        public string PeriodType { get; set; }
    }
}
