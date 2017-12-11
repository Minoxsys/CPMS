using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class PlannedEventRepository : IPlannedEventRepository
    {
        public IEnumerable<PlannedEvent> Get(Expression<Func<PlannedEvent, bool>> criteria, ListInput infoModel)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                if (infoModel == null || ((infoModel.OrderBy == null || infoModel.OrderDirection == null) && (infoModel.Index == null || infoModel.PageCount == null)))
                {
                    return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
                }

                if (infoModel.OrderBy != null && infoModel.OrderDirection != null)
                {
                    if (infoModel.Index == null || infoModel.PageCount == null)
                    {
                        return GetOrderedPlannedEvents(unitOfWork, infoModel.OrderBy, infoModel.OrderDirection, criteria);
                    }
                    return GetOrderedPlannedEventsWithPagination(unitOfWork, infoModel, criteria);
                }

                return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                        .Take(infoModel.PageCount.Value)
                    .ToArray();
            }
        }

        private IEnumerable<PlannedEvent> GetOrderedPlannedEventsWithPagination(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<PlannedEvent, bool>> criteria)
        {
            switch (infoModel.OrderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Event.Period.Pathway.Patient.Name);
                case OrderBy.EventCode:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Code.ToString());
                case OrderBy.Specialty:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Event.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedByDirectionAndPagination(unitOfWork, infoModel, criteria, e => e.Event.Clinician.Name);
            }
            return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
        }

        private IEnumerable<PlannedEvent> GetOrderedPlannedEvents(UnitOfWork unitOfWork, OrderBy? orderBy, OrderDirection? orderDirection, Expression<Func<PlannedEvent, bool>> criteria)
        {
            switch (orderBy)
            {
                case OrderBy.PatientName:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Event.Period.Pathway.Patient.Name);
                case OrderBy.EventCode:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Code.ToString());
                case OrderBy.Specialty:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Event.Clinician.Specialty.Name);
                case OrderBy.Clinician:
                    return GetOrderedByDirection(unitOfWork, orderDirection, criteria, e => e.Event.Clinician.Name);
            }
            return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .ToArray();
        }

        private IEnumerable<PlannedEvent> GetOrderedByDirection<TKey>(UnitOfWork unitOfWork, OrderDirection? orderDirection, Expression<Func<PlannedEvent, bool>> criteria, Func<PlannedEvent, TKey> orderCriteria)
        {
            if (orderDirection == OrderDirection.Ascending)
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork)
                    .OrderBy(orderCriteria)
                        .ToArray();
            }
            return GetPlannedEventsByCriteria(criteria, unitOfWork)
                    .OrderByDescending(orderCriteria)
                        .ToArray();
        }

        private IEnumerable<PlannedEvent> GetOrderedByDirectionAndPagination<TKey>(UnitOfWork unitOfWork, ListInput infoModel, Expression<Func<PlannedEvent, bool>> criteria, Func<PlannedEvent, TKey> orderCriteria)
        {
            if (infoModel.OrderDirection == OrderDirection.Ascending)
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .OrderBy(orderCriteria)
                        .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                        .Take(infoModel.PageCount.Value)
                        .ToArray();
            }
            return GetPlannedEventsByCriteria(criteria, unitOfWork)
                        .OrderByDescending(orderCriteria)
                        .Skip(infoModel.Index.Value * infoModel.PageCount.Value)
                        .Take(infoModel.PageCount.Value)
                        .ToArray();
        }

        public int Count(Expression<Func<PlannedEvent, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return GetPlannedEventsByCriteria(criteria, unitOfWork).Count();
            }
        }

        private IEnumerable<PlannedEvent> GetPlannedEventsByCriteria(Expression<Func<PlannedEvent, bool>> criteria, UnitOfWork unitOfWork)
        {
            return unitOfWork.PlannedEvents
                .Include(e => e.Event.Clinician.Specialty)
                .Include(e => e.Event.Clinician.Hospital)
                .Include(e => e.Event.Period.Pathway.Patient)
                .Include(e => e.Event.Period.Events)
                .Where(criteria.Compile());
        }
    }
}
