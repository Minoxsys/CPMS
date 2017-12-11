using System.Collections.Generic;

namespace CPMS.Patient.Manager
{
    public class PeriodBreachesInfo
    {
        public IEnumerable<PeriodBreachInfo> PeriodsInfo { get; set; }

        public int TotalNumberOfPeriods { get; set; }
    }
}
