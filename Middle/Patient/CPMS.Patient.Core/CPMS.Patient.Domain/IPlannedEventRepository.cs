using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IPlannedEventRepository
    {
        IEnumerable<PlannedEvent> Get(Expression<Func<PlannedEvent, bool>> criteria, ListInput listInput);

        int Count(Expression<Func<PlannedEvent, bool>> criteria);
    }
}
