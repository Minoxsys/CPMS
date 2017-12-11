using System;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class EventHistoryLogInfo
    {
        public EventCode EventCode { get; set; }

        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime ActualDate { get; set; }

        public DateTime ImportDate { get; set; }
    }
}
