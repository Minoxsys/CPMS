using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Patient.Domain;

namespace CPMS.Report.Manager
{
    public class ReportApplicationService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IClock _clock;

        private const int MaxPeriodWeekDistribution = 18;
        private const int BreachPeriodWeekDistribution = 0;

        public ReportApplicationService(IEventRepository eventRepository, IPeriodRepository periodRepository, IClock clock)
        {
            _eventRepository = eventRepository;
            _periodRepository = periodRepository;
            _clock = clock;
        }

        public IEnumerable<EventBreachesInfo> GetEventBreachesReport(DateTime fromDate, DateTime toDate,
            int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var events = GetEventsForBreachesCount(fromDate, hospitalId, specialtyCode, clinicianId, pathwayType).ToList();

            var eventsReport = new List<EventBreachesInfo>();

            for (var date = fromDate; date <= toDate; date = date.AddMonths(1))
            {
                var eventBreachesCount = GetEventBreachesCount(events, date, granularity);

                if (eventBreachesCount == null) continue;
                eventsReport.AddRange(eventBreachesCount.ToList());    
            }

            return eventsReport;
        }

        public IEnumerable<FuturePeriodBreachesInfo> GetFuturePeriodBreachesReport(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var events = GetEventsForFuturePeriodBreaches(weeksToBreach, hospitalId, specialtyCode, clinicianId, pathwayType)
                .Where(e =>
                    e.Period.GetDaysRemainingAt(_clock.Today) > 0 &&
                    e.Period.GetDaysRemainingAt(_clock.Today) <= weeksToBreach * 7)
                .ToList();

            var groupedEventsByGranularity = (from @event in events
                group @event by (granularity == Granularity.Specialty ? @event.Clinician.Specialty.Name :
                    (granularity == Granularity.Clinician ? @event.Clinician.Name :
                    @event.Clinician.Hospital.Name))
                into groupedEvents
                select new {groupedEvents.Key, Events = groupedEvents.ToList()})
            .ToDictionary(e => e.Key, e => e.Events);

            var futurePeriodBreachesReport = new List<FuturePeriodBreachesInfo>();

            foreach (var groupedEvents in groupedEventsByGranularity)
            {
                for (var weekToBreach = 1; weekToBreach <= weeksToBreach; weekToBreach++)
                {
                    var filteredEvents = groupedEvents.Value.Where(e =>
                        e.Period.GetDaysRemainingAt(_clock.Today) > (weekToBreach - 1) * 7 &&
                        e.Period.GetDaysRemainingAt(_clock.Today) <= weekToBreach * 7).ToList();

                    var @event = filteredEvents.Any() ? filteredEvents.FirstOrDefault() : groupedEvents.Value.FirstOrDefault();
                    if (@event != null)
                    {
                        futurePeriodBreachesReport.Add(new FuturePeriodBreachesInfo
                        {
                            Clinician = granularity == Granularity.Clinician ? @event.Clinician.Name : string.Empty,
                            Hospital = @event.Clinician.Hospital.Name,
                            Specialty = granularity == Granularity.Clinician || granularity == Granularity.Specialty ? @event.Clinician.Specialty.Name : string.Empty,
                            WeeksToBreach = weekToBreach,
                            NumberOfBreaches = filteredEvents.Any() ? filteredEvents.Count() : 0
                        });
                    }
                }
            }

            return futurePeriodBreachesReport;
        }

        public IEnumerable<ActivePeriodInfo> GetActivePeriodsDistributionReport(int hospitalId, string specialtyCode, int clinicianId, Granularity granularity, string pathwayType)
        {
            var periods = GetActivePeriods(hospitalId, specialtyCode, clinicianId, pathwayType).ToList();

            var groupedPeriodsByGranularity = (from period in periods
                let lastEvent = period.Events.LastOrDefault()
                where lastEvent != null
                group period by (granularity == Granularity.Specialty ? lastEvent.Clinician.Specialty.Name :
                    (granularity == Granularity.Clinician ? lastEvent.Clinician.Name :
                    lastEvent.Clinician.Hospital.Name))
                into groupedPeriods
                select new { groupedPeriods.Key, Periods = groupedPeriods.ToList() })
            .ToDictionary(e => e.Key, e => e.Periods);

            var activePeriodsDistribution = new List<ActivePeriodInfo>();

            foreach (var groupedPeriod in groupedPeriodsByGranularity)
            {
                for (var week = BreachPeriodWeekDistribution; week <= MaxPeriodWeekDistribution; week++)
                {
                    var filteredPeriods = (from period in groupedPeriod.Value
                        let daysInPeriod = period.GetDaysSpentAt(_clock.Today)
                        let periodWeek = (int) Math.Round((double) (daysInPeriod/7)) + 1
                        where
                            ((periodWeek == week) ||
                            (week == BreachPeriodWeekDistribution && periodWeek > MaxPeriodWeekDistribution))
                        select period)
                    .ToList();

                    var firstPeriod = filteredPeriods.Any() ? filteredPeriods.FirstOrDefault() : groupedPeriod.Value.FirstOrDefault();
                    if (firstPeriod != null)
                    {
                        var lastEvent = firstPeriod.Events.LastOrDefault();
                        if (lastEvent != null)
                        {
                            activePeriodsDistribution.Add(new ActivePeriodInfo
                            {
                                Clinician = lastEvent.Clinician.Name,
                                ClinicianId = lastEvent.Clinician.Id,
                                Specialty = lastEvent.Clinician.Specialty.Name,
                                SpecialtyCode = lastEvent.Clinician.Specialty.Code,
                                Hospital = lastEvent.Clinician.Hospital.Name,
                                HospitalId = lastEvent.Clinician.Hospital.Id,
                                NumberOfActivePeriods = filteredPeriods.Any() ? filteredPeriods.Count : 0,
                                Week = week > MaxPeriodWeekDistribution ? BreachPeriodWeekDistribution : week
                            });
                        }
                    }
                }
            }

            return activePeriodsDistribution;
        }

        private IEnumerable<Period> GetActivePeriods(int hospitalId, string specialtyCode, int clinicianId, string pathwayType)
        {
            return _periodRepository.Get(period => period.IsActive &&
                       (clinicianId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Id == clinicianId) &&
                       (String.IsNullOrEmpty(specialtyCode) || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Specialty.Code == specialtyCode) &&
                       (string.IsNullOrEmpty(pathwayType) || period.Pathway.PathwayType.ToString() == pathwayType) &&
                       (hospitalId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Hospital.Id == hospitalId));
        }

        private IEnumerable<Event> GetEventsForBreachesCount(DateTime fromDate, int hospitalId, string specialtyCode,
            int clinicianId, string pathwayType)
        {
            return _eventRepository.Get(ev => (hospitalId == 0 || ev.Clinician.Hospital.Id == hospitalId) &&
                                              (clinicianId == 0 || ev.Clinician.Id == clinicianId) &&
                                              (string.IsNullOrEmpty(pathwayType) || ev.Period.Pathway.PathwayType.ToString() == pathwayType) &&
                                              (string.IsNullOrEmpty(specialtyCode) ||
                                               ev.Clinician.Specialty.Code == specialtyCode) &&
                                              ev.EventDate >= fromDate
                , null);
        }

        private IEnumerable<Event> GetEventsForFuturePeriodBreaches(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, string pathwayType)
        {
            return _eventRepository.Get(@event => @event.IsActive &&
                @event.Period.ShouldCountForBreaches &&
                (GetCriteriaForPeriodsFilter(@event, hospitalId, specialtyCode, clinicianId, pathwayType) &&
                (@event.Period.StartDate.AddDays(@event.Period.MaximumNumberOfDays) >= _clock.Today.AddDays(-weeksToBreach * 7))), null);
        }

        private bool GetCriteriaForPeriodsFilter(Event @event, int hospitalId, string specialtyCode, int clinicianId, string pathwayType)
        {
            return ((clinicianId == 0 || @event.Clinician.Id == clinicianId) &&
                   (String.IsNullOrEmpty(specialtyCode) || String.Equals(@event.Clinician.Specialty.Code, specialtyCode, StringComparison.InvariantCultureIgnoreCase)) &&
                   (string.IsNullOrEmpty(pathwayType) || @event.Period.Pathway.PathwayType.ToString() == pathwayType) &&
                   (hospitalId == 0 || @event.Clinician.Hospital.Id == hospitalId));
        }

        private DateTime GetFirstDateOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        private DateTime GetLastDateOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        private IEnumerable<EventBreachesInfo> GetEventBreachesCount(IList<Event> events, DateTime date,
            Granularity? granularity)
        {
            if (!events.Any()) return null;

            var firstDateOfMonth = GetFirstDateOfMonth(date);
            var lastDateOfMonth = GetLastDateOfMonth(date);

            var eventBreaches = new List<EventBreachesInfo>();

            switch (granularity)
            {
                case Granularity.Specialty:
                {
                    var eventsGroupedBySpecialty = from @event in events
                        where @event != null
                        group @event by @event.Clinician.Specialty.Name
                        into groupedEvents
                        select new {SpecialtyId = groupedEvents.Key, Events = groupedEvents.ToList()};

                    foreach (var eventBreachInfo in from eventGroupedBySpecialty in eventsGroupedBySpecialty let eventsGroupedByHospital = (from @event in eventGroupedBySpecialty.Events
                        where @event != null
                        group @event by @event.Clinician.Hospital.Name
                        into groupedEvents
                        select new {HospitalId = groupedEvents.Key, Events = groupedEvents.ToList()}).ToList() from eventGroupedByHospital in eventsGroupedByHospital select new EventBreachesInfo
                        {
                            Clinician = string.Empty,
                            Hospital = eventGroupedByHospital.HospitalId,
                            Specialty = eventGroupedBySpecialty.SpecialtyId,
                            Month = date.Month,
                            Year = date.Year,
                            InpatientCompletedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                p =>
                                    p.IsActive == false && p.BreachStatus == EventBreachStatus.Success &&
                                    p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                            InpatientBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                p =>
                                    p.IsActive == false && p.BreachStatus == EventBreachStatus.Breached &&
                                    p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                            OpenedBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                p =>
                                    p.BreachStatus == EventBreachStatus.Breached && p.IsActive &&
                                    p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                            OpenedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                p =>
                                    p.IsActive && p.BreachStatus != EventBreachStatus.Breached &&
                                    p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth)

                        })
                    {
                        eventBreachInfo.BreachedPeriodsNumber =
                            eventBreachInfo.InpatientBreachedPeriodsNumber +
                            eventBreachInfo.OpenedBreachedPeriodsNumber;
                        eventBreachInfo.PeriodsNumber = eventBreachInfo.InpatientBreachedPeriodsNumber +
                                                        eventBreachInfo.InpatientCompletedPeriodsNumber +
                                                        eventBreachInfo.OpenedBreachedPeriodsNumber +
                                                        eventBreachInfo.OpenedPeriodsNumber;

                        eventBreaches.Add(eventBreachInfo);
                    }


                    break;
                }
                case Granularity.Clinician:
                {
                    var eventsGroupedByClinician = (from @event in events
                        where @event != null
                        group @event by @event.Clinician.Name
                        into groupedEvents
                        select new {ClinicianId = groupedEvents.Key, Events = groupedEvents.ToList()}).ToList();

                    foreach (var eventBreachInfo in from eventGroupedByClinician in eventsGroupedByClinician let eventsGroupedBySpecialty = (from @event in eventGroupedByClinician.Events
                        where @event != null
                        group @event by @event.Clinician.Specialty.Name
                        into groupedEvents
                        select new {SpecialtyId = groupedEvents.Key, Events = groupedEvents.ToList()}) from eventGroupedBySpecialty in eventsGroupedBySpecialty let eventsGroupedByHospital = (from @event in eventGroupedBySpecialty.Events
                            where @event != null
                            group @event by @event.Clinician.Hospital.Name
                            into groupedEvents
                            select new {HospitalId = groupedEvents.Key, Events = Enumerable.ToList<Event>(groupedEvents)}).ToList() from eventGroupedByHospital in eventsGroupedByHospital select new EventBreachesInfo
                            {
                                Clinician = eventGroupedByClinician.ClinicianId,
                                Hospital = eventGroupedByHospital.HospitalId,
                                Specialty = eventGroupedBySpecialty.SpecialtyId,
                                Month = date.Month,
                                Year = date.Year,
                                InpatientCompletedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive == false && p.BreachStatus == EventBreachStatus.Success &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                InpatientBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive == false && p.BreachStatus == EventBreachStatus.Breached &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                OpenedBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.BreachStatus == EventBreachStatus.Breached && p.IsActive &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                OpenedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive && p.BreachStatus != EventBreachStatus.Breached &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth)

                            })
                    {
                        eventBreachInfo.BreachedPeriodsNumber =
                            eventBreachInfo.InpatientBreachedPeriodsNumber +
                            eventBreachInfo.OpenedBreachedPeriodsNumber;
                        eventBreachInfo.PeriodsNumber = eventBreachInfo.InpatientBreachedPeriodsNumber +
                                                        eventBreachInfo.InpatientCompletedPeriodsNumber +
                                                        eventBreachInfo.OpenedBreachedPeriodsNumber +
                                                        eventBreachInfo.OpenedPeriodsNumber;

                        eventBreaches.Add(eventBreachInfo);
                    }
                    break;
                }
                default:
                {
                    var eventsGroupedByHospital = (from @event in events
                        where @event != null
                        group @event by @event.Clinician.Hospital.Name
                        into groupedEvents
                        select new {HospitalId = groupedEvents.Key, Events = groupedEvents.ToList()}).ToList();

                    foreach (
                        var eventBreachInfo in
                            eventsGroupedByHospital.Select(eventGroupedByHospital => new EventBreachesInfo
                            {
                                Clinician = string.Empty,
                                Hospital = eventGroupedByHospital.HospitalId,
                                Specialty = string.Empty,
                                Month = date.Month,
                                Year = date.Year,
                                InpatientCompletedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive == false && p.BreachStatus == EventBreachStatus.Success &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                InpatientBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive == false && p.BreachStatus == EventBreachStatus.Breached &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                OpenedBreachedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.BreachStatus == EventBreachStatus.Breached && p.IsActive &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth),
                                OpenedPeriodsNumber = eventGroupedByHospital.Events.Count(
                                    p =>
                                        p.IsActive && p.BreachStatus != EventBreachStatus.Breached &&
                                        p.EventDate >= firstDateOfMonth && p.EventDate <= lastDateOfMonth)
                            }))
                    {
                        eventBreachInfo.BreachedPeriodsNumber =
                            eventBreachInfo.InpatientBreachedPeriodsNumber +
                            eventBreachInfo.OpenedBreachedPeriodsNumber;
                        eventBreachInfo.PeriodsNumber = eventBreachInfo.InpatientBreachedPeriodsNumber +
                                                        eventBreachInfo.InpatientCompletedPeriodsNumber +
                                                        eventBreachInfo.OpenedBreachedPeriodsNumber +
                                                        eventBreachInfo.OpenedPeriodsNumber;

                        eventBreaches.Add(eventBreachInfo);
                    }
                }
                    break;
            }

            return eventBreaches;
        }
    }
}
