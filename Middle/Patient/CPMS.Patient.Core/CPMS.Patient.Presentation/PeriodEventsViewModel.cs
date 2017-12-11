using System.Collections.Generic;

namespace CPMS.Patient.Presentation
{
    public class PeriodEventsViewModel
    {
        public IEnumerable<PeriodEventViewModel> Events { get; set; }

        public int TotalNumberOfEvents { get; set; }

        public bool IsBreached { get; set; }
    }
}
