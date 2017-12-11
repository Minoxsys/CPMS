using System;

namespace CPMS.Notification.Presentation
{
    public class RuleViolationViewModel
    {
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PpiNumber { get; set; }

        public int? PeriodId { get; set; }

        public string NhsNumber { get; set; }
    }
}