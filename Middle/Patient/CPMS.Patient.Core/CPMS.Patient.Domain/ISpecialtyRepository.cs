using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface ISpecialtyRepository
    {
        IEnumerable<Specialty> Get(Expression<Func<Specialty, bool>> criteria);
    }
}
