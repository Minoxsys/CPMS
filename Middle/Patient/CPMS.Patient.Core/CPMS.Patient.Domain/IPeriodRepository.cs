using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IPeriodRepository
    {
        IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria);

        IEnumerable<Period> Get(Expression<Func<Period, bool>> criteria, DateTime fromDate);

        int Count(Expression<Func<Period, bool>> criteria);
    }
}
