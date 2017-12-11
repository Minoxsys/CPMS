using System;

namespace CPMS.Patient.Presentation
{
    public class PeriodViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? StopDate { get; set; }

        public string OrganizationCode { get; set; }

        public string Status { get; set; }

        public string PeriodType { get; set; }

        public bool IsBreached { get; set; }

        public int DaysInPeriod { get; set; }

        public DateTime ExpectedBreachDate { get; set; }
    }
}
