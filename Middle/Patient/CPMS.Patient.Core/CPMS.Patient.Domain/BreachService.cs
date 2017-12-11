using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public class BreachService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlannedEventRepository _plannedEventRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IClock _clock;
        private const int OneWeek = 7;
        private const int ThreeWeeks = 21;
        private const int ThreeDaysToBreach = 3;
        private const int FourDaysToBreach = 4;

        public BreachService(IEventRepository eventRepository, IPlannedEventRepository plannedEventRepository, IClock clock, IPeriodRepository periodRepository)
        {
            _eventRepository = eventRepository;
            _plannedEventRepository = plannedEventRepository;
            _clock = clock;
            _periodRepository = periodRepository;
        }

        public virtual IEnumerable<Event> GetEventsForBreachingPeriods(int weeksToBreach, BreachInput breachInput)
        {
            return _eventRepository.Get(GetCriteriaForEventsInBreachingPeriods(weeksToBreach, breachInput.BreachFilterInput),
                breachInput.ListInput);
        }

        public virtual int CountEventsForBreachingPeriods(int? weeksToBreach, BreachFilterInput breachFilterInput)
        {
            return _eventRepository.Count(GetCriteriaForEventsInBreachingPeriods(weeksToBreach, breachFilterInput));
        }

        public virtual IEnumerable<PlannedEvent> GetPlannedBreachingEvents(int daysToBreach, BreachInput breachInput)
        {
            return _plannedEventRepository.Get(GetCriteriaForPlannedBreachingEvents(daysToBreach, breachInput.BreachFilterInput), breachInput.ListInput);
        }

        public virtual int CountPlannedBreachingEvents(int? daysToBreach, BreachFilterInput breachFilterInput)
        {
            return _plannedEventRepository.Count(GetCriteriaForPlannedBreachingEvents(daysToBreach, breachFilterInput));
        }
        
        public virtual IEnumerable<Event> GetPeriodEvents(int periodId, PeriodEventsInput periodEventsInput)
        {
            return _eventRepository.Get(GetCriteriaForPeriodEvents(periodId, periodEventsInput.PeriodEventsFilterInput), periodEventsInput.ListInput);
        }
        
        public virtual IEnumerable<PlannedEvent> GetPeriodPlannedEvents(int periodId, PeriodEventsInput periodEventsInput)
        {
            return
                _plannedEventRepository.Get(
                    GetCriteriaForPeriodPlannedEvents(periodId, periodEventsInput.PeriodEventsFilterInput), periodEventsInput.ListInput);
        }

        public virtual bool IsPeriodBreached(int periodId)
        {
            return _periodRepository.Get(p => p.Id == periodId && p.GetDaysRemainingAt(_clock.TodayDate) < 0).Any();
        }

        public virtual PeriodCounter GetPeriodCounter(int? hospitalId = null, string specialtyCode = null, int? clinicianId = null)
        {
            if (hospitalId == null && specialtyCode == null && clinicianId == null) return null;

            var periods = _periodRepository.Get(
                p => ((((hospitalId != null) && p.Events.Any(ev => ev.Clinician.Hospital.Id == hospitalId)) ||
                     (specialtyCode != null && p.Events.Any(ev => ev.Clinician.Specialty.Code == specialtyCode)) ||
                     (clinicianId != null && p.Events.Any(ev => ev.Clinician.Id == clinicianId))) &&
                     p.ShouldCountForBreaches && p.IsActive)).ToArray();

            var aboutToBreachNumber = 0;
            var breachedNumber = 0;
            var onTrackNumber = 0;
            foreach (var period in periods)
            {
                var daysRemaining = period.GetDaysRemainingAt(_clock.TodayDate);
                if (daysRemaining >= 0 )
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
            return new PeriodCounter
            {
                AboutToBreachNumber = aboutToBreachNumber,
                BreachedNumber = breachedNumber,
                OnTrackNumber = onTrackNumber
            };
        }

        private Expression<Func<Event, bool>> GetCriteriaForEventsInBreachingPeriods(int? weeksToBreach, BreachFilterInput breachFilterInput)
        {
            if (weeksToBreach < 0)
            {
                // Breached
                return @event => GetCriteriaForEvent(@event) &&
                    @event.Period.GetDaysRemainingAt(_clock.TodayDate) < 0 &&
                    @event.Period.GetDaysRemainingAt(_clock.TodayDate) >= -ThreeWeeks &&
                    GetCriteriaWithFiltersForEvent(@event, breachFilterInput);
            }

            return @event => GetCriteriaForEvent(@event) &&
                ((@event.Period.GetDaysRemainingAt(_clock.TodayDate) == 0 && weeksToBreach == 1) ||
                (@event.Period.GetDaysRemainingAt(_clock.TodayDate) > (weeksToBreach - 1) * OneWeek &&
                @event.Period.GetDaysRemainingAt(_clock.TodayDate) <= weeksToBreach * OneWeek)) &&
                GetCriteriaWithFiltersForEvent(@event, breachFilterInput);
        }

        private Expression<Func<PlannedEvent, bool>> GetCriteriaForPlannedBreachingEvents(int? daysToBreach, BreachFilterInput breachFilterInput)
        {
            if (daysToBreach < 0)
            {
                // Breached
                return plannedEvent => GetCriteriaForPlannedEvent(plannedEvent) &&
                            (((int)plannedEvent.GetDaysToBreachAt(_clock.TodayDate)) < 0) &&
                            GetCriteriaWithFiltersForPlannedEvent(plannedEvent, breachFilterInput);
            }

            if (daysToBreach == ThreeWeeks + 1)
            {
                // About to breach
                return plannedEvent => GetCriteriaForPlannedEvent(plannedEvent) &&
                        (((int)plannedEvent.GetDaysToBreachAt(_clock.TodayDate)) > ThreeDaysToBreach) &&
                        (((int)plannedEvent.GetDaysToBreachAt(_clock.TodayDate)) <= ThreeWeeks + 1) &&
                        GetCriteriaWithFiltersForPlannedEvent(plannedEvent, breachFilterInput);
            }

            return plannedEvent => GetCriteriaForPlannedEvent(plannedEvent) &&
                        (((int)plannedEvent.GetDaysToBreachAt(_clock.TodayDate)) == daysToBreach) &&
                        GetCriteriaWithFiltersForPlannedEvent(plannedEvent, breachFilterInput);
        }

        private bool GetCriteriaWithFiltersForPlannedEvent(PlannedEvent plannedEvent, BreachFilterInput breachFilterInput)
        {
            return ((breachFilterInput == null) ||
                       ((String.IsNullOrEmpty(breachFilterInput.NhsNumber) || plannedEvent.Event.Period.Pathway.Patient.NHSNumber.ToLowerInvariant() == breachFilterInput.NhsNumber.ToLowerInvariant()) &&
                       (String.IsNullOrEmpty(breachFilterInput.PatientName) || plannedEvent.Event.Period.Pathway.Patient.Name.ToLowerInvariant().Contains(breachFilterInput.PatientName.ToLowerInvariant())) &&
                       (breachFilterInput.EventsCode == null || (breachFilterInput.EventsCode.Count == 0 && breachFilterInput.EventCodeFilterValue==null)|| breachFilterInput.EventsCode.Contains(plannedEvent.Code)) &&
                       (String.IsNullOrEmpty(breachFilterInput.Specialty) || plannedEvent.Event.Clinician.Specialty.Name.ToLowerInvariant().Contains(breachFilterInput.Specialty.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.Hospital) || plannedEvent.Event.Clinician.Hospital.Name.ToLowerInvariant().Contains(breachFilterInput.Hospital.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.Clinician) || plannedEvent.Event.Clinician.Name.ToLowerInvariant().Contains(breachFilterInput.Clinician.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.PeriodType) || plannedEvent.Event.Period.GetType().Name.ToLowerInvariant() == breachFilterInput.PeriodType.ToLowerInvariant()) &&
                       (breachFilterInput.AdvancementOrDelay == null || plannedEvent.Event.Period.GetDelayOrAdvancementDays(_clock.TodayDate) == breachFilterInput.AdvancementOrDelay)));
        }

        private bool GetCriteriaWithFiltersForEvent(Event currentEvent, BreachFilterInput breachFilterInput)
        {
            return ((breachFilterInput == null) ||
                       ((String.IsNullOrEmpty(breachFilterInput.NhsNumber) || currentEvent.Period.Pathway.Patient.NHSNumber.ToLowerInvariant() == breachFilterInput.NhsNumber.ToLowerInvariant()) &&
                       (String.IsNullOrEmpty(breachFilterInput.PatientName) || currentEvent.Period.Pathway.Patient.Name.ToLowerInvariant().Contains(breachFilterInput.PatientName.ToLowerInvariant())) &&
                       (breachFilterInput.EventsCode == null || (breachFilterInput.EventsCode.Count == 0 && breachFilterInput.EventCodeFilterValue == null) || breachFilterInput.EventsCode.Contains(currentEvent.Code)) &&
                       (String.IsNullOrEmpty(breachFilterInput.Specialty) || currentEvent.Clinician.Specialty.Name.ToLowerInvariant().Contains(breachFilterInput.Specialty.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.Hospital) || currentEvent.Clinician.Hospital.Name.ToLowerInvariant().Contains(breachFilterInput.Hospital.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.Clinician) || currentEvent.Clinician.Name.ToLowerInvariant().Contains(breachFilterInput.Clinician.ToLowerInvariant())) &&
                       (String.IsNullOrEmpty(breachFilterInput.PeriodType) || currentEvent.Period.GetType().Name.ToLowerInvariant() == breachFilterInput.PeriodType.ToLowerInvariant())));
        }

        private Expression<Func<PlannedEvent, bool>> GetCriteriaForPeriodPlannedEvents(int periodId, PeriodEventsFilterInput periodEventsFilterInput)
        {
            return plannedEvent => plannedEvent.Event.Period.Id == periodId &&
                                   GetCriteriaForPlannedEvent(plannedEvent) &&
                                   GetCriteriaWithFiltersForPeriodPlannedEvents(plannedEvent, periodEventsFilterInput);
        }

        private bool GetCriteriaForPlannedEvent(PlannedEvent plannedEvent)
        {
            return plannedEvent.Event.IsActive &&
                   plannedEvent.IsMandatory &&
                   plannedEvent.Event.Period.ShouldCountForBreaches &&
                   (plannedEvent.BreachDate != null);
        }

        private bool GetCriteriaForEvent(Event @event)
        {
            return @event.IsActive &&
                   @event.Period.ShouldCountForBreaches;
        }

        private bool GetCriteriaWithFiltersForPeriodPlannedEvents(PlannedEvent plannedEvent, PeriodEventsFilterInput periodEventsFilterInput)
        {
            if ((periodEventsFilterInput == null))
            {
                return true;
            }
            var daysToBreachAt = plannedEvent.GetDaysToBreachAt(_clock.TodayDate);
            var isPlannedEventMatched = (String.IsNullOrEmpty(periodEventsFilterInput.Specialty) || plannedEvent.Event.Clinician.Specialty.Name.ToLowerInvariant().Contains(periodEventsFilterInput.Specialty.ToLowerInvariant())) &&
                (periodEventsFilterInput.TargetYear == null || (plannedEvent.BreachDate != null && plannedEvent.BreachDate.Value.Year == periodEventsFilterInput.TargetYear)) &&
                (periodEventsFilterInput.ActualYear == null) &&
                (periodEventsFilterInput.EventsCode == null || (periodEventsFilterInput.EventsCode.Count == 0 && periodEventsFilterInput.EventCodeFilterValue==null)|| periodEventsFilterInput.EventsCode.Contains(plannedEvent.Code));

            if (periodEventsFilterInput.Breaches == null)
            {
                return isPlannedEventMatched; 
            }
            if (daysToBreachAt == null)
            {
                return false;
            }
            if (periodEventsFilterInput.Breaches < 0)
            {
                return isPlannedEventMatched && (((int)daysToBreachAt) < 0);
            }
            if (periodEventsFilterInput.Breaches == FourDaysToBreach)
            {
                return isPlannedEventMatched && (((int)daysToBreachAt) >= FourDaysToBreach);
            }
            return isPlannedEventMatched && daysToBreachAt == periodEventsFilterInput.Breaches;
        }

        private Expression<Func<Event, bool>> GetCriteriaForPeriodEvents(int periodId, PeriodEventsFilterInput periodEventsFilterInput)
        {
            return @event => @event.Period.Id == periodId &&
                             GetCriteriaWithFiltersForPeriodEvents(@event, periodEventsFilterInput);
        }

        private bool GetCriteriaWithFiltersForPeriodEvents(Event @event, PeriodEventsFilterInput periodEventsFilterInput)
        {
            if (periodEventsFilterInput == null)
            {
                return true;
            }
            var isEventMatched = (String.IsNullOrEmpty(periodEventsFilterInput.Specialty) || @event.Clinician.Specialty.Name.ToLowerInvariant().Contains(periodEventsFilterInput.Specialty.ToLowerInvariant())) &&
                 (periodEventsFilterInput.TargetYear == null || (@event.TargetDate != null && @event.TargetDate.Value.Year == periodEventsFilterInput.TargetYear)) &&
                 (periodEventsFilterInput.ActualYear == null || @event.EventDate.Year == periodEventsFilterInput.ActualYear) &&
                 (periodEventsFilterInput.EventsCode == null || (periodEventsFilterInput.EventsCode.Count == 0 && periodEventsFilterInput.EventCodeFilterValue == null) || periodEventsFilterInput.EventsCode.Contains(@event.Code));

            if (periodEventsFilterInput.Breaches == null)
            {
                return isEventMatched;
            }
            if (periodEventsFilterInput.Breaches < 0)
            {
               return isEventMatched && @event.PostBreachDays != null && @event.PostBreachDays.Value > 0;
            }

           return false;
        }
    }
}
