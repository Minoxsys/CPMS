using System;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class PeriodAndEventBreachesCountReportService
    {
        private readonly IClock _clock;

        private const int ThreeDays = 3;
        private const int TenDays = 10;
        private const int OneWeek = 7;
        private const int ThreeWeeks = 21;

        public PeriodAndEventBreachesCountReportService(IClock clock)
        {
            _clock = clock;
        }

        public Func<EventMilestone, bool> GetFilterForEventBreaches(int daysToBreach)
        {
            if (daysToBreach < 0)
            {
                // Breached
                return eventMilestone =>
                {
                    var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                    return daysToBreachAt != null &&
                        eventMilestone.BreachDate != null &&
                        (int)daysToBreachAt < 0;
                };
            }

            if (daysToBreach == TenDays)
            {
                // About to breach
                return eventMilestone =>
                {
                    var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                    return daysToBreachAt != null &&
                        eventMilestone.BreachDate != null &&
                        (int)daysToBreachAt > ThreeDays &&
                        (int)daysToBreachAt <= TenDays;
                };
            }

            return eventMilestone =>
            {
                var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                return daysToBreachAt != null &&
                    eventMilestone.BreachDate != null &&
                    (int)daysToBreachAt == daysToBreach;
            };
        }

        public Func<CompletedEvent, bool> GetFilterForPeriodBreaches(int weeksToBreach)
        {
            if (weeksToBreach < 0)
            {
                // Breached
                return completedEvent => completedEvent.Period.GetDaysRemainingAt(_clock.Today) < 0 &&
                                 completedEvent.Period.GetDaysRemainingAt(_clock.Today) >= -ThreeWeeks;
            }

            return completedEvent => ((completedEvent.Period.GetDaysRemainingAt(_clock.Today) == 0 && weeksToBreach == 1) ||
                                    (completedEvent.Period.GetDaysRemainingAt(_clock.Today) > (weeksToBreach - 1) * OneWeek &&
                                    completedEvent.Period.GetDaysRemainingAt(_clock.Today) <= weeksToBreach * OneWeek));
        }
    }
}
