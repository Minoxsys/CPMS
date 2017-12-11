using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using ICompletedEventRepository = CPMS.Report.Manager.ICompletedEventRepository;

namespace CPMS.Report.Core.Adapters
{
    public class CompletedEventRepository: ICompletedEventRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public CompletedEventRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null, DateTime? fromDate = null, params Expression<Func<CompletedEvent, object>>[] includeProperties)
        {
            var completedEventsSet = _unitOfWork.CompletedEvents as IQueryable<CompletedEvent>;

            completedEventsSet = includeProperties.Aggregate(completedEventsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return completedEventsSet
                .Where(criteria)
                .Where(completedEvent => (String.IsNullOrEmpty(periodType) ||
                    (periodType == typeof(RTT18WeekPeriod).Name.ToLower() && completedEvent.Period is RTT18WeekPeriod) ||
                    (periodType == typeof(Non18WeekPeriod).Name.ToLower() && completedEvent.Period is Non18WeekPeriod) ||
                    (periodType == typeof(CancerPeriod).Name.ToLower() && completedEvent.Period is CancerPeriod)))
                .Where(completedEvent => fromDate == null || DbFunctions.AddDays(completedEvent.Period.StartDate, completedEvent.Period.MaximumNumberOfDays) >= fromDate)
                .ToArray();
        }
    }
}
