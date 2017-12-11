using System.Collections.Generic;
using System.Linq;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class FuturePeriodBreachesReportService
    {
        public IEnumerable<FuturePeriodBreachesInfo> GetFuturePeriodBreaches(IList<CompletedEvent> completedEvents, int weeksToBreach, Granularity? granularity)
        {
            if (!completedEvents.Any()) return null;

            var futurePeriodBreaches = new List<FuturePeriodBreachesInfo>();

            switch (granularity)
            {
                case Granularity.Specialty:
                    {
                        var completedEventsGroupedBySpecialty = from completedEvent in completedEvents
                                                            group completedEvent by completedEvent.Clinician.Specialty
                                                            into groupedEvents
                                                            select new { SpecialtyId = groupedEvents.Key, Events = groupedEvents.ToList() };

                        futurePeriodBreaches.AddRange(
                            from completedEventGroupedBySpecialty in completedEventsGroupedBySpecialty
                            let firstCompletedEvent = completedEventGroupedBySpecialty.Events.FirstOrDefault()
                            select new FuturePeriodBreachesInfo
                            {
                                Clinician = string.Empty,
                                Hospital = firstCompletedEvent.Clinician.Hospital.Name,
                                Specialty = firstCompletedEvent.Clinician.Specialty.Name,
                                WeeksToBreach = weeksToBreach,
                                NumberOfBreaches = completedEventGroupedBySpecialty.Events.Count()
                            });
                        break;
                    }
                case Granularity.Clinician:
                    {
                        var completedEventsGroupedByClinician = from completedEvent in completedEvents
                                                            group completedEvent by completedEvent.Clinician.Name
                                                            into groupedEvents
                                                            select new { ClinicianId = groupedEvents.Key, Events = groupedEvents.ToList() };

                        futurePeriodBreaches.AddRange(
                            from completedEventGroupedByClinician in completedEventsGroupedByClinician
                            let firstCompletedEvent = completedEventGroupedByClinician.Events.FirstOrDefault()
                            select new FuturePeriodBreachesInfo
                            {
                                Clinician = firstCompletedEvent.Clinician.Name,
                                Hospital = firstCompletedEvent.Clinician.Hospital.Name,
                                Specialty = firstCompletedEvent.Clinician.Specialty.Name,
                                WeeksToBreach = weeksToBreach,
                                NumberOfBreaches = completedEventGroupedByClinician.Events.Count()
                            });
                        break;
                    }
                default:
                    {
                        var completedEvent = completedEvents.FirstOrDefault();
                        if (completedEvent != null)
                        {
                            futurePeriodBreaches.Add(new FuturePeriodBreachesInfo
                            {
                                Clinician = string.Empty,
                                Hospital = completedEvent.Clinician.Hospital.Name,
                                Specialty = string.Empty,
                                WeeksToBreach = weeksToBreach,
                                NumberOfBreaches = completedEvents.Count()
                            });
                        }
                        break;
                    }
            }

            return futurePeriodBreaches;
        }
    }
}
