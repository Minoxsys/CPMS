using System.Collections.Generic;

namespace CPMS.Patient.Presentation
{
    public class PeriodBreachesViewModel
    {
        public IEnumerable<PeriodBreachViewModel> PeriodsInfo { get; set; }

        public int TotalNumberOfPeriods { get; set; }
    }
}
