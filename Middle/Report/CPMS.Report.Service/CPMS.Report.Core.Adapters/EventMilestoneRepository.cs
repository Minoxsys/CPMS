using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using IEventMilestoneRepository = CPMS.Report.Manager.IEventMilestoneRepository;

namespace CPMS.Report.Core.Adapters
{
    public class EventMilestoneRepository : IEventMilestoneRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public EventMilestoneRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<EventMilestone> Get(Expression<Func<EventMilestone, bool>> criteria, string periodType, params Expression<Func<EventMilestone, object>>[] includeProperties)
        {
            var eventMilestonesSet = _unitOfWork.EventMilestones as IQueryable<EventMilestone>;

            eventMilestonesSet = includeProperties.Aggregate(eventMilestonesSet,
                (current, includeProperty) => current.Include(includeProperty));

            return eventMilestonesSet
                .Where(criteria)
                .Where(e => (String.IsNullOrEmpty(periodType) ||
                                 (periodType == typeof(RTT18WeekPeriod).Name.ToLower() && e.CompletedEvent.Period is RTT18WeekPeriod) ||
                                 (periodType == typeof(Non18WeekPeriod).Name.ToLower() && e.CompletedEvent.Period is Non18WeekPeriod) ||
                                 (periodType == typeof(CancerPeriod).Name.ToLower() && e.CompletedEvent.Period is CancerPeriod)))
                .ToArray(); 
        }
    }
}
