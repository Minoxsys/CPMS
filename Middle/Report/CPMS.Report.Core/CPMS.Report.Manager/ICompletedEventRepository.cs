using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Report.Manager
{
    public interface ICompletedEventRepository
    {
        IEnumerable<CompletedEvent> Get(Expression<Func<CompletedEvent, bool>> criteria, string periodType = null, DateTime? fromDate = null, params Expression<Func<CompletedEvent, object>>[] includeProperties);
    }
}
