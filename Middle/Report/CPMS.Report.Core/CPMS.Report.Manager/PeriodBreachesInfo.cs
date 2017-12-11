using System.Collections.Generic;

namespace CPMS.Report.Manager
{
    public class PeriodBreachesInfo
    {
        public IEnumerable<PeriodBreachInfo> PeriodsInfo { get; set; }

        public int TotalNumberOfPeriodBreaches { get; set; }
    }
}
