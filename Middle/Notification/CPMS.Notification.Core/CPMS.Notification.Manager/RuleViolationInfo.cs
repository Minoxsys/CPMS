using System;

namespace CPMS.Notification.Manager
{
    public class RuleViolationInfo
    {
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PpiNumber { get; set; }

        public int? PeriodId { get; set; }

        public string  NhsNumber { get; set; }

    }
}
