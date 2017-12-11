using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IEventRepository
    {
        IEnumerable<Event> Get(Expression<Func<Event, bool>> criteria, ListInput listInput);

        int Count(Expression<Func<Event, bool>> criteria);
    }
}
