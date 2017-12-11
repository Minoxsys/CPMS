using System;
using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class MonthlyPeriodBreachesReportService
    {
        public IEnumerable<Monthly18wRTTPerformanceInfo> GetMonthlyPeriodBreaches(IList<Period> periods, DateTime date, Granularity? granularity)
        {
            if (!periods.Any()) return null;

            var firstDateOfMonth = date.FirstDayOfMonth();
            var lastDateOfMonth = date.FirstDayOfMonth();

            var periodBreaches = new List<Monthly18wRTTPerformanceInfo>();

            switch (granularity)
            {
                case Granularity.Specialty:
                {
                    var periodsGroupedBySpecialty = from period in periods
                        let lastEvent = period.CompletedEvents.LastOrDefault()
                        where lastEvent != null
                        group period by lastEvent.Clinician.Specialty
                        into groupedPeriods
                        select new {SpecialtyId = groupedPeriods.Key, Periods = groupedPeriods.ToList()};

                    foreach (var periodGroupedBySpecialty in periodsGroupedBySpecialty)
                    {
                        var period = periodGroupedBySpecialty.Periods.FirstOrDefault();
                        if (period != null && period.MaximumNumberOfDays > 0)
                        {
                            var lastEvent = period.CompletedEvents.LastOrDefault();
                            if (lastEvent != null)
                            {
                                var periodBreachInfo = new Monthly18wRTTPerformanceInfo
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
                                                     let lastEvent = period.CompletedEvents.LastOrDefault()
                        where lastEvent != null
                        group period by lastEvent.Clinician.Name
                        into groupedPeriods
                        select new {ClinicianId = groupedPeriods.Key, Periods = groupedPeriods.ToList()}).ToList();

                    foreach (var periodGroupedByClinician in periodsGroupedByClinician)
                    {
                        var period = periodGroupedByClinician.Periods.FirstOrDefault();
                        if (period != null && period.MaximumNumberOfDays > 0)
                        {
                            var lastEvent = period.CompletedEvents.LastOrDefault();
                            if (lastEvent != null)
                            {
                                var periodBreachInfo = new Monthly18wRTTPerformanceInfo

                                {
                                    Clinician = lastEvent.Clinician.Name,
                                    Hospital = lastEvent.Clinician.Hospital.Name,
                                    Specialty = lastEvent.Clinician.Specialty.Name,
                                    Month = date.Month,
                                    Year = date.Year,
                                    InpatientCompletedPeriodsNumber = periodGroupedByClinician.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             p.HasAdmittedPatient()),
                                    OutpatientCompletedPeriodsNumber = periodGroupedByClinician.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             !p.HasAdmittedPatient()),
                                    InpatientBreachedPeriodsNumber = periodGroupedByClinician.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             p.HasAdmittedPatient()),
                                    OutpatientBreachedPeriodsNumber = periodGroupedByClinician.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             !p.HasAdmittedPatient()),
                                    OpenedBreachedPeriodsNumber = periodGroupedByClinician.Periods.Count(
                                        p => p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                             p.StartDate <= lastDateOfMonth),
                                    OpenedPeriodsNumber = periodGroupedByClinician.Periods.Count(
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
                    var periodsGroupedByHospital = (from period in periods
                                                     let lastEvent = period.CompletedEvents.LastOrDefault()
                                                     where lastEvent != null
                                                     group period by lastEvent.Clinician.Hospital.Name
                                                         into groupedPeriods
                                                         select new { HospitalId = groupedPeriods.Key, Periods = groupedPeriods.ToList() }).ToList();
                    foreach (var periodGroupedByHospital in periodsGroupedByHospital)
                    {
                        var period = periodGroupedByHospital.Periods.FirstOrDefault();

                        if (period != null && period.MaximumNumberOfDays > 0)
                        {
                            var lastEvent = period.CompletedEvents.LastOrDefault();

                            if (lastEvent != null)
                            {
                                var periodBreachInfo = new Monthly18wRTTPerformanceInfo
                                {
                                    Clinician = string.Empty,
                                    Hospital = lastEvent.Clinician.Hospital.Name,
                                    Specialty = string.Empty,
                                    Month = date.Month,
                                    Year = date.Year,
                                    InpatientCompletedPeriodsNumber = periodGroupedByHospital.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             p.HasAdmittedPatient()),
                                    OutpatientCompletedPeriodsNumber = periodGroupedByHospital.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysSpentAt(lastDateOfMonth) <= p.MaximumNumberOfDays &&
                                             !p.HasAdmittedPatient()),
                                    InpatientBreachedPeriodsNumber = periodGroupedByHospital.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             p.HasAdmittedPatient()),
                                    OutpatientBreachedPeriodsNumber = periodGroupedByHospital.Periods.Count(
                                        p => p.StopDate >= firstDateOfMonth && p.StopDate <= lastDateOfMonth &&
                                             p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             !p.HasAdmittedPatient()),
                                    OpenedBreachedPeriodsNumber = periodGroupedByHospital.Periods.Count(
                                        p => p.GetDaysRemainingAt(lastDateOfMonth) < 0 &&
                                             (p.StopDate > lastDateOfMonth || p.StopDate == null) &&
                                             p.StartDate <= lastDateOfMonth),
                                    OpenedPeriodsNumber = periodGroupedByHospital.Periods.Count(
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
            }

            return periodBreaches;
        }
    }
}
