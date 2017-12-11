using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Notification.Manager
{
    public class NotificationApplicationService
    {
        private readonly IClock _clock;
        private readonly IRuleViolationRepository _ruleViolationRepository;
        private readonly IEventMilestoneRepository _eventMilestoneRepository;
        private readonly ICompletedEventRepository _completedEventRepository;

        private const int OneWeek = 7;
        private const int PostBreach = -1;

        public NotificationApplicationService(
            IClock clock,
            IRuleViolationRepository ruleViolationRepository,
            IEventMilestoneRepository eventMilestoneRepository,
            ICompletedEventRepository completedEventRepository)
        {
            _clock = clock;
            _ruleViolationRepository = ruleViolationRepository;
            _eventMilestoneRepository = eventMilestoneRepository;
            _completedEventRepository = completedEventRepository;
        }

        public virtual BreachesInfo GetBreaches()
        {
            var eventMilestones =
                _eventMilestoneRepository.Get(
                eventMilestone => eventMilestone.CompletedEvent.IsActive &&
                eventMilestone.IsMandatory &&
                eventMilestone.CompletedEvent.Period.ShouldCountForBreaches,
                eventMilestone => eventMilestone.Name,
                eventMilestone => eventMilestone.CompletedEvent.Period.CompletedEvents,
                eventMilestone => eventMilestone.CompletedEvent.Period.Pathway.Patient).ToArray();
            var completedEvents = _completedEventRepository.Get(
                completedEvent => completedEvent.IsActive &&
                completedEvent.Period.ShouldCountForBreaches,
                completedEvent => completedEvent.Name,
                completedEvent => completedEvent.Period.CompletedEvents,
                completedEvent => completedEvent.Period.Pathway.Patient).ToArray();

            var eventBreachesInfo = GetEventBreachesInfo(eventMilestones, 0)
                    .Concat(GetEventBreachesInfo(eventMilestones, 1))
                    .Concat(GetEventBreachesInfo(eventMilestones, 2))
                    .Concat(GetEventBreachesInfo(eventMilestones, 3))
                    .Concat(GetEventBreachesInfo(eventMilestones, -1))
                    .ToArray();
            var periodBreachedInfo = GetPeriodBreachesInfo(completedEvents, 0)
                .Concat(GetPeriodBreachesInfo(completedEvents, 1))
                .Concat(GetPeriodBreachesInfo(completedEvents, 2))
                .Concat(GetPeriodBreachesInfo(completedEvents, 3))
                .ToArray();

            return new BreachesInfo
            {
                EventsBreaches = eventBreachesInfo,
                PeriodsBreaches = periodBreachedInfo
            };
        }

        public virtual IEnumerable<RuleViolationInfo> GetRuleViolations()
        {
            var yesterdayClock = _clock.TodayDateAndTime.AddDays(-1);

            return _ruleViolationRepository.Get(
                ruleViolation => ruleViolation.CreatedAt >= yesterdayClock,
                ruleViolation => ruleViolation.Period.Pathway.Patient)
                .Select(ruleViolation => new RuleViolationInfo
                {
                    Message = ruleViolation.Message,
                    CreatedAt = ruleViolation.CreatedAt,
                    PeriodId = ruleViolation.Period == null ? (int?)null : ruleViolation.Period.Id,
                    PpiNumber = ruleViolation.Period == null ? null : ruleViolation.Period.Pathway.PPINumber,
                    NhsNumber = ruleViolation.Period == null ? null : ruleViolation.Period.Pathway.Patient.NHSNumber
                });
        }

        private IEnumerable<EventBreachInfo> GetEventBreachesInfo(IEnumerable<EventMilestone> eventMilestones, int daysToBreach)
        {
            var filteredEventMilestones = eventMilestones.Where(GetFilterForEventBreaches(daysToBreach)).ToArray();

            return (
                from eventMilestone in filteredEventMilestones
                let statusDays = eventMilestone.BreachStatusAt(_clock.TodayDate)
                let postBreachDays = eventMilestone.GetDaysToBreachAt(_clock.TodayDate)
                select new EventBreachInfo
                {
                    EventDescription = eventMilestone.Name.Description,
                    NhsNumber = eventMilestone.CompletedEvent.Period.Pathway.Patient.NHSNumber,
                    PatientName = eventMilestone.CompletedEvent.Period.Pathway.Patient.Name,
                    PpiNumber = eventMilestone.CompletedEvent.Period.Pathway.PPINumber,
                    PeriodId = eventMilestone.CompletedEvent.Period.Id,
                    Status = statusDays,
                    DaysForStatus = daysToBreach == PostBreach ? (Math.Abs((int)postBreachDays)) : daysToBreach
                });
        }

        private Func<EventMilestone, bool> GetFilterForEventBreaches(int daysToBreach)
        {
            if (daysToBreach < 0)
            {
                // Breached
                return eventMilestone =>
                {
                    var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.TodayDate);
                    return daysToBreachAt != null && eventMilestone.BreachDate != null && daysToBreachAt < 0;
                };
            }

            return eventMilestone =>
            {
                var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.TodayDate);
                return daysToBreachAt != null && eventMilestone.BreachDate != null && daysToBreachAt == daysToBreach;
            };
        }

        private IEnumerable<PeriodBreachInfo> GetPeriodBreachesInfo(IEnumerable<CompletedEvent> completedEvents, int weeksToBreach)
        {
            var filteredCompletedEvents = completedEvents.Where(GetFilterForPeriodBreaches(weeksToBreach)).ToArray();

            return (from completedEvent in filteredCompletedEvents
                    let daysRemaining = completedEvent.Period.GetDaysRemainingAt(_clock.TodayDate)
                    orderby daysRemaining
                    select new PeriodBreachInfo
                    {
                        PeriodName = completedEvent.Period.Name,
                        NhsNumber = completedEvent.Period.Pathway.Patient.NHSNumber,
                        PatientName = completedEvent.Period.Pathway.Patient.Name,
                        PpiNumber = completedEvent.Period.Pathway.PPINumber,
                        PeriodId = completedEvent.Period.Id,
                        DaysToBreach = daysRemaining
                    });
        }

        private Func<CompletedEvent, bool> GetFilterForPeriodBreaches(int weeksToBreach)
        {
            if (weeksToBreach == 0)
            {
                return completedEvent => completedEvent.Period.GetDaysRemainingAt(_clock.TodayDate) == 0;
            }

            return completedEvent => completedEvent.Period.GetDaysRemainingAt(_clock.TodayDate) > (weeksToBreach - 1) * OneWeek &&
                                    completedEvent.Period.GetDaysRemainingAt(_clock.TodayDate) <= weeksToBreach * OneWeek;
        }
    }
}
