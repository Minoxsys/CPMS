using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public class EventBreachesReportService
    {
        private readonly IClock _clock;
        private const int ThreeWeeks = 21;
        private const int ThreeDays = 3;

        public EventBreachesReportService(IClock clock)
        {
            _clock = clock;
        }

        public Expression<Func<EventMilestone, bool>> GetCriteria(BreachFilterInputInfo breachFilterInput)
        {
            return eventMilestone =>
                eventMilestone.CompletedEvent.IsActive &&
                eventMilestone.IsMandatory &&
                eventMilestone.CompletedEvent.Period.ShouldCountForBreaches &&
                (String.IsNullOrEmpty(breachFilterInput.NhsNumber) || eventMilestone.CompletedEvent.Period.Pathway.Patient.NHSNumber.ToLower() == breachFilterInput.NhsNumber.ToLower()) &&
                (String.IsNullOrEmpty(breachFilterInput.PatientName) || eventMilestone.CompletedEvent.Period.Pathway.Patient.Name.ToLower().Contains(breachFilterInput.PatientName.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Specialty) || eventMilestone.CompletedEvent.Clinician.Specialty.Name.ToLower().Contains(breachFilterInput.Specialty.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Hospital) || eventMilestone.CompletedEvent.Clinician.Hospital.Name.ToLower().Contains(breachFilterInput.Hospital.ToLower())) &&
                (String.IsNullOrEmpty(breachFilterInput.Clinician) || eventMilestone.CompletedEvent.Clinician.Name.ToLower().Contains(breachFilterInput.Clinician.ToLower())) &&
                (string.IsNullOrEmpty(breachFilterInput.EventDescription) || eventMilestone.Name.Description.ToLower().Contains(breachFilterInput.EventDescription));
        }

        public Func<EventMilestone, bool> GetFilter(int? daysToBreach, int? advancementOrDelay)
        {
            if (daysToBreach < 0)
            {
                // Breached
                return eventMilestone =>
                {
                    var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                    return daysToBreachAt != null &&
                        eventMilestone.BreachDate != null &&
                        (int)daysToBreachAt < 0 &&
                        (advancementOrDelay == null || eventMilestone.CompletedEvent.Period.GetDelayOrAdvancementDays(_clock.Today) == advancementOrDelay);
                };
            }

            if (daysToBreach == ThreeWeeks + 1)
            {
                // About to breach
                return eventMilestone =>
                {
                    var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                    return daysToBreachAt != null &&
                        eventMilestone.BreachDate != null &&
                        (int)daysToBreachAt > ThreeDays &&
                        (int)daysToBreachAt <= ThreeWeeks + 1 &&
                        (advancementOrDelay == null || eventMilestone.CompletedEvent.Period.GetDelayOrAdvancementDays(_clock.Today) == advancementOrDelay);
                };
            }

            return eventMilestone =>
            {
                var daysToBreachAt = eventMilestone.GetDaysToBreachAt(_clock.Today);
                return daysToBreachAt != null &&
                    eventMilestone.BreachDate != null &&
                    (int)daysToBreachAt == daysToBreach &&
                    (advancementOrDelay == null || eventMilestone.CompletedEvent.Period.GetDelayOrAdvancementDays(_clock.Today) == advancementOrDelay);
            };
        }

        public IEnumerable<EventMilestone> ApplyPaginationAndOrdering(IEnumerable<EventMilestone> eventMilestones, ListInputInfo listInputInfo)
        {
            if (listInputInfo == null || ((listInputInfo.OrderBy == null || listInputInfo.OrderDirection == null) && (listInputInfo.Index == null || listInputInfo.PageCount == null)))
            {
                return eventMilestones;
            }

            if (listInputInfo.OrderBy != null && listInputInfo.OrderDirection != null)
            {
                if (listInputInfo.Index == null || listInputInfo.PageCount == null)
                {
                    return GetOrderedEventMilestones(eventMilestones, listInputInfo.OrderBy, listInputInfo.OrderDirection);
                }
                return GetOrderedEventMilestonesWithPagination(eventMilestones, listInputInfo);
            }

            return eventMilestones
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<EventMilestone> GetOrderedEventMilestones(IEnumerable<EventMilestone> eventMilestones, OrderBy? orderBy, OrderDirection? orderDirection)
        {
            switch (orderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.CompletedEvent.Period.Pathway.Patient.Name);
                case OrderBy.EventDescription:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.Name.Description.ToString());
                case OrderBy.Specialty:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.CompletedEvent.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.CompletedEvent.Clinician.Name);
                case OrderBy.DaysInPeriod:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.CompletedEvent.Period.GetDaysSpentAt(_clock.Today));
                case OrderBy.DaysRemainingInPeriod:
                    return GetOrderedEventMilestonesByDirection(eventMilestones, orderDirection, e => e.CompletedEvent.Period.GetDaysRemainingAt(_clock.Today));
            }

            return eventMilestones.ToArray();
        }

        private IEnumerable<EventMilestone> GetOrderedEventMilestonesWithPagination(IEnumerable<EventMilestone> eventMilestones, ListInputInfo listInputInfo)
        {
            switch (listInputInfo.OrderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.CompletedEvent.Period.Pathway.Patient.Name);
                case OrderBy.EventDescription:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.Name.Description.ToString());
                case OrderBy.Specialty:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.CompletedEvent.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.CompletedEvent.Clinician.Name);
                case OrderBy.DaysInPeriod:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.CompletedEvent.Period.GetDaysSpentAt(_clock.Today));
                case OrderBy.DaysRemainingInPeriod:
                    return GetOrderedEventMilestonesByDirectionAndPagination(eventMilestones, listInputInfo, e => e.CompletedEvent.Period.GetDaysRemainingAt(_clock.Today));
            }
            return eventMilestones.ToArray();
        }

        private IEnumerable<EventMilestone> GetOrderedEventMilestonesByDirection<TKey>(IEnumerable<EventMilestone> eventMilestones, OrderDirection? orderDirection, Func<EventMilestone, TKey> orderCriteria)
        {
            return orderDirection == OrderDirection.Ascending
                ? eventMilestones.OrderBy(orderCriteria).ToArray()
                : eventMilestones.OrderByDescending(orderCriteria).ToArray();
        }

        private IEnumerable<EventMilestone> GetOrderedEventMilestonesByDirectionAndPagination<TKey>(IEnumerable<EventMilestone> eventMilestones, ListInputInfo listInputInfo, Func<EventMilestone, TKey> orderCriteria)
        {
            return listInputInfo.OrderDirection == OrderDirection.Ascending
                ? OrderEventMilestonesAscendingWithPagination(eventMilestones, listInputInfo, orderCriteria)
                : OrderEventMilestonesDescendingWithPagination(eventMilestones, listInputInfo, orderCriteria);
        }

        private IEnumerable<EventMilestone> OrderEventMilestonesDescendingWithPagination<TKey>(IEnumerable<EventMilestone> eventMilestones, ListInputInfo listInputInfo, Func<EventMilestone, TKey> orderCriteria)
        {
            return eventMilestones
                .OrderByDescending(orderCriteria)
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<EventMilestone> OrderEventMilestonesAscendingWithPagination<TKey>(IEnumerable<EventMilestone> eventMilestones, ListInputInfo listInputInfo, Func<EventMilestone, TKey> orderCriteria)
        {
            return eventMilestones
                .OrderBy(orderCriteria)
                .Skip((listInputInfo.Index.Value - 1) * listInputInfo.PageCount.Value)
                .Take(listInputInfo.PageCount.Value)
                .ToArray();
        }
    }
}
