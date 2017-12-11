using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public interface ICompletedEventRepository
    {
        IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null, ListInput infoModel = null, params Expression<Func<CompletedEvent, object>>[] includeProperties);

        int Count(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null);
    }
}
