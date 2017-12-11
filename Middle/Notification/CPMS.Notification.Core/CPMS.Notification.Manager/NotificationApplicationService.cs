using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Manager
{
    public class NotificationApplicationService
    {
        private readonly BreachService _breachService;
        private readonly IClock _clock;
        private readonly IErrorRepository _errorRepository;

        public NotificationApplicationService(BreachService breachService, IClock clock, IErrorRepository errorRepository)
        {
            _breachService = breachService;
            _clock = clock;
            _errorRepository = errorRepository;
        }

        public virtual BreachesInfo GetBreaches()
        {
            var breachInput = new BreachInput();
            var eventBreachesInfo = GetEventBreachesInfo(0, breachInput)
                    .Concat(GetEventBreachesInfo(1, breachInput))
                    .Concat(GetEventBreachesInfo(2, breachInput))
                    .Concat(GetEventBreachesInfo(3, breachInput))
                    .Concat(GetEventBreachesInfo(-1, breachInput))
                    .ToArray();
            var periodBreachedInfo = GetPeriodBreachesInfo(0, breachInput)
                .Concat(GetPeriodBreachesInfo(1, breachInput))
                .Concat(GetPeriodBreachesInfo(2, breachInput))
                .Concat(GetPeriodBreachesInfo(3, breachInput))
                .ToArray();

            return new BreachesInfo
            {
                EventsBreaches = eventBreachesInfo,
                PeriodsBreaches = periodBreachedInfo
            };
        }

        public virtual IEnumerable<ErrorInfo> GetErrors()
        {
            var yesterdayClock = _clock.TodayDateAndTime.AddDays(-1);

            return _errorRepository.Get(error => error.CreatedAt >= yesterdayClock)
                .Select(error => new ErrorInfo
                {
                    Message = error.Message,
                    CreatedAt = error.CreatedAt,
                    PeriodId = error.Period == null ? (int?) null : error.Period.Id,
                    PpiNumber = error.Period == null ? null : error.Period.Pathway.PPINumber,
                    NhsNumber = error.Period == null ? null : error.Period.Pathway.Patient.NHSNumber
                });
        }

        private IEnumerable<EventBreachInfo> GetEventBreachesInfo(int daysToBreach, BreachInput breachInput)
        {
              var plannedEvents = _breachService.GetPlannedBreachingEvents(daysToBreach, breachInput);
            return (
                from plannedEvent in plannedEvents
                let statusDays = plannedEvent.BreachStatusAt(_clock.TodayDate)
                let postBreachDays = plannedEvent.GetDaysToBreachAt(_clock.TodayDate)
                select new EventBreachInfo
                {
                    EventCode = plannedEvent.Code,
                    NhsNumber = plannedEvent.Event.Period.Pathway.Patient.NHSNumber,
                    PatientName = plannedEvent.Event.Period.Pathway.Patient.Name,
                    PpiNumber = plannedEvent.Event.Period.Pathway.PPINumber,
                    PeriodId = plannedEvent.Event.Period.Id,
                    Status = statusDays,
                    DaysForStatus = daysToBreach == -1 ? (Math.Abs((int)postBreachDays)) : daysToBreach,
                    PathwayType = plannedEvent.Event.Period.Pathway.PathwayType.ToString()
                });
        }

        private IEnumerable<PeriodBreachInfo> GetPeriodBreachesInfo(int weeksToBreach, BreachInput breachInput)
        {
            var events = _breachService.GetEventsForBreachingPeriods(weeksToBreach, breachInput);
            return (from @event in events
                    let daysRemaining = @event.Period.GetDaysRemainingAt(_clock.TodayDate)
                    orderby daysRemaining
                    select new PeriodBreachInfo
                    {
                        PeriodName = @event.Period.Name,
                        NhsNumber = @event.Period.Pathway.Patient.NHSNumber,
                        PatientName = @event.Period.Pathway.Patient.Name,
                        PpiNumber = @event.Period.Pathway.PPINumber,
                        PeriodId = @event.Period.Id,
                        DaysToBreach = daysRemaining,
                        PathwayType = @event.Period.Pathway.PathwayType.ToString()
                    });
        }
    }
}
