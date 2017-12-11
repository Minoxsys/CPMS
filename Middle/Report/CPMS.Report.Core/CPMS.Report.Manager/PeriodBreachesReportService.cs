using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class PeriodBreachesReportService
    {
        private readonly IClock _clock;

        private const int OneWeek = 7;
        private const int ThreeWeeks = 21;

        public PeriodBreachesReportService(IClock clock)
        {
            _clock = clock;
        }

        public Expression<Func<CompletedEvent, bool>> GetCriteria(BreachFilterInputInfo breachFilterInput)
        {
            return completedEvent =>
                completedEvent.IsActive &&
                completedEvent.Period.ShouldCountForBreaches &&
                (String.IsNullOrEmpty(breachFilterInput.NhsNumber) || completedEvent.Period.Pathway.Patient.NHSNumber.ToLower() == breachFilterInput.NhsNumber.ToLower()) &&
                (String.IsNullOrEmpty(breachFilterInput.PatientName) || completedEvent.Period.Pathway.Patient.Name.ToLower().Contains(breachFilterInput.PatientName.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Specialty) || completedEvent.Clinician.Specialty.Name.ToLower().Contains(breachFilterInput.Specialty.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Hospital) || completedEvent.Clinician.Hospital.Name.ToLower().Contains(breachFilterInput.Hospital.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Clinician) || completedEvent.Clinician.Name.ToLower().Contains(breachFilterInput.Clinician.ToLower())) &&
                (string.IsNullOrEmpty(breachFilterInput.EventDescription) || completedEvent.Name.Description.ToLower().Contains(breachFilterInput.EventDescription));
        }

        public Func<CompletedEvent, bool> GetFilter(int? weeksToBreach)
        {
            if (weeksToBreach < 0)
            {
                // Breached
                return completedEvent =>
                    completedEvent.Period.GetDaysRemainingAt(_clock.Today) < 0 &&
                    completedEvent.Period.GetDaysRemainingAt(_clock.Today) >= -ThreeWeeks;
            }

            return completedEvent =>
                (completedEvent.Period.GetDaysRemainingAt(_clock.Today) == 0 && weeksToBreach == 1) ||
                (completedEvent.Period.GetDaysRemainingAt(_clock.Today) > (weeksToBreach - 1) * OneWeek &&
                 completedEvent.Period.GetDaysRemainingAt(_clock.Today) <= weeksToBreach * OneWeek);
        }

        public IEnumerable<CompletedEvent> ApplyPaginationAndOrdering(IEnumerable<CompletedEvent> completedEvents, ListInputInfo listInputInfo)
        {
            if (listInputInfo == null || ((listInputInfo.OrderBy == null || listInputInfo.OrderDirection == null) && (listInputInfo.Index == null || listInputInfo.PageCount == null)))
            {
                return completedEvents;
            }

            if (listInputInfo.OrderBy != null && listInputInfo.OrderDirection != null)
            {
                if (listInputInfo.Index == null || listInputInfo.PageCount == null)
                {
                    return GetOrderedCompletedEvents(completedEvents, listInputInfo.OrderBy, listInputInfo.OrderDirection);
                }
                return GetOrderedCompletedEventsWithPagination(completedEvents, listInputInfo);
            }

            return completedEvents
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEvents(IEnumerable<CompletedEvent> completedEvents, OrderBy? orderBy, OrderDirection? orderDirection)
        {
            switch (orderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Period.Pathway.Patient.Name);
                case OrderBy.EventDescription:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Name.Description);
                case OrderBy.Specialty:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Clinician.Name);
                case OrderBy.DaysInPeriod:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Period.GetDaysSpentAt(_clock.Today));
                case OrderBy.DaysRemainingInPeriod:
                    return GetOrderedCompletedEventsByDirection(completedEvents, orderDirection, e => e.Period.GetDaysRemainingAt(_clock.Today));
            }
            return completedEvents.ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEventsWithPagination(IEnumerable<CompletedEvent> completedEvents, ListInputInfo listInputInfo)
        {
            switch (listInputInfo.OrderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Period.Pathway.Patient.Name);
                case OrderBy.EventDescription:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Name.Description);
                case OrderBy.Specialty:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Clinician.Name);
                case OrderBy.DaysInPeriod:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Period.GetDaysSpentAt(_clock.Today));
                case OrderBy.DaysRemainingInPeriod:
                    return GetOrderedCompletedEventsByDirectionAndPagination(completedEvents, listInputInfo, e => e.Period.GetDaysRemainingAt(_clock.Today));
            }
            return completedEvents.ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEventsByDirection<TKey>(IEnumerable<CompletedEvent> completedEvents, OrderDirection? orderDirection, Func<CompletedEvent, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.Ascending
                ? completedEvents.OrderBy(orderCriteria).ToArray()
                : completedEvents.OrderByDescending(orderCriteria).ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEventsByDirectionAndPagination<TKey>(IEnumerable<CompletedEvent> completedEvents, ListInputInfo listInputInfo, Func<CompletedEvent, TKey> orderCriteria)
        {
            return listInputInfo.OrderDirection == OrderDirection.Ascending
                ? OrderCompletedEventsAscendingWithPagination(completedEvents, listInputInfo, orderCriteria)
                : OrderCompletedEventsDescendingWithPagination(completedEvents, listInputInfo, orderCriteria);
        }

        private IEnumerable<CompletedEvent> OrderCompletedEventsDescendingWithPagination<TKey>(IEnumerable<CompletedEvent> completedEvents, ListInputInfo listInputInfo, Func<CompletedEvent, TKey> orderCriteria)
        {
            return completedEvents
                .OrderByDescending(orderCriteria)
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<CompletedEvent> OrderCompletedEventsAscendingWithPagination<TKey>(IEnumerable<CompletedEvent> completedEvents, ListInputInfo listInputInfo, Func<CompletedEvent, TKey> orderCriteria)
        {
            return completedEvents
                .OrderBy(orderCriteria)
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }
    }
}
