using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Patient.Manager
{
    public class PatientApplicationService
    {
        private readonly BreachService _breachService;
        private readonly IClock _clock;
        private readonly IMapper<BreachInputInfo, BreachInput> _breachInputInfoToBreachInputMapper;
        private readonly IMapper<Hospital, HospitalInfo> _hospitalToHospitalInfoMapper;
        private readonly IMapper<Specialty, SpecialtyInfo> _specialtyToSpecialtyInfoMapper;
        private readonly IMapper<Clinician, ClinicianInfo> _clinicianToClinicianInfoMapper;
        private readonly IMapper<Pathway, PathwayInfo> _pathwayToPathwayInfoMapper;
        private readonly IMapper<Event, EventHistoryLogInfo> _eventToEventHistoryLogInfoMapper;
        private readonly IMapper<PeriodEventsInputInfo, PeriodEventsInput> _periodEventsInputInfoToPeriodEventsInputMapper;
        private readonly IMapper<ListInputInfo, ListInput> _listInputInfoToListInputMapper;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IClinicianRepository _clinicianRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IPathwayRepository _pathwayRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IPlannedEventRepository _plannedEventRepository ;


        public PatientApplicationService(BreachService breachService,
            IClock clock,
            IMapper<BreachInputInfo, BreachInput> breachInputInfoToBreachInputMapper,
            IMapper<Hospital, HospitalInfo> hospitalToHospitalInfoMapper,
            IMapper<Specialty, SpecialtyInfo> specialtyToSpecialtyInfoMapper,
            IMapper<Clinician, ClinicianInfo> clinicianToClinicianInfoMapper,
            IMapper<Pathway, PathwayInfo> pathwayToPathwayInfoMapper,
            IMapper<Event, EventHistoryLogInfo> eventToEventHistoryLogInfoMapper,
            IMapper<PeriodEventsInputInfo, PeriodEventsInput> periodEventsInputInfoToPeriodEventsInputMapper,
            IMapper<ListInputInfo, ListInput> listInputInfoToListInputMapper,
            IHospitalRepository hospitalRepository,
            ISpecialtyRepository specialtyRepository,
            IClinicianRepository clinicianRepository,
            IEventRepository eventRepository,
            IPathwayRepository pathwayRepository,
            IPeriodRepository periodRepository,
            IPlannedEventRepository plannedEventRepository)
        {
            _breachService = breachService;
            _clock = clock;
            _breachInputInfoToBreachInputMapper = breachInputInfoToBreachInputMapper;
            _hospitalToHospitalInfoMapper = hospitalToHospitalInfoMapper;
            _specialtyToSpecialtyInfoMapper = specialtyToSpecialtyInfoMapper;
            _clinicianToClinicianInfoMapper = clinicianToClinicianInfoMapper;
            _pathwayToPathwayInfoMapper = pathwayToPathwayInfoMapper;
            _periodEventsInputInfoToPeriodEventsInputMapper = periodEventsInputInfoToPeriodEventsInputMapper;
            _hospitalRepository = hospitalRepository;
            _specialtyRepository = specialtyRepository;
            _clinicianRepository = clinicianRepository;
            _eventRepository = eventRepository;
            _pathwayRepository = pathwayRepository;
            _periodRepository = periodRepository;
            _eventToEventHistoryLogInfoMapper = eventToEventHistoryLogInfoMapper;
            _listInputInfoToListInputMapper = listInputInfoToListInputMapper;
            _plannedEventRepository = plannedEventRepository;
        }

        public virtual EventBreachesInfo GetEventBreaches(int daysToBreach, BreachInputInfo breachInputInfo)
        {
            var eventBreachInput = _breachInputInfoToBreachInputMapper.Map(breachInputInfo);

            var plannedEvents = _breachService.GetPlannedBreachingEvents(daysToBreach, eventBreachInput);

            var eventInfoList = (
                from plannedEvent in plannedEvents
                let postBreachDays = plannedEvent.GetDaysToBreachAt(_clock.TodayDate)
                let daysRemaining = plannedEvent.Event.Period.GetDaysRemainingAt(_clock.TodayDate)
                where postBreachDays != null
                select new EventBreachInfo
                {
                    EventCode = plannedEvent.Code,
                    PatientNHSNumber = plannedEvent.Event.Period.Pathway.Patient.NHSNumber,
                    PatientName = plannedEvent.Event.Period.Pathway.Patient.Name,
                    PPINumber = plannedEvent.Event.Period.Pathway.PPINumber,
                    Clinician = plannedEvent.Event.Clinician.Name,
                    Specialty = plannedEvent.Event.Clinician.Specialty.Name,
                    PostBreachDays = Math.Abs((int)postBreachDays),
                    DaysInPeriod = plannedEvent.Event.Period.GetDaysSpentAt(_clock.TodayDate),
                    DaysRemainingInPeriod = daysRemaining < 0 ? 0 : daysRemaining,
                    DelayOrAdvancementDays = plannedEvent.Event.Period.GetDelayOrAdvancementDays(_clock.TodayDate),
                    PeriodType = plannedEvent.Event.Period.GetType().Name,
                    EventType = plannedEvent.Event.GetType().Name,
                    PeriodId = plannedEvent.Event.Period.Id
                }).ToArray();

            return new EventBreachesInfo
            {
                EventsInfo = OrderBySpentDaysOrToGoDaysInPeriod(eventInfoList, eventBreachInput.ListInput),
                TotalNumberOfPlannedEvents = _breachService.CountPlannedBreachingEvents(daysToBreach, eventBreachInput.BreachFilterInput)
            };
        }

        public virtual IEnumerable<LiteEventBreachInfo> GetLiteEventBreaches(int periodId)
        {
            var allPeriodPlannedEvents = _breachService.GetPeriodPlannedEvents(periodId, new PeriodEventsInput());
            var allEvents = _breachService.GetPeriodEvents(periodId, new PeriodEventsInput());
 
            return
                allPeriodPlannedEvents.Where(
                    plannedEvent =>
                        plannedEvent.GetDaysToBreachAt(_clock.TodayDate) != null &&
                        plannedEvent.GetDaysToBreachAt(_clock.TodayDate) <= 3)
                    .Select(
                        plannedEvent =>
                        {
                            var daysToBreachAt = plannedEvent.GetDaysToBreachAt(_clock.TodayDate);
                            var eventBreachStatus = plannedEvent.BreachStatusAt(_clock.TodayDate);
                            if (daysToBreachAt != null && eventBreachStatus != null)
                            {
                                return new LiteEventBreachInfo
                                {
                                    EventCode = plannedEvent.Code,
                                    DaysForStatus = Math.Abs(daysToBreachAt.Value),
                                    Status = eventBreachStatus.Value
                                };
                            }
                            return null;
                        }).Concat(allEvents.Where(
                @event => @event.BreachStatus == EventBreachStatus.Breached).Select(@event =>
                    new LiteEventBreachInfo
                    {
                        EventCode = @event.Code,
                        DaysForStatus = (int) @event.PostBreachDays,
                        Status = EventBreachStatus.Breached
                    })).ToArray();
        }

        public virtual PeriodBreachesInfo GetPeriodBreaches(int weeksToBreach, BreachInputInfo breachInputInfo)
        {
            var eventBreachInput = _breachInputInfoToBreachInputMapper.Map(breachInputInfo);

            var events = _breachService.GetEventsForBreachingPeriods(weeksToBreach, eventBreachInput);

            var periodsInfoList = (
                from @event in events
                let postBreachDays = @event.Period.GetDaysRemainingAt(_clock.TodayDate)
                let daysRemaining = @event.Period.GetDaysRemainingAt(_clock.TodayDate)
                select new PeriodBreachInfo
                {
                    EventCode = @event.Code,
                    PatientNHSNumber = @event.Period.Pathway.Patient.NHSNumber,
                    PatientName = @event.Period.Pathway.Patient.Name,
                    PPINumber = @event.Period.Pathway.PPINumber,
                    Clinician = @event.Clinician.Name,
                    Specialty = @event.Clinician.Specialty.Name,
                    PostBreachDays = Math.Abs(postBreachDays),
                    DaysInPeriod = @event.Period.GetDaysSpentAt(_clock.TodayDate),
                    DaysRemainingInPeriod = daysRemaining < 0 ? 0 : daysRemaining,
                    PeriodType = @event.Period.GetType().Name,
                    EventType = @event.GetType().Name,
                    PeriodId = @event.Period.Id
                }).ToArray();

            return new PeriodBreachesInfo
            {
                PeriodsInfo = periodsInfoList,
                TotalNumberOfPeriods = _breachService.CountEventsForBreachingPeriods(weeksToBreach, eventBreachInput.BreachFilterInput)
            };
        }

        public virtual PeriodsAndEventsBreachesCountInfo GetPeriodsAndEventsBreachesCount()
        {
            return new PeriodsAndEventsBreachesCountInfo
            {
                EventsBreachesCountInfo = new EventsBreachesCountInfo
                {
                    ThreeDays = _breachService.CountPlannedBreachingEvents(3, null),
                    TwoDays = _breachService.CountPlannedBreachingEvents(2, null),
                    OneDays =  _breachService.CountPlannedBreachingEvents(1, null),
                    Breach = _breachService.CountPlannedBreachingEvents(0, null),
                    PostBreach = _breachService.CountPlannedBreachingEvents(-1, null),
                    AboutToBreach = _breachService.CountPlannedBreachingEvents(22, null)
                },
                PeriodsBreachesCountInfo = new PeriodsBreachesCountInfo
                {
                    FourWeeks = _breachService.CountEventsForBreachingPeriods(4, null),
                    ThreeWeeks = _breachService.CountEventsForBreachingPeriods(3, null),
                    TwoWeeks = _breachService.CountEventsForBreachingPeriods(2, null),
                    OneWeek = _breachService.CountEventsForBreachingPeriods(1, null),
                    PostBreach = _breachService.CountEventsForBreachingPeriods(-1, null)
                }
            };
        }

        public virtual IEnumerable<HospitalInfo> GetHospitals()
        {
            return _hospitalRepository.Get(hospital => true).Select(hospital => _hospitalToHospitalInfoMapper.Map(hospital));
        }

        public virtual IEnumerable<SpecialtyInfo> GetSpecialties(int? hospitalId)
        {
            return
                _specialtyRepository.Get(
                    specialty =>
                        hospitalId == null ||
                        (specialty.Hospitals.Any(hospital => hospital.Id == hospitalId) &&
                         specialty.Clinicians.Any(clinician => clinician.Hospital.Id == hospitalId)))
                    .Select(specialty => _specialtyToSpecialtyInfoMapper.Map(specialty));
        }

        public virtual IEnumerable<ClinicianInfo> GetClinicians(int? hospitalId, string specialtyCode)
        {
            return _clinicianRepository.Get(clinician =>
                    (string.IsNullOrEmpty(specialtyCode) || clinician.Specialty.Code == specialtyCode) &&
                    (hospitalId == null || clinician.Hospital.Id == hospitalId)).Select(clinician => _clinicianToClinicianInfoMapper.Map(clinician));
        }

        public virtual PatientsInfo GetPatientsOnPathway(PatientInputInfo patientInputInfo)
        {
            var listInput = _listInputInfoToListInputMapper.Map(patientInputInfo.ListInputInfo);

            var activeEvents = _eventRepository.Get(GetCriteriaForPatientsOnPathway(patientInputInfo.PatientsFilterInputInfo),
                listInput);

            if (patientInputInfo.PatientsFilterInputInfo != null &&
                patientInputInfo.PatientsFilterInputInfo.NhsNumber != null)
            {
                var @event = activeEvents.First();
                var patientInfo = CreatePatientInfoFromEvent(@event);
                return new PatientsInfo
                {
                    PatientInfo = new List<PatientInfo>{patientInfo},
                    TotalNumberOfPatients = 1
                };
            }

            var patientsInfo = (
                    from @event in activeEvents
                    select CreatePatientInfoFromEvent(@event)).ToArray();

            return new PatientsInfo
            {
                PatientInfo = patientsInfo,
                TotalNumberOfPatients = _eventRepository.Count(GetCriteriaForPatientsOnPathway(patientInputInfo.PatientsFilterInputInfo))
            };

        }

        private PatientInfo CreatePatientInfoFromEvent(Event @event)
        {
            return new PatientInfo
            {
                NHSNumber = @event.Period.Pathway.Patient.NHSNumber,
                PatientName = @event.Period.Pathway.Patient.Name,
                DateOfBirth = @event.Period.Pathway.Patient.DateOfBirth,
                Age = @event.Period.Pathway.Patient.GetAgeAt(_clock.TodayDate),
                Hospital = @event.Clinician.Hospital.Name,
                PpiNumber = @event.Period.Pathway.PPINumber,
                PeriodId = @event.Period.Id
            };
        }

        public virtual IEnumerable<PathwayInfo> GetPathwaysForPatient(string nhsNumber)
        {
            return _pathwayRepository.Get(pathway => pathway.Patient.NHSNumber == nhsNumber && pathway.Periods.Any()).Select(pathway => _pathwayToPathwayInfoMapper.Map(pathway));
        }

        public virtual IEnumerable<PeriodInfo> GetPeriodsForPathway(string ppiNumber)
        {
            var periods = _periodRepository.Get(period => period.Pathway.PPINumber == ppiNumber);

            return from period in periods
                let isBreached = _breachService.IsPeriodBreached(period.Id)
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
            var periodEventsInput = _periodEventsInputInfoToPeriodEventsInputMapper.Map(periodEventsInputModel);
            var listInput = periodEventsInput.ListInput;
            periodEventsInput.ListInput = null;
            var events = _breachService.GetPeriodEvents(periodId, periodEventsInput);
            var plannedEvents = _breachService.GetPeriodPlannedEvents(periodId, periodEventsInput);

            var periodEventsInfo = (
                from @event in events
                select new PeriodEventInfo
                {
                    EventCode = @event.Code,
                    Specialty = @event.Clinician.Specialty.Name,
                    TargetDate = @event.TargetDate,
                    ActualDate = @event.EventDate,
                    EventStatus = @event.EventStatus,
                    BreachStatus = @event.BreachStatus,
                    DaysPostbreach = @event.PostBreachDays,
                    PeriodType = @event.Period.GetType().Name
                }).Concat(
                    from plannedEvent in plannedEvents
                    let remainingDays = plannedEvent.GetDaysToBreachAt(_clock.TodayDate)
                    select new PeriodEventInfo
                    {
                        EventCode = plannedEvent.Code,
                        Specialty = plannedEvent.Event.Clinician.Specialty.Name,
                        TargetDate =
                            (plannedEvent.Event.EventStatus == EventStatus.Paused) ? null : plannedEvent.BreachDate,
                        BreachStatus = plannedEvent.BreachStatusAt(_clock.TodayDate),
                        DaysPostbreach = remainingDays == null ? (int?) null : Math.Abs((int) remainingDays)
                    }).ToArray();

            var orderedEvents = OrderPeriodEventsByProperty(periodEventsInfo, listInput);

            return new PeriodEventsInfo
            {
                Events = (listInput == null || listInput.Index == null || listInput.PageCount == null) ? orderedEvents : orderedEvents.Skip((int)listInput.Index * (int)listInput.PageCount).Take((int)listInput.PageCount),
                TotalNumberOfEvents = periodEventsInfo.Count(),
                IsBreached = _breachService.IsPeriodBreached(periodId)
            };
        }


        public virtual EventsHistoryLogInfo GetEventHistoryLog(int periodId, EventHistoryLogInputInfo inputInfo)
        {
            var listInput = _listInputInfoToListInputMapper.Map(inputInfo.ListInputInfo);
            var eventsInfo =
                _eventRepository.Get(GetCriteriaForEventHistoryLog(periodId, inputInfo.EventHistoryLogFilterInputInfo), listInput).Select(@event => _eventToEventHistoryLogInfoMapper.Map(@event)).ToArray();
          
            return new EventsHistoryLogInfo
            {
                EventsInfo = eventsInfo,
                TotalNumberOfEvents = _eventRepository.Count(GetCriteriaForEventHistoryLog(periodId, inputInfo.EventHistoryLogFilterInputInfo))
            };
        }

        public virtual IEnumerable<EventCounterInfo> GetEventsCounter()
        {
            foreach (var eventCode in (EventCode[])Enum.GetValues(typeof(EventCode)))
            {
                if (eventCode == EventCode.ReferralReceived || eventCode == EventCode.PathwayCompletion ||
                    eventCode == EventCode.PatientCancelEvent || eventCode == EventCode.HospitalCancelEvent ||
                    eventCode == EventCode.ReferralReview || eventCode == EventCode.DiagnosticsOrderPlaced ||
                    eventCode == EventCode.BookedOutpatientFollowUpAppointment ||
                    eventCode == EventCode.BookedPostInpatientFirstOutpatientAppointment ||
                    eventCode == EventCode.BookedPostInpatientSecondOutpatientAppointment ||
                    eventCode == EventCode.DidNotAttend || eventCode == EventCode.PatientCancelEvent ||
                    eventCode == EventCode.HospitalCancelEvent) continue;

                var plannedEvents = GetPlannedEventsByCodeForActiveEvents(eventCode).ToArray();
                var events = GetEventsByCodeForActivePeriods(eventCode).ToArray();

                var breachedPlannedEvents = CountBreachedPlannedEvents(plannedEvents);
                var breachedEvents = CountBreachedEvents(events);

                yield return new EventCounterInfo
                {
                    EventCode = eventCode,
                    BreachedEventsNumber = breachedEvents,
                    BreachedPlannedEventsNumber = breachedPlannedEvents,
                    TotalEventsNumber = events.Count(),
                    TotalPlannedEventsNumber = plannedEvents.Count()
                };
            }
        }


        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForTrust()
        {
            var hospitals = _hospitalRepository.Get(hospital => true);

            return
                from hospital in hospitals
                let periodCounter = _breachService.GetPeriodCounter(hospital.Id)
                select new PeriodCounterInfo
                {
                    Name = hospital.Name,
                    Id = hospital.Id.ToString(CultureInfo.InvariantCulture),
                    AboutToBreachNumber = periodCounter.AboutToBreachNumber,
                    BreachedNumber = periodCounter.BreachedNumber,
                    OnTrackNumber = periodCounter.OnTrackNumber
                };
        }

        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForHospital(string hospitalId)
        {
            var id = Int32.Parse(hospitalId);
            var specialties =
                _specialtyRepository.Get(
                    specialty => specialty.Hospitals.Any(hospital => hospital.Id == id));

            return
                from specialty in specialties
                let periodCounter = _breachService.GetPeriodCounter(specialtyCode: specialty.Code)
                select new PeriodCounterInfo
                {
                    Name = specialty.Name,
                    Id = specialty.Code,
                    BreachedNumber = periodCounter.BreachedNumber,
                    AboutToBreachNumber = periodCounter.AboutToBreachNumber,
                    OnTrackNumber = periodCounter.OnTrackNumber
                };
        }

        public virtual IEnumerable<PeriodCounterInfo> GetPeriodCounterForSpecialty(string specialtyCode, string hospitalId)
        {
            IEnumerable<Clinician> clinicians;
            if (string.IsNullOrEmpty(hospitalId))
            {
                clinicians =
                    _clinicianRepository.Get(
                        clinician => clinician.Specialty.Code == specialtyCode);
            }
            else
            {
                clinicians =
                    _clinicianRepository.Get(
                        clinician => clinician.Specialty.Code == specialtyCode && clinician.Hospital.Id == int.Parse(hospitalId));
            }
            return
                from clinician in clinicians
                let periodCounter = _breachService.GetPeriodCounter(clinicianId: clinician.Id)
                select new PeriodCounterInfo
                {
                    Name = clinician.Name,
                    Id = clinician.Id.ToString(CultureInfo.InvariantCulture),
                    BreachedNumber = periodCounter.BreachedNumber,
                    AboutToBreachNumber = periodCounter.AboutToBreachNumber,
                    OnTrackNumber = periodCounter.OnTrackNumber
                };
        }


        private IEnumerable<PlannedEvent> GetPlannedEventsByCodeForActiveEvents(EventCode eventCode)
        {
            return _plannedEventRepository.Get(plannedEvent => plannedEvent.Code == eventCode &&
                                                               plannedEvent.Event.IsActive, null);
        }

        private IEnumerable<Event> GetEventsByCodeForActivePeriods(EventCode eventCode)
        {
            return _eventRepository.Get(@event => @event.Code == eventCode &&
                                                               @event.Period.IsActive, null);
        }

        private int CountBreachedEvents(IEnumerable<Event> events)
        {
            return events.Count(@event =>
                        @event.BreachStatus == EventBreachStatus.Breached &&
                        @event.Period.ShouldCountForBreaches);
        }

        private int CountBreachedPlannedEvents(IEnumerable<PlannedEvent> plannedEvents)
        {
            return plannedEvents.Count(plannedEvent => plannedEvent.Event.Period.ShouldCountForBreaches &&
                                                       (plannedEvent.BreachStatusAt(_clock.TodayDate) != null &&
                                                        plannedEvent.BreachStatusAt(_clock.TodayDate).Value ==
                                                        EventBreachStatus.Breached));
        }

        private Expression<Func<Event, bool>> GetCriteriaForEventHistoryLog(int periodId, EventHistoryLogFilterInputInfo eventHistoryLogFilterInputInfo)
        {
            return @event => @event.Period.Id == periodId &&
                      ((eventHistoryLogFilterInputInfo == null) ||
                      ((eventHistoryLogFilterInputInfo.TargetYear == null || (@event.TargetDate != null && @event.TargetDate.Value.Year == eventHistoryLogFilterInputInfo.TargetYear)) &&
                      (eventHistoryLogFilterInputInfo.ActualYear == null || @event.EventDate.Year == eventHistoryLogFilterInputInfo.ActualYear) &&
                      (eventHistoryLogFilterInputInfo.ImportYear == null || @event.EventDate.Year == eventHistoryLogFilterInputInfo.ImportYear) &&
                      (String.IsNullOrEmpty(eventHistoryLogFilterInputInfo.Description) || (@event.Comments!=null && @event.Comments.ToLowerInvariant().Contains(eventHistoryLogFilterInputInfo.Description.ToLowerInvariant()))) &&
                      (eventHistoryLogFilterInputInfo.EventsCode == null || (eventHistoryLogFilterInputInfo.EventsCode.Count == 0 && eventHistoryLogFilterInputInfo.EventCodeFilterValue == null) || eventHistoryLogFilterInputInfo.EventsCode.Contains(@event.Code))));
        }

        private IEnumerable<PeriodEventInfo> OrderPeriodEventsByProperty(IEnumerable<PeriodEventInfo> periodEventsInfo, ListInput listInput)
        {
            if (listInput == null || listInput.OrderBy == null || listInput.OrderDirection == null)
            {
                return periodEventsInfo;
            }
            switch (listInput.OrderBy)
            {
                case OrderBy.EventCode:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInput.OrderDirection, e => e.EventCode.ToString());
                case OrderBy.Specialty:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInput.OrderDirection, e => e.Specialty);
                case OrderBy.TargetDate:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInput.OrderDirection, e => e.TargetDate);
                case OrderBy.ActualDate:
                    return GetPeriodEventInfoOrderedByDirection(periodEventsInfo, listInput.OrderDirection, e => e.ActualDate);
            }
            return periodEventsInfo;
        }

        private IEnumerable<PeriodEventInfo> GetPeriodEventInfoOrderedByDirection<TKey>(IEnumerable<PeriodEventInfo> periodEventsInfo, OrderDirection? orderDirection, Func<PeriodEventInfo, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.Ascending
                ? periodEventsInfo.OrderBy(orderCriteria)
                : periodEventsInfo.OrderByDescending(orderCriteria);
        }

        private Expression<Func<Event, bool>> GetCriteriaForPatientsOnPathway(PatientFilterInputInfo patientFilterInputInfo)
        {
            if (patientFilterInputInfo != null && patientFilterInputInfo.NhsNumber != null)
            {
                return @event => GetEventForPatientWithFilters(patientFilterInputInfo, @event);
       
            }
            return @event => @event.IsActive &&
                             ((patientFilterInputInfo == null) ||
                             GetEventForPatientWithFilters(patientFilterInputInfo, @event));
        }

        private static bool GetEventForPatientWithFilters(PatientFilterInputInfo patientFilterInputInfo, Event @event)
        {
            return ((String.IsNullOrEmpty(patientFilterInputInfo.Hospital) || @event.Clinician.Hospital.Name.ToLowerInvariant().Contains(patientFilterInputInfo.Hospital.ToLowerInvariant())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.PatientName) || @event.Period.Pathway.Patient.Name.ToLowerInvariant().Contains(patientFilterInputInfo.PatientName.ToLowerInvariant())) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.PpiNumber) || @event.Period.Pathway.PPINumber.ToLowerInvariant() == patientFilterInputInfo.PpiNumber.ToLowerInvariant()) &&
                    (String.IsNullOrEmpty(patientFilterInputInfo.NhsNumber) || @event.Period.Pathway.Patient.NHSNumber.ToLowerInvariant() == patientFilterInputInfo.NhsNumber.ToLowerInvariant())) &&
                   (String.IsNullOrEmpty(patientFilterInputInfo.PeriodType) || @event.Period.GetType().Name.ToLowerInvariant() == patientFilterInputInfo.PeriodType.ToLowerInvariant());
        }


        private IEnumerable<EventBreachInfo> OrderBySpentDaysOrToGoDaysInPeriod(IEnumerable<EventBreachInfo> eventsBreachInfo, ListInput listInput)
        {
            if (listInput == null || listInput.OrderBy == null || listInput.OrderDirection == null)
            {
                return eventsBreachInfo;
            }
            switch (listInput.OrderBy)
            {
                case OrderBy.DaysInPeriod:
                    return GetEventBreachInfoOrderedByDirection(eventsBreachInfo, listInput.OrderDirection, e => e.DaysInPeriod);
                case OrderBy.DaysRemainingInPeriod:
                    return GetEventBreachInfoOrderedByDirection(eventsBreachInfo, listInput.OrderDirection, e => e.DaysRemainingInPeriod);
            }
            return eventsBreachInfo;
        }

        private IEnumerable<EventBreachInfo> GetEventBreachInfoOrderedByDirection<TKey>(IEnumerable<EventBreachInfo> eventsBreachInfo, OrderDirection? orderDirection, Func<EventBreachInfo, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.Ascending
                ? eventsBreachInfo.OrderBy(orderCriteria)
                : eventsBreachInfo.OrderByDescending(orderCriteria);
        }
    }
}
