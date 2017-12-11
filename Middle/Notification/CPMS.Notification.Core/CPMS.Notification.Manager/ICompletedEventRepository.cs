using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Notification.Manager
{
    public interface ICompletedEventRepository
    {
        IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, params Expression<Func<CompletedEvent, object>>[] includeProperties);
    }
}
