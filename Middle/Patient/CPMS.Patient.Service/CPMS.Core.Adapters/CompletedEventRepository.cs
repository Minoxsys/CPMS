using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using CPMS.Patient.Manager;

namespace CPMS.Core.Adapters
{
    public class CompletedEventRepository : ICompletedEventRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public CompletedEventRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null, ListInput infoModel = null, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            if (infoModel == null ||
                ((infoModel.OrderBy == null || infoModel.OrderDirection == null) &&
                 (infoModel.Index == null || infoModel.PageCount == null)))
            {
                return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                    .ToArray();
            }

            if (infoModel.OrderBy != null && infoModel.OrderDirection != null)
            {
                if (infoModel.Index == null || infoModel.PageCount == null)
                {
                    return GetOrderedCompletedEvents(infoModel.OrderBy, infoModel.OrderDirection, criteria, periodType, includeProperties);
                }
                return GetOrderedCompletedEventsWithPagination(infoModel, criteria, periodType, includeProperties);
            }

            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                .OrderBy(completedEvent => completedEvent.EventDate)
                .Skip((infoModel.Index.Value - 1) * infoModel.PageCount.Value)
                .Take(infoModel.PageCount.Value)
                .ToArray();
        }

        public int Count(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null)
        {
            return _unitOfWork.CompletedEvents
                .Where(e => (String.IsNullOrEmpty(periodType) ||
                        (periodType == typeof (RTT18WeekPeriod).Name.ToLower() && e.Period is RTT18WeekPeriod) ||
                        (periodType == typeof (Non18WeekPeriod).Name.ToLower() && e.Period is Non18WeekPeriod) ||
                        (periodType == typeof (CancerPeriod).Name.ToLower() && e.Period is CancerPeriod)))
                .Count(criteria);
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEventsWithPagination(ListInput infoModel, Expression<Func<CompletedEvent, bool>> criteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            switch (infoModel.OrderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Period.Pathway.Patient.Name, periodType, includeProperties);
                case OrderBy.EventDescription:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Name.Code.ToString(), periodType, includeProperties);
                case OrderBy.Specialty:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Clinician.Specialty.Name, periodType, includeProperties);
                case OrderBy.Clinician:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Clinician.Name, periodType, includeProperties);
                case OrderBy.Hospital:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Clinician.Hospital.Name, periodType, includeProperties);
                case OrderBy.DateOfBirth:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Period.Pathway.Patient.DateOfBirth, periodType, includeProperties);
                case OrderBy.Age:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.Period.Pathway.Patient.DateOfBirth, periodType, includeProperties);
                case OrderBy.ImportDate:
                    return GetOrderedByDirectionAndPagination(infoModel, criteria, e => e.EventDate, periodType);
            }
            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties).ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedCompletedEvents(OrderBy? orderBy, OrderDirection? orderDirection, Expression<Func<CompletedEvent, bool>> criteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            switch (orderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Period.Pathway.Patient.Name, periodType, includeProperties);
                case OrderBy.EventDescription:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Name.Code.ToString(), periodType, includeProperties);
                case OrderBy.Specialty:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Clinician.Specialty.Name, periodType, includeProperties);
                case OrderBy.Clinician:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Clinician.Name, periodType, includeProperties);
                case OrderBy.Hospital:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Clinician.Hospital.Name, periodType, includeProperties);
                case OrderBy.DateOfBirth:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.Period.Pathway.Patient.DateOfBirth, periodType, includeProperties);
                case OrderBy.Age:
                    {
                        orderDirection = orderDirection == OrderDirection.Ascending ? OrderDirection.Descending : OrderDirection.Ascending;
                        return GetOrderedByDirection(orderDirection, criteria, e => e.Period.Pathway.Patient.DateOfBirth, periodType, includeProperties);
                    }
                case OrderBy.ImportDate:
                    return GetOrderedByDirection(orderDirection, criteria, e => e.EventDate, periodType, includeProperties);
            }
            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties).ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedByDirection<TKey>(OrderDirection? orderDirection, Expression<Func<CompletedEvent, bool>> criteria, Expression<Func<CompletedEvent, TKey>> orderCriteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            if (orderDirection == OrderDirection.Ascending)
            {
                return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                    .OrderBy(orderCriteria)
                    .ToArray();
            }
            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                .OrderByDescending(orderCriteria)
                .ToArray();
        }

        private IEnumerable<CompletedEvent> GetOrderedByDirectionAndPagination<TKey>(ListInput infoModel, Expression<Func<CompletedEvent, bool>> criteria, Expression<Func<CompletedEvent, TKey>> orderCriteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            if (infoModel.OrderDirection == OrderDirection.Ascending)
            {
                if (infoModel.OrderBy == OrderBy.Age)
                {
                    return GetCriteriaDescendingWithPagination(infoModel, criteria, orderCriteria, periodType, includeProperties);
                }
                return GetCriteriaAscendingWithPagination(infoModel, criteria, orderCriteria, periodType, includeProperties);
            }
            if (infoModel.OrderBy == OrderBy.Age)
            {
                return GetCriteriaAscendingWithPagination(infoModel, criteria, orderCriteria, periodType, includeProperties);
            }
            return GetCriteriaDescendingWithPagination(infoModel, criteria, orderCriteria, periodType, includeProperties);
        }

        private IEnumerable<CompletedEvent> GetCriteriaAscendingWithPagination<TKey>(ListInput infoModel, Expression<Func<CompletedEvent, bool>> criteria, Expression<Func<CompletedEvent, TKey>> orderCriteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                .OrderBy(orderCriteria)
                .Skip((infoModel.Index.Value - 1) * infoModel.PageCount.Value)
                .Take(infoModel.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<CompletedEvent> GetCriteriaDescendingWithPagination<TKey>(ListInput infoModel, Expression<Func<CompletedEvent, bool>> criteria, Expression<Func<CompletedEvent, TKey>> orderCriteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            return GetCompletedEventsByCriteria(criteria, periodType, includeProperties)
                .OrderByDescending(orderCriteria)
                .Skip((infoModel.Index.Value - 1) * infoModel.PageCount.Value)
                .Take(infoModel.PageCount.Value)
                .ToArray();
        }

        private IQueryable<CompletedEvent> GetCompletedEventsByCriteria(Expression<Func<CompletedEvent, bool>> criteria, string periodType, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            var completedEventsSet = _unitOfWork.CompletedEvents as IQueryable<CompletedEvent>;

            completedEventsSet = includeProperties.Aggregate(completedEventsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return completedEventsSet
                .Where(criteria)
                .Where(e => (String.IsNullOrEmpty(periodType) ||
                    (periodType == typeof(RTT18WeekPeriod).Name.ToLower() && e.Period is RTT18WeekPeriod) ||
                    (periodType == typeof(Non18WeekPeriod).Name.ToLower() && e.Period is Non18WeekPeriod) ||
                    (periodType == typeof(CancerPeriod).Name.ToLower() && e.Period is CancerPeriod)));
        }

    }
}
