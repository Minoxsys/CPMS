using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class ReportApplicationService
    {
        private readonly ICompletedEventRepository _completedEventRepository;
        private readonly IEventMilestoneRepository _eventMilestoneRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IClinicianRepository _clinicianRepository;
        private readonly IEventNameRepository _eventNameRepository;
        private readonly IClock _clock;
        private readonly MonthlyPeriodBreachesReportService _monthlyPeriodBreachesReportService;
        private readonly EventBreachesReportService _eventBreachesReportService;
        private readonly PeriodBreachesReportService _periodBreachesReportService;
        private readonly PeriodAndEventBreachesCountReportService _periodAndEventBreachesCountReportService;
        private readonly PeriodBreachesCounterReportService _periodBreachesCounterReportService;

        private const int OneWeek = 7;
        private const int EighteenWeekPeriod = 18;
        private const int ThreeWeeks = 21;
        private const int BreachPeriodWeekDistribution = 0;

        public ReportApplicationService(
            ICompletedEventRepository completedEventRepository,
            IEventMilestoneRepository eventMilestoneRepository,
            IPeriodRepository periodRepository,
            IHospitalRepository hospitalRepository,
            ISpecialtyRepository specialtyRepository,
            IClinicianRepository clinicianRepository,
            IEventNameRepository eventNameRepository,
            IClock clock,
            MonthlyPeriodBreachesReportService monthlyPeriodBreachesReportService,
            EventBreachesReportService eventBreachesReportService,
            PeriodBreachesReportService periodBreachesReportService,
            PeriodAndEventBreachesCountReportService periodAndEventBreachesCountReportService,
            PeriodBreachesCounterReportService periodBreachesCounterReportService)
        {
            _completedEventRepository = completedEventRepository;
            _eventMilestoneRepository = eventMilestoneRepository;
            _periodRepository = periodRepository;
            _hospitalRepository = hospitalRepository;
            _specialtyRepository = specialtyRepository;
            _clinicianRepository = clinicianRepository;
            _eventNameRepository = eventNameRepository;
            _clock = clock;
            _monthlyPeriodBreachesReportService = monthlyPeriodBreachesReportService;
            _eventBreachesReportService = eventBreachesReportService;
            _periodBreachesReportService = periodBreachesReportService;
            _periodAndEventBreachesCountReportService = periodAndEventBreachesCountReportService;
            _periodBreachesCounterReportService = periodBreachesCounterReportService;
        }

        public IEnumerable<Monthly18wRTTPerformanceInfo> GetMonthly18wRTTPerformanceReport(DateTime fromDate, DateTime toDate,
            int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periods = _periodRepository.Get(period =>
                                (clinicianId == 0 || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Id == clinicianId) &&
                                (String.IsNullOrEmpty(specialtyCode) || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Specialty.Code == specialtyCode) &&
                                (hospitalId == 0 || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Hospital.Id == hospitalId),
                            fromDate,
                            p => p.CompletedEvents.Select(e => e.Name),
                            p => p.CompletedEvents.Select(e => e.Clinician.Specialty),
                            p => p.CompletedEvents.Select(e => e.Clinician.Hospital))
                        .ToList();

            var periodsReport = new List<Monthly18wRTTPerformanceInfo>();

            for (var date = fromDate; date <= toDate; date = date.AddMonths(1))
            {
                var monthlyPeriodBreaches = _monthlyPeriodBreachesReportService.GetMonthlyPeriodBreaches(periods, date, granularity);
                if (monthlyPeriodBreaches != null)
                {
                    periodsReport.AddRange(monthlyPeriodBreaches);
                }
            }

            return periodsReport;
        }

        public IEnumerable<FuturePeriodBreachesInfo> GetFuturePeriodBreachesReport(int weeksToBreach, int hospitalId,
            string specialtyCode, int clinicianId, Granularity granularity)
        {
            var fromDate = _clock.Today.AddDays(-weeksToBreach*OneWeek);
            var completedEvents = _completedEventRepository.Get(completedEvent => completedEvent.IsActive &&
                                                  (clinicianId == 0 || completedEvent.Clinician.Id == clinicianId) &&
                                                  (string.IsNullOrEmpty(specialtyCode) || completedEvent.Clinician.Specialty.Code == specialtyCode) &&
                                                  (hospitalId == 0 || completedEvent.Clinician.Hospital.Id == hospitalId) &&
                                                  completedEvent.Period.ShouldCountForBreaches,
                                                  null,
                                                  fromDate,
                                                  e => e.Period.CompletedEvents.Select(completedEvent => completedEvent.Name),
                                                  e => e.Clinician.Specialty,
                                                  e => e.Clinician.Hospital)
                .Where(completedEvent =>
                    completedEvent.Period.GetDaysRemainingAt(_clock.Today) > 0 &&
                    completedEvent.Period.GetDaysRemainingAt(_clock.Today) <= weeksToBreach * OneWeek)
                .ToArray();

            var groupedEventsByGranularity = (from completedEvent in completedEvents
                                              group completedEvent by (granularity == Granularity.Specialty ? completedEvent.Clinician.Specialty.Name :
                                                  (granularity == Granularity.Clinician ? completedEvent.Clinician.Name :
                                                  completedEvent.Clinician.Hospital.Name))
                                                  into groupedEvents
                                                  select new { groupedEvents.Key, Events = groupedEvents.ToList() })
            .ToDictionary(e => e.Key, e => e.Events);

            var futurePeriodBreachesReport = new List<FuturePeriodBreachesInfo>();

            foreach (var groupedEvents in groupedEventsByGranularity)
            {
                for (var weekToBreach = 1; weekToBreach <= weeksToBreach; weekToBreach++)
                {
                    var filteredEvents = groupedEvents.Value.Where(e =>
                        e.Period.GetDaysRemainingAt(_clock.Today) > (weekToBreach - 1) * OneWeek &&
                        e.Period.GetDaysRemainingAt(_clock.Today) <= weekToBreach * OneWeek).ToList();

                    var completedEvent = filteredEvents.Any() ? filteredEvents.FirstOrDefault() : groupedEvents.Value.FirstOrDefault();
                    if (completedEvent != null)
                    {
                        futurePeriodBreachesReport.Add(new FuturePeriodBreachesInfo
                        {
                            Clinician = granularity == Granularity.Clinician ? completedEvent.Clinician.Name : string.Empty,
                            Hospital = completedEvent.Clinician.Hospital.Name,
                            Specialty = granularity == Granularity.Clinician || granularity == Granularity.Specialty ? completedEvent.Clinician.Specialty.Name : string.Empty,
                            WeeksToBreach = weekToBreach,
                            NumberOfBreaches = filteredEvents.Any() ? filteredEvents.Count() : 0
                        });
                    }
                }
            }

            return futurePeriodBreachesReport;
        }

        public IEnumerable<ActivePeriodInfo> GetActivePeriodsDistributionReport(int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periods = GetActivePeriods(hospitalId, specialtyCode, clinicianId).ToList();

            var groupedPeriodsByGranularity = (from period in periods
                                               let lastCompletedEvent = period.CompletedEvents.LastOrDefault()
                                               where lastCompletedEvent != null
                                               group period by (granularity == Granularity.Specialty ? lastCompletedEvent.Clinician.Specialty.Name :
                                                   (granularity == Granularity.Clinician ? lastCompletedEvent.Clinician.Name :
                                                   lastCompletedEvent.Clinician.Hospital.Name))
                                                   into groupedPeriods
                                                   select new { groupedPeriods.Key, Periods = groupedPeriods.ToList() })
            .ToDictionary(e => e.Key, e => e.Periods);

            var activePeriodsDistribution = new List<ActivePeriodInfo>();

            foreach (var groupedPeriod in groupedPeriodsByGranularity)
            {
                for (var week = BreachPeriodWeekDistribution; week <= EighteenWeekPeriod; week++)
                {
                    var filteredPeriods = (from period in groupedPeriod.Value
                                           let daysInPeriod = period.GetDaysSpentAt(_clock.Today)
                                           let periodWeek = (int)Math.Round((double)(daysInPeriod / 7)) + 1
                                           where
                                               ((periodWeek == week) ||
                                               (week == BreachPeriodWeekDistribution && periodWeek > EighteenWeekPeriod))
                                           select period)
                    .ToList();

                    var firstPeriod = filteredPeriods.Any() ? filteredPeriods.FirstOrDefault() : groupedPeriod.Value.FirstOrDefault();
                    if (firstPeriod != null)
                    {
                        var lastCompletedEvent = firstPeriod.CompletedEvents.LastOrDefault();
                        if (lastCompletedEvent != null)
                        {
                            activePeriodsDistribution.Add(new ActivePeriodInfo
                            {
                                Clinician = lastCompletedEvent.Clinician.Name,
                                ClinicianId = lastCompletedEvent.Clinician.Id,
                                Specialty = lastCompletedEvent.Clinician.Specialty.Name,
                                SpecialtyCode = lastCompletedEvent.Clinician.Specialty.Code,
                                Hospital = lastCompletedEvent.Clinician.Hospital.Name,
                                HospitalId = lastCompletedEvent.Clinician.Hospital.Id,
                                NumberOfActivePeriods = filteredPeriods.Any() ? filteredPeriods.Count : 0,
                                Week = week > EighteenWeekPeriod ? BreachPeriodWeekDistribution : week
                            });
                        }
                    }
                }
            }

            return activePeriodsDistribution;
        }

        public EventBreachesInfo GetEventBreachesReport(int daysToBreach, BreachFilterInputInfo filterInputInfo, ListInputInfo listInputInfo)
        {
            var eventMilestones = _eventMilestoneRepository
                .Get(_eventBreachesReportService.GetCriteria(filterInputInfo),
                    filterInputInfo.PeriodType,
                    e => e.Name,
                    e => e.CompletedEvent.Period.CompletedEvents.Select(completedEvent => completedEvent.Name),
                    e => e.CompletedEvent.Period.Pathway.Patient,
                    e => e.CompletedEvent.Clinician.Specialty)
                .Where(_eventBreachesReportService.GetFilter(daysToBreach, filterInputInfo.AdvancementOrDelay)).ToList();

            var totalNumberOfEventBreaches = eventMilestones.Count;

            eventMilestones = _eventBreachesReportService.ApplyPaginationAndOrdering(eventMilestones, listInputInfo).ToList();

            var eventBreachesList = (
                from eventMilestone in eventMilestones
                let postBreachDays = eventMilestone.GetDaysToBreachAt(_clock.Today)
                let daysRemaining = eventMilestone.CompletedEvent.Period.GetDaysRemainingAt(_clock.Today)
                where postBreachDays != null
                select new EventBreachInfo
                {
                    EventDescription = eventMilestone.Name.Description,
                    PatientNHSNumber = eventMilestone.CompletedEvent.Period.Pathway.Patient.NHSNumber,
                    PatientName = eventMilestone.CompletedEvent.Period.Pathway.Patient.Name,
                    PPINumber = eventMilestone.CompletedEvent.Period.Pathway.PPINumber,
                    Clinician = eventMilestone.CompletedEvent.Clinician.Name,
                    Specialty = eventMilestone.CompletedEvent.Clinician.Specialty.Name,
                    PostBreachDays = Math.Abs((int)postBreachDays),
                    DaysInPeriod = eventMilestone.CompletedEvent.Period.GetDaysSpentAt(_clock.Today),
                    DaysRemainingInPeriod = daysRemaining < 0 ? 0 : daysRemaining,
                    DelayOrAdvancementDays = eventMilestone.CompletedEvent.Period.GetDelayOrAdvancementDays(_clock.Today),
                    PeriodType = eventMilestone.CompletedEvent.Period.GetType().Name,
                    EventType = eventMilestone.CompletedEvent.GetType().Name,
                    PeriodId = eventMilestone.CompletedEvent.Period.Id
                }).ToArray();

            return new EventBreachesInfo
            {
                EventBreaches = eventBreachesList,
                TotalNumberOfEventBreaches = totalNumberOfEventBreaches
            };
        }

        public PeriodBreachesInfo GetPeriodBreachesReport(int weeksToBreach, BreachFilterInputInfo filterInputInfo, ListInputInfo listInputInfo)
        {
            var completedEvents = _completedEventRepository
                .Get(_periodBreachesReportService.GetCriteria(filterInputInfo),
                    filterInputInfo.PeriodType,
                    null,
                    e => e.Name,
                    e => e.Period.CompletedEvents,
                    e => e.Period.Pathway.Patient,
                    e => e.Clinician.Specialty)
                .Where(_periodBreachesReportService.GetFilter(weeksToBreach))
                .ToList();

            var totalNumberOfPeriodBreaches = completedEvents.Count;

            completedEvents = _periodBreachesReportService.ApplyPaginationAndOrdering(completedEvents, listInputInfo).ToList();

            var periodsInfoList = (
                from completedEvent in completedEvents
                let postBreachDays = completedEvent.Period.GetDaysRemainingAt(_clock.Today)
                let daysRemaining = completedEvent.Period.GetDaysRemainingAt(_clock.Today)
                select new PeriodBreachInfo
                {
                    EventDescription = completedEvent.Name.Description,
                    PatientNHSNumber = completedEvent.Period.Pathway.Patient.NHSNumber,
                    PatientName = completedEvent.Period.Pathway.Patient.Name,
                    PPINumber = completedEvent.Period.Pathway.PPINumber,
                    Clinician = completedEvent.Clinician.Name,
                    Specialty = completedEvent.Clinician.Specialty.Name,
                    PostBreachDays = Math.Abs(postBreachDays),
                    DaysInPeriod = completedEvent.Period.GetDaysSpentAt(_clock.Today),
                    DaysRemainingInPeriod = daysRemaining < 0 ? 0 : daysRemaining,
                    PeriodType = completedEvent.Period.GetType().Name,
                    EventType = completedEvent.GetType().Name,
                    PeriodId = completedEvent.Period.Id
                }).ToArray();

            return new PeriodBreachesInfo
            {
                PeriodsInfo = periodsInfoList,
                TotalNumberOfPeriodBreaches = totalNumberOfPeriodBreaches
            };
        }

        public PeriodsAndEventsPerformanceInfo GetPeriodsAndEventsPerformanceReport()
        {
            var eventMilestones =
                _eventMilestoneRepository.Get(eventMilestone =>
                    eventMilestone.CompletedEvent.IsActive &&
                    eventMilestone.IsMandatory &&
                    eventMilestone.CompletedEvent.Period.ShouldCountForBreaches,
                null,
                e => e.CompletedEvent.Period.CompletedEvents.Select(completedEvent => completedEvent.Name))
            .ToArray();

            var completedEvents = _completedEventRepository.Get(
                completedEvent => completedEvent.IsActive &&
                completedEvent.Period.ShouldCountForBreaches,
                null, null,
                e => e.Name,
                e => e.Period.CompletedEvents)
            .ToArray();

            return new PeriodsAndEventsPerformanceInfo
            {
                EventsPerformanceInfo = new EventsPerformanceInfo
                {
                    ThreeDays = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(3)),
                    TwoDays = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(2)),
                    OneDays = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(1)),
                    Breach = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(0)),
                    PostBreach = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(-1)),
                    AboutToBreach = eventMilestones.Count(_periodAndEventBreachesCountReportService.GetFilterForEventBreaches(ThreeWeeks + 1))
                },
                PeriodsPerformanceInfo = new PeriodsPerformanceInfo
                {
                    FourWeeks = completedEvents.Count(_periodAndEventBreachesCountReportService.GetFilterForPeriodBreaches(4)),
                    ThreeWeeks = completedEvents.Count(_periodAndEventBreachesCountReportService.GetFilterForPeriodBreaches(3)),
                    TwoWeeks = completedEvents.Count(_periodAndEventBreachesCountReportService.GetFilterForPeriodBreaches(2)),
                    OneWeek = completedEvents.Count(_periodAndEventBreachesCountReportService.GetFilterForPeriodBreaches(1)),
                    PostBreach = completedEvents.Count(_periodAndEventBreachesCountReportService.GetFilterForPeriodBreaches(-1))
                }
            };
        }

        public IEnumerable<EventPerformanceInfo> GetEventPerformanceReport()
        {
            var eventNames = _eventNameRepository.Get();

            return from eventName in (eventNames)
                where
                    eventName.Code != EventCode.ReferralReceived && eventName.Code != EventCode.PathwayCompletion &&
                    eventName.Code != EventCode.PatientCancelEvent && eventName.Code != EventCode.HospitalCancelEvent &&
                    eventName.Code != EventCode.ReferralReview && eventName.Code != EventCode.DiagnosticsOrderPlaced &&
                    eventName.Code != EventCode.BookedOutpatientFollowUpAppointment &&
                    eventName.Code != EventCode.BookedPostInpatientFirstOutpatientAppointment &&
                    eventName.Code != EventCode.BookedPostInpatientSecondOutpatientAppointment &&
                    eventName.Code != EventCode.DidNotAttend && eventName.Code != EventCode.PatientCancelEvent &&
                    eventName.Code != EventCode.HospitalCancelEvent
                let eventMilestones = _eventMilestoneRepository.Get(
                        eventMilestone => eventMilestone.Name.Code == eventName.Code && eventMilestone.CompletedEvent.IsActive,
                        null,
                        eventMilestone => eventMilestone.CompletedEvent.Period)
                    .ToArray()
                let breachedEventMilestones = eventMilestones
                    .Where(eventMilestone =>
                    {
                        var eventBreachStatus = eventMilestone.BreachStatusAt(_clock.Today);
                        return eventBreachStatus != null &&
                               eventMilestone.CompletedEvent.Period.ShouldCountForBreaches &&
                               eventBreachStatus.Value == EventBreachStatus.Breached;
                    })
                let completedEvents = _completedEventRepository.Get(
                        completedEvent => eventName.Code == completedEvent.Name.Code && completedEvent.IsActive,
                        null, null,
                        completedEvent => completedEvent.Period)
                    .ToArray()
                let breachedCompletedEvents = completedEvents
                    .Where(completedEvent =>
                        completedEvent.Period.ShouldCountForBreaches &&
                        completedEvent.BreachStatus == EventBreachStatus.Breached)
                select new EventPerformanceInfo
                {
                    EventDescription = eventName.Description,
                    BreachedCompletedEventsNumber = breachedCompletedEvents.Count(),
                    BreachedEventMilestonesNumber = breachedEventMilestones.Count(),
                    TotalCompletedEventsNumber = completedEvents.Count(),
                    TotalEventMilestonesNumber = eventMilestones.Count()
                };
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport()
        {
            var hospitals = _hospitalRepository.Get().ToArray();
            var hospitalIds = hospitals.Select(hospital => hospital.Id);

            var allCompletedEvents = _completedEventRepository
                .Get(completedEvent =>
                        hospitalIds.Contains(completedEvent.Clinician.Hospital.Id) &&
                        completedEvent.Period.ShouldCountForBreaches,
                    null, null,
                    completedEvent => completedEvent.Period.CompletedEvents.Select(e => e.Name),
                    completedEvent => completedEvent.Clinician.Hospital)
                .ToList();

            return from hospitalId in hospitalIds
                let completedEventsForHospital = allCompletedEvents
                    .Where(completedEventForHospital => completedEventForHospital.Clinician.Hospital.Id == hospitalId)
                    .ToList()
                let periodBreachesCounter = _periodBreachesCounterReportService
                    .GetPeriodBreachesCounterInfo(completedEventsForHospital, hospitals.Single(hospital => hospital.Id == hospitalId).Name, hospitalId.ToString(CultureInfo.InvariantCulture))
                where periodBreachesCounter != null
                select periodBreachesCounter;
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport(string hospitalId)
        {
            var id = Int32.Parse(hospitalId);
            var specialties = _specialtyRepository
                .Get(specialty => specialty.Hospitals.Any(hospital => hospital.Id == id),
                    specialty => specialty.Hospitals)
                .ToArray();
            var specialtyCodes = specialties.Select(specialty => specialty.Code);

            var allCompletedEvents = _completedEventRepository
                .Get(completedEvent =>
                        specialtyCodes.Contains(completedEvent.Clinician.Specialty.Code) &&
                        completedEvent.Clinician.Hospital.Id == id &&
                        completedEvent.Period.ShouldCountForBreaches,
                    null, null,
                    completedEvent => completedEvent.Period,
                    completedEvent => completedEvent.Clinician.Specialty)
                .ToArray();

            return from specialtyId in specialtyCodes
                let completedEventsForSpecialty = allCompletedEvents
                    .Where(completedEventForSpecialty => completedEventForSpecialty.Clinician.Specialty.Code == specialtyId)
                    .ToList()
                let periodBreachesCounter = _periodBreachesCounterReportService
                    .GetPeriodBreachesCounterInfo(completedEventsForSpecialty, specialties.Single(specialty => specialty.Code == specialtyId).Name, specialtyId)
                where periodBreachesCounter != null
                select periodBreachesCounter;
        }

        public IEnumerable<PeriodPerformanceInfo> GetPeriodPerformanceReport(string hospitalId, string specialtyCode)
        {
            var parsedHospitalId = int.Parse(hospitalId);
            var clinicians = _clinicianRepository
                    .Get(clinician =>
                            clinician.Specialty.Code == specialtyCode &&
                            ((string.IsNullOrEmpty(hospitalId)) || clinician.Hospital.Id == parsedHospitalId))
                    .ToArray();

            var clinicianIds = clinicians.Select(clinician => clinician.Id);

            var allCompletedEvents = _completedEventRepository
                .Get(completedEvent => 
                        clinicianIds.Contains(completedEvent.Clinician.Id) &&
                        completedEvent.Period.ShouldCountForBreaches,
                    null, null,
                    completedEvent => completedEvent.Period,
                    completedEvent => completedEvent.Clinician)
                .ToArray();

            return from clinicianId in clinicianIds
                let completedEventsForClinician = allCompletedEvents
                    .Where(completedEventForClinician => completedEventForClinician.Clinician.Id == clinicianId)
                    .ToList()
                let periodBreachesCounter = _periodBreachesCounterReportService
                    .GetPeriodBreachesCounterInfo(completedEventsForClinician, clinicians.Single(clinician => clinician.Id == clinicianId).Name, clinicianId.ToString(CultureInfo.InvariantCulture))
                where periodBreachesCounter != null
                select periodBreachesCounter;
        }

        private IEnumerable<Period> GetActivePeriods(int hospitalId, string specialtyCode, int clinicianId)
        {
            return _periodRepository.Get(period => period.IsActive &&
                       (clinicianId == 0 || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Id == clinicianId) &&
                       (String.IsNullOrEmpty(specialtyCode) || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Specialty.Code == specialtyCode) &&
                       (hospitalId == 0 || period.CompletedEvents.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Hospital.Id == hospitalId),
                    null,
                    p => p.CompletedEvents.Select(e => e.Clinician.Specialty),
                    p => p.CompletedEvents.Select(e => e.Clinician.Hospital));
        }
    }
}
