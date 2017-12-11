using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Patient.Domain;

namespace CPMS.Core.Adapters
{
    public class PathwayRepository : IPathwayRepository
    {
        public IEnumerable<Pathway> Get(Expression<Func<Pathway, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Pathways.Where(criteria).ToArray();
            }
        }
    }
}
