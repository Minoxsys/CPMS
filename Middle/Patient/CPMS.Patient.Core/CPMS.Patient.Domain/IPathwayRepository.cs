using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Patient.Domain
{
    public interface IPathwayRepository
    {
        IEnumerable<Pathway> Get(Expression<Func<Pathway, bool>> criteria);
    }
}
