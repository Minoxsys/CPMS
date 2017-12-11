using System;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class PeriodInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? StopDate { get; set; }

        public string OrganizationCode { get; set; }

        public PeriodStatus Status { get; set; }

        public string PeriodType { get; set; }

        public bool IsBreached { get; set; }

        public int DaysInPeriod { get; set; }

        public DateTime ExpectedBreachDate { get; set; }
    }
}
