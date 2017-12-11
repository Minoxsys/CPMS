using System;

namespace CPMS.Patient.Presentation
{
    public class EventHistoryLogViewModel
    {
        public string EventCode { get; set; }

        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime ActualDate { get; set; }

        public DateTime ImportDate { get; set; }
    }
}
