using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public class PatientApplicationService
    {
        private readonly IClock _clock;
        private readonly IMapper<Pathway, PathwayInfo> _pathwayToPathwayInfoMapper;
        private readonly IMapper<CompletedEvent, EventHistoryLogInfo> _completedEventToEventHistoryLogInfoMapper;
        private readonly IMapper<ListInputInfo, ListInput> _listInputInfoToListInputMapper;

        private readonly ICompletedEventRepository _completedEventRepository;
        private readonly IEventMilestoneRepository _eventMilestoneRepository;
        private readonly IPathwayRepository _pathwayRepository;
        private readonly IPeriodRepository _periodRepository;

        private const int FourDaysToBreach = 4;
        private const int ThreeDaysToBreach = 3;

        private const string PausedPeriod = "PausedPeriod";

        public PatientApplicationService(IClock clock,
            IMapper<Pathway, PathwayInfo> pathwayToPathwayInfoMapper,
            IMapper<CompletedEvent, EventHistoryLogInfo> completedEventToEventHistoryLogInfoMapper,
            IMapper<ListInputInfo, ListInput> listInputInfoToListInputMapper,
            ICompletedEventRepository completedEventRepository,
            IEventMilestoneRepository eventMilestoneRepository,
            IPathwayRepository pathwayRepository,
            IPeriodRepository periodRepository)
        {
            _clock = clock;
            _pathwayToPathwayInfoMapper = pathwayToPathwayInfoMapper;
            _completedEventRepository = completedEventRepository;
            _pathwayRepository = pathwayRepository;
            _periodRepository = periodRepository;
            _eventMilestoneRepository = eventMilestoneRepository;
            _completedEventToEventHistoryLogInfoMapper = completedEventToEventHistoryLogInfoMapper;
            _listInputInfoToListInputMapper = listInputInfoToListInputMapper;
        }

        public virtual IEnumerable<LiteEventBreachInfo> GetLiteEventBreaches(int periodId)
        {
            var allEventMilestones = _eventMilestoneRepository
                .Get(GetCriteriaForPeriodEventMilestones(periodId),
                    e => e.CompletedEvent.Period,
                    e => e.Name)
                .Where(eventMilestone => eventMilestone.BreachDate != null)
                .ToArray();

            var allCompletedEvents = _completedEventRepository.Get(GetCriteriaForPeriodCompletedEvents(periodId), null, null, e => e.Name).ToArray();

            return allEventMilestones
                .Where(eventMilestone =>
                        eventMilestone.GetDaysToBreachAt(_clock.TodayDate) != null &&
                        eventMilestone.GetDaysToBreachAt(_clock.TodayDate) <= ThreeDaysToBreach)
                .Select(eventMilestone =>
                    {
                        var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.TodayDate);
                        var eventBreachStatus = eventMilestone.BreachStatusAt(_clock.TodayDate);
                        if (daysToBreachAt != null && eventBreachStatus != null)
                        {
                            return new LiteEventBreachInfo
                            {
                                EventDescription = eventMilestone.Name.Description,
                                DaysForStatus = Math.Abs(daysToBreachAt.Value),
                                Status = eventBreachStatus.Value
                            };
                        }
                        return null;
                    })
                .Concat(allCompletedEvents.Select(completedEvent =>
                    new LiteEventBreachInfo
                    {
                        EventDescription = completedEvent.Name.Description,
                        DaysForStatus = (int)completedEvent.PostBreachDays,
                        Status = EventBreachStatus.Breached
                    }))
                .ToArray();
        }

        public virtual PatientsInfo GetPatientsOnPathway(PatientInputInfo patientInputInfo)
        {
            var listInput = _listInputInfoToListInputMapper.Map(patientInputInfo.ListInputInfo);

            var pausedPatients = patientInputInfo.PatientsFilterInputInfo.PeriodType == PausedPeriod;

            var completedEvents = pausedPatients ?
                _completedEventRepository
                    .Get(GetCriteriaForPatientsOnPathway(patientInputInfo.PatientsFilterInputInfo),
                        null,
                        listInput,
                        e => e.Period.Pathway.Patient,
                        e => e.Clinician.Hospital,
                        e => e.Clinician.Specialty)
                    .Where(completedEvent => completedEvent.Period.GetPeriodStatus() == PeriodStatus.Paused)
                    .ToList() :
                _completedEventRepository
                    .Get(
                        GetCriteriaForPatientsOnPathway(patientInputInfo.PatientsFilterInputInfo),
                        patientInputInfo.PatientsFilterInputInfo.PeriodType,
                        listInput,
                        e => e.Period.Pathway.Patient,
                        e => e.Clinician.Hospital,
                        e => e.Clinician.Specialty)
                    .Where(completedEvent => completedEvent.Period.GetPeriodStatus() != PeriodStatus.Paused)
                    .ToList();

            var patientInfoCount = completedEvents.Count;

            var patientsInfo = (
                    from completedEvent in completedEvents
                    select CreatePatientInfoFromEvent(completedEvent)).ToArray();

            return new PatientsInfo
            {
                PatientInfo = patientsInfo,
                TotalNumberOfPatients = patientInfoCount
            };
        }

        public virtual IEnumerable<PathwayInfo> GetPathwaysForPatient(string nhsNumber)
        {
            return _pathwayRepository
                .Get(pathway => pathway.Patient.NHSNumber == nhsNumber && pathway.Periods.Any())
                .Select(pathway => _pathwayToPathwayInfoMapper.Map(pathway))
                .ToArray();
        }

        public virtual IEnumerable<PeriodInfo> GetPeriodsForPathway(string ppiNumber)
        {
            var periods = _periodRepository
                .Get(
                    period => period.Pathway.PPINumber == ppiNumber,
                    period => period.Pathway,
                    period => period.CompletedEvents.Select(completedEvent => completedEvent.Name))
                .ToArray();

            return from period in periods
                let isBreached = period.GetDaysRemainingAt(_clock.TodayDate) < 0
                let daysInPeriod = period.GetDaysSpentAt(_clock.TodayDate)
                select new PeriodInfo
                {
                    Id = period.Id,
                    Name = period.Name,
                    StartDate = period.StartDate,
                    StopDate = period.StopDate,
                    OrganizationCode = period.Pathway.OrganizationCode,
                    Status = period.GetPeriodStatus(),
                    PeriodType = period.GetType().Name,
                    IsBreached = isBreached,
                    DaysInPeriod = daysInPeriod,
                    ExpectedBreachDate = period.StopDate != null ?
                        ((DateTime)period.StopDate).AddDays(period.MaximumNumberOfDays - daysInPeriod) :
                        _clock.TodayDate.AddDays(period.MaximumNumberOfDays - daysInPeriod)
                };
        }

        public virtual PeriodEventsInfo GetPeriodEvents(int periodId, PeriodEventsInputInfo periodEventsInputModel)
        {
            var period = _periodRepository
                .Get(p => p.Id == periodId,
                    p => p.CompletedEvents.Select(e => e.Clinician.Specialty),
                    p => p.CompletedEvents.Select(e => e.Name))
                .FirstOrDefault();

            if (period != null)
            {
                var completedEvents = period.CompletedEvents
                    .Where(
                        completedEvent =>
                            IsCompletedEventMatched(completedEvent, periodEventsInputModel.PeriodEventsFilterInputInfo))
                    .ToList();

                var lastCompletedEvent = completedEvents.LastOrDefault();

                IEnumerable<EventMilestone> eventMilestones = null;
                if (lastCompletedEvent != null && lastCompletedEvent.IsActive)
                {
                    eventMilestones = _eventMilestoneRepository
                        .Get(eventMilestone =>
                            eventMilestone.CompletedEvent.Id == lastCompletedEvent.Id &&
                            eventMilestone.IsMandatory,
                            eventMilestone => eventMilestone.Name)
                        .Where(eventMilestone =>
                            eventMilestone.BreachDate != null &&
                            IsEventMilestoneMatched(eventMilestone, periodEventsInputModel.PeriodEventsFilterInputInfo));
                }
                var periodEventsInfo = GetPeriodEventsInfo(completedEvents, eventMilestones, period.GetType().Name);
                if (periodEventsInfo != null)
                {
                    var periodEventsInfoList = periodEventsInfo.ToList();
                    var orderedEvents = OrderPeriodEventsByProperty(periodEventsInfoList, periodEventsInputModel.ListInputInfo);

                    return new PeriodEventsInfo
                    {
                        Events = (periodEventsInputModel.ListInputInfo == null || periodEventsInputModel.ListInputInfo.Index == null || periodEventsInputModel.ListInputInfo.PageCount == null)
                                ? orderedEvents
                                : orderedEvents
                                    .Skip(((int) periodEventsInputModel.ListInputInfo.Index - 1) * (int) periodEventsInputModel.ListInputInfo.PageCount)
                                    .Take((int) periodEventsInputModel.ListInputInfo.PageCount),
                        TotalNumberOfEvents = periodEventsInfoList.Count(),
                        IsBreached = period.GetDaysRemainingAt(_clock.TodayDate) < 0
                    };
                }
            }

            return null;
        }

        public virtual EventsHistoryLogInfo GetEventHistoryLog(int periodId, EventHistoryLogInputInfo inputInfo)
        {
            var listInput = _listInputInfoToListInputMapper.Map(inputInfo.ListInputInfo);
            var eventsInfo = _completedEventRepository
                .Get(
                    GetCriteriaForEventHistoryLog(periodId, inputInfo.EventHistoryLogFilterInputInfo),
                    null,
                    listInput,
                    e => e.Name)
                .Select(completedEvent => _completedEventToEventHistoryLogInfoMapper.Map(completedEvent))
                .ToArray();

            return new EventsHistoryLogInfo
            {
                EventsInfo = eventsInfo,
                TotalNumberOfEvents = _completedEventRepository.Count(GetCriteriaForEventHistoryLog(periodId, inputInfo.EventHistoryLogFilterInputInfo))
            };
        }

        private Expression<Func<EventMilestone, bool>> GetCriteriaForPeriodEventMilestones(int periodId)
        {
            return eventMilestone => eventMilestone.CompletedEvent.Period.Id == periodId &&
                                     eventMilestone.CompletedEvent.Period.ShouldCountForBreaches &&
                                     eventMilestone.CompletedEvent.IsActive &&
                                     eventMilestone.IsMandatory;
        }

        private Expression<Func<CompletedEvent, bool>> GetCriteriaForPeriodCompletedEvents(int periodId)
        {
            return completedEvent => completedEvent.Period.Id == periodId && completedEvent.BreachStatus == EventBreachStatus.Breached;
        }

        private PatientInfo CreatePatientInfoFromEvent(CompletedEvent completedEvent)
        {
            return new PatientInfo
            {
                NHSNumber = completedEvent.Period.Pathway.Patient.NHSNumber,
                PatientName = completedEvent.Period.Pathway.Patient.Name,
                DateOfBirth = completedEvent.Period.Pathway.Patient.DateOfBirth,
                Age = completedEvent.Period.Pathway.Patient.GetAgeAt(_clock.TodayDate),
                Hospital = completedEvent.Clinician.Hospital.Name,
                PpiNumber = completedEvent.Period.Pathway.PPINumber,
                PeriodId = completedEvent.Period.Id
            };
        }

        private Expression<Func<CompletedEvent, bool>> GetCriteriaForEventHistoryLog(int periodId, EventHistoryLogFilterInputInfo eventHistoryLogFilterInputInfo)
        {
            return completedEvent => completedEvent.Period.Id == periodId &&
                      ((eventHistoryLogFilterInputInfo.TargetYear == null || (completedEvent.TargetDate != null && completedEvent.TargetDate.Value.Year == eventHistoryLogFilterInputInfo.TargetYear)) &&
                      (eventHistoryLogFilterInputInfo.ActualYear == null || completedEvent.EventDate.Year == eventHistoryLogFilterInputInfo.ActualYear) &&
                      (eventHistoryLogFilterInputInfo.ImportYear == null || completedEvent.EventDate.Year == eventHistoryLogFilterInputInfo.ImportYear) &&
                      (String.IsNullOrEmpty(eventHistoryLogFilterInputInfo.Description) || (completedEvent.Comments!=null && completedEvent.Comments.ToLower().Contains(eventHistoryLogFilterInputInfo.Description.ToLower()))) &&
                      (string.IsNullOrEmpty(eventHistoryLogFilterInputInfo.EventDescription) || completedEvent.Name.Description.Contains(eventHistoryLogFilterInputInfo.EventDescription)));
        }

        private IEnumerable<PeriodEventInfo> OrderPeriodEventsByProperty(IEnumerable<PeriodEventInfo> periodEventsInfo, ListInputInfo listInputInfo)
        {
            if (listInputInfo == null || listInputInfo.OrderBy == null || listInputInfo.OrderDirection == null)
            {
                return periodEventsInfo;
            }
            switch (listInputInfo.OrderBy)
            {
                case OrderBy.EventDescription:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInputInfo.OrderDirection, e => e.EventDescription);
                case OrderBy.Specialty:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInputInfo.OrderDirection, e => e.Specialty);
                case OrderBy.TargetDate:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInputInfo.OrderDirection, e => e.TargetDate);
                case OrderBy.ActualDate:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInputInfo.OrderDirection, e => e.ActualDate);
            }
            return periodEventsInfo;
        }

        private IEnumerable<PeriodEventInfo> GetPeriodEventInfoOrderedByDirection<TKey>(IEnumerable<PeriodEventInfo> periodEventsInfo, OrderDirection? orderDirection, Func<PeriodEventInfo, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.Ascending
                ? periodEventsInfo.OrderBy(orderCriteria)
                : periodEventsInfo.OrderByDescending(orderCriteria);
        }

        private Expression<Func<CompletedEvent, bool>> GetCriteriaForPatientsOnPathway(PatientFilterInputInfo patientFilterInputInfo)
        {
            return completedEvent => completedEvent.IsActive &&
                    ((String.IsNullOrEmpty(patientFilterInputInfo.Hospital) || completedEvent.Clinician.Hospital.Name.ToLower().Contains(patientFilterInputInfo.Hospital.ToLower())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.Specialty) || completedEvent.Clinician.Specialty.Name.ToLower().Contains(patientFilterInputInfo.Specialty.ToLower())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.Clinician) || completedEvent.Clinician.Name.ToLower().Contains(patientFilterInputInfo.Clinician.ToLower())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.PatientName) || completedEvent.Period.Pathway.Patient.Name.ToLower().Contains(patientFilterInputInfo.PatientName.ToLower())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.PpiNumber) || completedEvent.Period.Pathway.PPINumber.ToLower() == patientFilterInputInfo.PpiNumber.ToLower()) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.NhsNumber) || completedEvent.Period.Pathway.Patient.NHSNumber.ToLower() == patientFilterInputInfo.NhsNumber.ToLower()));
        }

        private bool IsCompletedEventMatched(CompletedEvent completedEvent, PeriodEventsFilterInputInfo periodEventsFilterInput)
        {
            if (periodEventsFilterInput == null)
            {
                return true;
            }
            var isCompletedEventMatched = (String.IsNullOrEmpty(periodEventsFilterInput.Specialty) || completedEvent.Clinician.Specialty.Name.ToLowerInvariant().Contains(periodEventsFilterInput.Specialty.ToLowerInvariant())) &&
                 (periodEventsFilterInput.TargetYear == null || (completedEvent.TargetDate != null && completedEvent.TargetDate.Value.Year == periodEventsFilterInput.TargetYear)) &&
                 (periodEventsFilterInput.ActualYear == null || completedEvent.EventDate.Year == periodEventsFilterInput.ActualYear) &&
                 (string.IsNullOrEmpty(periodEventsFilterInput.EventDescription) || completedEvent.Name.Description.Contains(periodEventsFilterInput.EventDescription));

            if (periodEventsFilterInput.Breaches == null)
            {
                return isCompletedEventMatched;
            }
            if (periodEventsFilterInput.Breaches < 0)
            {
                return isCompletedEventMatched && completedEvent.PostBreachDays != null && completedEvent.PostBreachDays.Value > 0;
            }

            return false;
        }

        private bool IsEventMilestoneMatched(EventMilestone eventMilestone, PeriodEventsFilterInputInfo periodEventsFilterInput)
        {
            if ((periodEventsFilterInput == null))
            {
                return true;
            }
            var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.TodayDate);
            var isEventMilestoneMatched = (String.IsNullOrEmpty(periodEventsFilterInput.Specialty) || eventMilestone.CompletedEvent.Clinician.Specialty.Name.ToLowerInvariant().Contains(periodEventsFilterInput.Specialty.ToLowerInvariant())) &&
                (periodEventsFilterInput.TargetYear == null || (eventMilestone.BreachDate != null && eventMilestone.BreachDate.Value.Year == periodEventsFilterInput.TargetYear)) &&
                (periodEventsFilterInput.ActualYear == null) &&
                (string.IsNullOrEmpty(periodEventsFilterInput.EventDescription) || eventMilestone.Name.Description.Contains(periodEventsFilterInput.EventDescription));

            if (periodEventsFilterInput.Breaches == null)
            {
                return isEventMilestoneMatched;
            }
            if (daysToBreachAt == null)
            {
                return false;
            }
            if (periodEventsFilterInput.Breaches < 0)
            {
                return isEventMilestoneMatched && (((int)daysToBreachAt) < 0);
            }
            if (periodEventsFilterInput.Breaches == FourDaysToBreach)
            {
                return isEventMilestoneMatched && (((int)daysToBreachAt) >= FourDaysToBreach);
            }
            return isEventMilestoneMatched && daysToBreachAt == periodEventsFilterInput.Breaches;
        }

        private IEnumerable<PeriodEventInfo> GetPeriodEventsInfo(List<CompletedEvent> completedEvents, IEnumerable<EventMilestone> eventMilestones, string periodName)
        {
            var lastCompletedEvent = completedEvents.LastOrDefault();

            if (lastCompletedEvent != null)
            {
                var periodEventsInfo = (from completedEvent in completedEvents
                        select new PeriodEventInfo
                        {
                            EventDescription = completedEvent.Name.Description,
                            Specialty = completedEvent.Clinician.Specialty.Name,
                            TargetDate = completedEvent.TargetDate,
                            ActualDate = completedEvent.EventDate,
                            EventStatus = completedEvent.EventStatus,
                            BreachStatus = completedEvent.BreachStatus,
                            DaysPostbreach = completedEvent.PostBreachDays,
                            PeriodType = periodName
                        });
                if (eventMilestones != null)
                {
                    periodEventsInfo = periodEventsInfo.Concat(
                        from eventMilestone in eventMilestones
                        let remainingDays = eventMilestone.GetDaysToBreachAt(_clock.TodayDate)
                        select new PeriodEventInfo
                        {
                            EventDescription = eventMilestone.Name.Description,
                            Specialty = lastCompletedEvent.Clinician.Specialty.Name,
                            TargetDate = (lastCompletedEvent.EventStatus == EventStatus.Paused) ? null : eventMilestone.BreachDate,
                            BreachStatus = eventMilestone.BreachStatusAt(_clock.TodayDate),
                            DaysPostbreach = remainingDays == null ? (int?) null : Math.Abs((int) remainingDays)
                        });
                }

                return periodEventsInfo;
            }
            return null;
        }
    }
}
