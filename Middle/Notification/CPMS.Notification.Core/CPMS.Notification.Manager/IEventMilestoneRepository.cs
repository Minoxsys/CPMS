using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Notification.Manager
{
    public interface IEventMilestoneRepository
    {
        IEnumerable<EventMilestone> Get(Expression<Func<EventMilestone, bool>> criteria, params Expression<Func<EventMilestone, object>>[] includeProperties);
    }
}
