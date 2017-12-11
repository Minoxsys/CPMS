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

        public IEnumerable<PeriodBreachesInfo> GetPeriodBreachesReport(DateTime fromDate, DateTime toDate,
            int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periods = GetPeriodsForBreachesCount(fromDate, hospitalId, specialtyCode, clinicianId).ToList();

            var periodsReport = new List<PeriodBreachesInfo>();

            for (var date = fromDate; date <= toDate; date = date.AddMonths(1))
            {
                var periodBreachesCount = GetPeriodBreachesCount(periods, date, granularity).ToList();
                if (periodBreachesCount.Any())
                {
                    periodsReport.AddRange(periodBreachesCount);
                }
            }

            return periodsReport;
        }

        public IEnumerable<FuturePeriodBreachesInfo> GetFuturePeriodBreachesReport(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var events = GetEventsForFuturePeriodBreaches(weeksToBreach, hospitalId, specialtyCode, clinicianId)
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

        public IEnumerable<ActivePeriodInfo> GetActivePeriodsDistributionReport(int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            var periods = GetActivePeriods(hospitalId, specialtyCode, clinicianId).ToList();

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

        private IEnumerable<Period> GetActivePeriods(int hospitalId, string specialtyCode, int clinicianId)
        {
            return _periodRepository.Get(period => period.IsActive &&
                       (clinicianId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Id == clinicianId) &&
                       (String.IsNullOrEmpty(specialtyCode) || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Specialty.Code == specialtyCode) &&
                       (hospitalId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Hospital.Id == hospitalId));
        }

        private IEnumerable<Period> GetPeriodsForBreachesCount(DateTime fromDate, int hospitalId, string specialtyCode, int clinicianId)
        {
            return _periodRepository.Get(period =>
                       (clinicianId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Id == clinicianId) &&
                       (String.IsNullOrEmpty(specialtyCode) || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Specialty.Code == specialtyCode) &&
                       (hospitalId == 0 || period.Events.OrderByDescending(e => e.Id).FirstOrDefault().Clinician.Hospital.Id == hospitalId), fromDate);
        }

        private IEnumerable<Event> GetEventsForFuturePeriodBreaches(int weeksToBreach, int hospitalId, string specialtyCode, int clinicianId)
        {
            return _eventRepository.Get(@event => @event.IsActive &&
                @event.Period.ShouldCountForBreaches &&
                (GetCriteriaForPeriodsFilter(@event, hospitalId, specialtyCode, clinicianId) &&
                (@event.Period.StartDate.AddDays(@event.Period.MaximumNumberOfDays) >= _clock.Today.AddDays(-weeksToBreach * 7))), null);
        }

        private bool GetCriteriaForPeriodsFilter(Event @event, int hospitalId, string specialtyCode, int clinicianId)
        {
            return ((clinicianId == 0 || @event.Clinician.Id == clinicianId) &&
                   (String.IsNullOrEmpty(specialtyCode) || String.Equals(@event.Clinician.Specialty.Code, specialtyCode, StringComparison.InvariantCultureIgnoreCase)) &&
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

        private IEnumerable<PeriodBreachesInfo> GetPeriodBreachesCount(IList<Period> periods, DateTime date, Granularity? granularity)
        {
            if (!periods.Any()) return null;

            var firstDateOfMonth = GetFirstDateOfMonth(date);
            var lastDateOfMonth = GetLastDateOfMonth(date);

            var periodBreaches = new List<PeriodBreachesInfo>();

            switch (granularity)
            {
                case Granularity.Specialty:
                    {
                        var periodsGroupedBySpecialty = from period in periods
                                                        let lastEvent = period.Events.LastOrDefault()
                                                        where lastEvent != null
                                                        group period by lastEvent.Clinician.Specialty
                                                            into groupedPeriods
                                                            select new { SpecialtyId = groupedPeriods.Key, Periods = groupedPeriods.ToList() };
                        foreach (var periodGroupedBySpecialty in periodsGroupedBySpecialty)
                        {
                            var period = periodGroupedBySpecialty.Periods.FirstOrDefault();
                            if (period != null && period.MaximumNumberOfDays > 0)
                            {
                                var lastEvent = period.Events.LastOrDefault();
                                if (lastEvent != null)
                                {
                                    var periodBreachInfo = new PeriodBreachesInfo
                                    {
                                        Clinician = string.Empty,
                                        Hospital = lastEvent.Clinician.Hospital.Name,
                                        Specialty = lastEvent.Clinician.Specialty.Name,
                                        Month = date.Month,
                                        Year = date.Year,
                                        InpatientCompletedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 p.HasAdmittedPatient()),
                                        OutpatientCompletedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 !p.HasAdmittedPatient()),
                                        InpatientBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 p.HasAdmittedPatient()),
                                        OutpatientBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 !p.HasAdmittedPatient()),
                                        OpenedBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                                 p.StartDate <= lastDateOfMonth),
                                        OpenedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                                 p.StartDate <= lastDateOfMonth)
                                    };
                                    periodBreachInfo.BreachedPeriodsNumber =
                                        periodBreachInfo.InpatientBreachedPeriodsNumber +
                                        periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                        periodBreachInfo.OpenedBreachedPeriodsNumber;

                                    periodBreachInfo.PeriodsNumber = periodBreachInfo.InpatientBreachedPeriodsNumber +
                                                                     periodBreachInfo.OutpatientCompletedPeriodsNumber +
                                                                     periodBreachInfo.InpatientCompletedPeriodsNumber +
                                                                     periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                                                     periodBreachInfo.OpenedBreachedPeriodsNumber +
                                                                     periodBreachInfo.OpenedPeriodsNumber;

                                    periodBreaches.Add(periodBreachInfo);
                                }
                            }
                        }

                        break;
                    }
                case Granularity.Clinician:
                    {
                        var periodsGroupedByClinician = (from period in periods
                                                        let lastEvent = period.Events.LastOrDefault()
                                                        where lastEvent != null
                                                        group period by lastEvent.Clinician.Name
                                                        into groupedPeriods
                                                        select new { ClinicianId = groupedPeriods.Key, Periods = groupedPeriods.ToList() }).ToList();

                        foreach (var periodGroupedBySpecialty in periodsGroupedByClinician)
                        {
                            var period = periodGroupedBySpecialty.Periods.FirstOrDefault();
                            if (period != null && period.MaximumNumberOfDays > 0)
                            {
                                var lastEvent = period.Events.LastOrDefault();
                                if (lastEvent != null)
                                {
                                    var periodBreachInfo = new PeriodBreachesInfo
                                    {
                                        Clinician = lastEvent.Clinician.Name,
                                        Hospital = lastEvent.Clinician.Hospital.Name,
                                        Specialty = lastEvent.Clinician.Specialty.Name,
                                        Month = date.Month,
                                        Year = date.Year,
                                        InpatientCompletedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 p.HasAdmittedPatient()),
                                        OutpatientCompletedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 !p.HasAdmittedPatient()),
                                        InpatientBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 p.HasAdmittedPatient()),
                                        OutpatientBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                                 p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 !p.HasAdmittedPatient()),
                                        OpenedBreachedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                                 (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                                 p.StartDate <= lastDateOfMonth),
                                        OpenedPeriodsNumber = periodGroupedBySpecialty.Periods.Count(
                                            p => p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                                 (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                                 p.StartDate <= lastDateOfMonth)
                                    };
                                    periodBreachInfo.BreachedPeriodsNumber =
                                        periodBreachInfo.InpatientBreachedPeriodsNumber +
                                        periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                        periodBreachInfo.OpenedBreachedPeriodsNumber;
                                    periodBreachInfo.PeriodsNumber = periodBreachInfo.InpatientBreachedPeriodsNumber +
                                                                     periodBreachInfo.OutpatientCompletedPeriodsNumber +
                                                                     periodBreachInfo.InpatientCompletedPeriodsNumber +
                                                                     periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                                                     periodBreachInfo.OpenedBreachedPeriodsNumber +
                                                                     periodBreachInfo.OpenedPeriodsNumber;

                                    periodBreaches.Add(periodBreachInfo);
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        var period = periods.FirstOrDefault();

                        if (period != null && period.MaximumNumberOfDays > 0)
                        {
                            var lastEvent = period.Events.LastOrDefault();

                            if (lastEvent != null)
                            {
                                var periodBreachInfo = new PeriodBreachesInfo
                                {
                                    Clinician = string.Empty,
                                    Hospital = lastEvent.Clinician.Hospital.Name,
                                    Specialty = string.Empty,
                                    Month = date.Month,
                                    Year = date.Year,
                                    InpatientCompletedPeriodsNumber = periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             p.HasAdmittedPatient()),
                                    OutpatientCompletedPeriodsNumber = periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             !p.HasAdmittedPatient()),
                                    InpatientBreachedPeriodsNumber = periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             p.HasAdmittedPatient()),
                                    OutpatientBreachedPeriodsNumber = periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             !p.HasAdmittedPatient()),
                                    OpenedBreachedPeriodsNumber = periods.Count(
                                        p => p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                             p.StartDate <= lastDateOfMonth),
                                    OpenedPeriodsNumber = periods.Count(
                                        p => p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                             p.StartDate <= lastDateOfMonth)
                                };
                                periodBreachInfo.BreachedPeriodsNumber =
                                    periodBreachInfo.InpatientBreachedPeriodsNumber +
                                    periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                    periodBreachInfo.OpenedBreachedPeriodsNumber;
                                periodBreachInfo.PeriodsNumber = periodBreachInfo.InpatientBreachedPeriodsNumber +
                                                                 periodBreachInfo.OutpatientCompletedPeriodsNumber +
                                                                 periodBreachInfo.InpatientCompletedPeriodsNumber +
                                                                 periodBreachInfo.OutpatientBreachedPeriodsNumber +
                                                                 periodBreachInfo.OpenedBreachedPeriodsNumber +
                                                                 periodBreachInfo.OpenedPeriodsNumber;
                                periodBreaches.Add(periodBreachInfo);
                            }
                        }
                        break;
                    }
            }

            return periodBreaches;
        }
    }
}
