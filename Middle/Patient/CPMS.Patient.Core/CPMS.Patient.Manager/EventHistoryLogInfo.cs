using System;

namespace CPMS.Patient.Manager
{
    public class EventHistoryLogInfo
    {
        public string EventDescription { get; set; }

        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime ActualDate { get; set; }

        public DateTime ImportDate { get; set; }
    }
}
