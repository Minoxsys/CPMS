﻿
namespace CPMS.Report.Presentation
{
    public class Monthly18wRTTPerformanceViewModel
    {
        public string Hospital { get; set; }

        public string Specialty { get; set; }

        public string Clinician { get; set; }

        public int InpatientBreachedPeriodsNumber { get; set; }

        public int InpatientCompletedPeriodsNumber { get; set; }

        public int OutpatientBreachedPeriodsNumber { get; set; }

        public int OutpatientCompletedPeriodsNumber { get; set; }

        public int OpenedBreachedPeriodsNumber { get; set; }

        public int OpenedPeriodsNumber { get; set; }

        public int PeriodsNumber { get; set; }

        public int BreachedPeriodsNumber { get; set; }

        public string Month { get; set; }
    }
}
