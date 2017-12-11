using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class PeriodBreachesCounterReportService
    {
        private readonly IClock _clock;

        private const int ThreeWeeks = 21;

        public PeriodBreachesCounterReportService(IClock clock)
        {
            _clock = clock;
        }

        public PeriodPerformanceInfo GetPeriodBreachesCounterInfo(List<CompletedEvent> completedEvents, string name, string id)
        {
            var aboutToBreachNumber = 0;
            var breachedNumber = 0;
            var onTrackNumber = 0;
            var completedNumber = 0;

            var periods = new List<Period>();

            foreach (var completedEvent in completedEvents.Where(completedEvent => periods.All(period => period.Id != completedEvent.Period.Id)))
            {
                periods.Add(completedEvent.Period);
            }

            foreach (var period in periods)
            {
                if (period.IsActive)
                {
                    var daysRemaining = period.GetDaysRemainingAt(_clock.Today);
                    if (daysRemaining >= 0)
                    {
                        if (daysRemaining <= ThreeWeeks)
                        {
                            aboutToBreachNumber++;
                        }
                        else
                        {
                            onTrackNumber++;
                        }
                    }
                    else if (daysRemaining < 0 && daysRemaining >= -ThreeWeeks)
                    {
                        breachedNumber++;
                    }
                }
                if (period.StopDate != null)
                {
                    completedNumber++;
                }
            }

            return (aboutToBreachNumber > 0 || breachedNumber > 0 || onTrackNumber > 0)
                ? new PeriodPerformanceInfo
                {
                    Name = name,
                    Id = id,
                    AboutToBreachNumber = aboutToBreachNumber,
                    BreachedNumber = breachedNumber,
                    OnTrackNumber = onTrackNumber,
                    CompletedNumber = completedNumber
                }
                : null;
        }
    }
}
