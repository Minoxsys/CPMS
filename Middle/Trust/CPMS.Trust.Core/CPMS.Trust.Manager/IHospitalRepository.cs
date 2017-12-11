using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Trust.Manager
{
    public interface IHospitalRepository
    {
        IEnumerable<Hospital> Get(Expression<Func<Hospital, bool>> criteria, params Expression<Func<Hospital, object>>[] includeProperties);
    }
}
