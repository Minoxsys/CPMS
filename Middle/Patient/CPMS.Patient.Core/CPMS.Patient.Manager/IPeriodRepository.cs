using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public interface IPeriodRepository
    {
        IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, params Expression<Func<Period, object>>[] includeProperties);

        int Count(Expression<Func<Period, bool>> criteria);
    }
}
