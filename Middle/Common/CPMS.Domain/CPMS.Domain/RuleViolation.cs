using System;

namespace CPMS.Domain
{
    public class RuleViolation
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public Period Period { get; set; }
    }
}
