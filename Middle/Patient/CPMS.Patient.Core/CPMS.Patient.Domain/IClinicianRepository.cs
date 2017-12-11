using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IClinicianRepository
    {
        IEnumerable<Clinician> Get(Expression<Func<Clinician, bool>> criteria);
    }
}
