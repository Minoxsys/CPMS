using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using IEventMilestoneRepository = CPMS.Notification.Manager.IEventMilestoneRepository;

namespace CPMS.Notification.Core.Adapters
{
    public class EventMilestoneRepository : IEventMilestoneRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public EventMilestoneRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<EventMilestone> Get(Expression<Func<EventMilestone, bool>> criteria, params Expression<Func<EventMilestone, object>>[] includeProperties)
        {
            var eventMilestonesSet = _unitOfWork.EventMilestones as IQueryable<EventMilestone>;

            eventMilestonesSet = includeProperties.Aggregate(eventMilestonesSet,
                (current, includeProperty) => current.Include(includeProperty));

            return eventMilestonesSet.Where(criteria).ToArray();
        }
    }
}
