using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Trust.Manager
{
    public interface ISpecialtyRepository
    {
        IEnumerable<Specialty> Get(Expression<Func<Specialty, bool>> criteria, params Expression<Func<Specialty, object>>[] includeProperties);
    }
}
