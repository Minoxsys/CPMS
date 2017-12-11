using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IHospitalRepository
    {
        IEnumerable<Hospital> Get(Expression<Func<Hospital, bool>> criteria);
    }
}
