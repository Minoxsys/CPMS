using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Domain;

namespace CPMS.Patient.Manager
{
    public interface IPathwayRepository
    {
        IEnumerable<Pathway> Get(Expression<Func<Pathway, bool>> criteria);
    }
}
