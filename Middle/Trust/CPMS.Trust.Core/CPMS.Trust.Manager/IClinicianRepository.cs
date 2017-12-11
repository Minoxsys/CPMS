using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Trust.Manager
{
    public interface IClinicianRepository
    {
        IEnumerable<Clinician> Get(Expression<Func<Clinician, bool>> criteria, params Expression<Func<Clinician, object>>[] includeProperties);
    }
}
