using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Domain;
using CPMS.Patient.Manager;

namespace CPMS.Core.Adapters
{
    public class PathwayRepository : IPathwayRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public PathwayRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Pathway> Get(Expression<Func<Pathway, bool>> criteria)
        {
            return _unitOfWork.Pathways.Where(criteria).ToArray();
        }
    }
}
