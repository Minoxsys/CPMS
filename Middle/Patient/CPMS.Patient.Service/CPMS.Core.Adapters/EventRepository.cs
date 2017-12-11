using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;
using System.Data.Entity;

namespace CPMS.Core.Adapters
{
    public class EventRepository: IEventRepository
    {
        public IEnumerable<Event> Get(Expression<Func<Event, bool>> criteria, ListInput infoModel)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                if (infoModel == null || ((infoModel.OrderBy == null || infoModel.OrderDirection == null) && (infoModel.Index == null || infoModel.PageCount == null)))
                {
                    return GetEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
                }

                if (infoModel.OrderBy != null && infoModel.OrderDirection != null)
                {
                    if (infoModel.Index == null || infoModel.PageCount == null)
                    {
                        return GetOrderedEvents(unitOfWork, infoModel.OrderBy, infoModel.OrderDirection, criteria);
                    }
                    return GetOrderedEventsWithPagination(unitOfWork, infoModel, criteria);
                }

                return GetEventsByCriteria(criteria, unitOfWork)
                        .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                        .Take(infoModel.PageCount.Value)
                    .ToArray();
            }
        }

        public int Count(Expression<Func<Event, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetEventsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private IEnumerable<Event> GetOrderedEventsWithPagination(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<Event, bool>> criteria)
        {
            switch (infoModel.OrderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Period.Pathway.Patient.Name);
                case OrderBy.EventCode:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Code.ToString());
                case OrderBy.Specialty:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Clinician.Name);
                case OrderBy.Hospital:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Clinician.Hospital.Name);
                case OrderBy.DateOfBirth:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Period.Pathway.Patient.DateOfBirth);
                case OrderBy.Age:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Period.Pathway.Patient.DateOfBirth);
                case OrderBy.ImportDate:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.EventDate);
            }
            return GetEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
        }

        private IEnumerable<Event> GetOrderedEvents(UnitOfWork unitOfWork, OrderBy? orderBy, OrderDirection? orderDirection, Expression<Func<Event, bool>> criteria)
        {
            switch (orderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Period.Pathway.Patient.Name);
                case OrderBy.EventCode:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Code.ToString());
                case OrderBy.Specialty:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Clinician.Name);
                case OrderBy.Hospital:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Clinician.Hospital.Name);
                case OrderBy.DateOfBirth:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Period.Pathway.Patient.DateOfBirth);
                case OrderBy.Age:
                {
                    orderDirection = orderDirection == OrderDirection.Ascending ? OrderDirection.Descending : OrderDirection.Ascending;
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria,
                        e => e.Period.Pathway.Patient.DateOfBirth);
                }
                case OrderBy.ImportDate:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.EventDate);
            }
            return GetEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
        }

        private IEnumerable<Event> GetOrderedByDirection<TKey>(UnitOfWork unitOfWork, OrderDirection? orderDirection, Expression<Func<Event, bool>> criteria, Func<Event, TKey> orderCriteria)
        {
            if (orderDirection == OrderDirection.Ascending)
            {
                return GetEventsByCriteria(criteria, unitOfWork)
                    .OrderBy(orderCriteria)
                        .ToArray();
            }
            return GetEventsByCriteria(criteria, unitOfWork)
                    .OrderByDescending(orderCriteria)
                        .ToArray();
        }

        private IEnumerable<Event> GetOrderedByDirectionAndPagination<TKey>(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<Event, bool>> criteria, Func<Event, TKey> orderCriteria)
        {
            if (infoModel.OrderDirection == OrderDirection.Ascending)
            {
                if (infoModel.OrderBy == OrderBy.Age)
                {
                    return GetCriteriaDescendingWithPagination(unitOfWork, infoModel, criteria, orderCriteria);
                }
                return GetCriteriaAscendingWithPagination(unitOfWork, infoModel, criteria, orderCriteria);
            }
            if (infoModel.OrderBy == OrderBy.Age)
            {
                return GetCriteriaAscendingWithPagination(unitOfWork, infoModel, criteria, orderCriteria);
            }
            return GetCriteriaDescendingWithPagination(unitOfWork, infoModel, criteria, orderCriteria);
        }

        private IEnumerable<Event> GetCriteriaAscendingWithPagination<TKey>(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<Event, bool>> criteria, Func<Event, TKey> orderCriteria)
        {
            return GetEventsByCriteria(criteria, unitOfWork)
                .OrderBy(orderCriteria)
                .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                .Take(infoModel.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<Event> GetCriteriaDescendingWithPagination<TKey>(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<Event, bool>> criteria, Func<Event, TKey> orderCriteria)
        {
            return GetEventsByCriteria(criteria, unitOfWork)
                .OrderByDescending(orderCriteria)
                .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                .Take(infoModel.PageCount.Value)
                .ToArray();
        }

        private IEnumerable<Event> GetEventsByCriteria(Expression<Func<Event, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.Events
                .Include(e => e.Period.Pathway.Patient)
                .Include(e => e.Clinician.Specialty)
                .Include(e => e.Clinician.Hospital)
                .Where(criteria.Compile());
        }

    }
}
