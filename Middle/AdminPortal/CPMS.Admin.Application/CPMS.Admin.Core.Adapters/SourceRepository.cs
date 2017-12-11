using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using CPMS.Configuration;
using ISourceRepository = CPMS.Admin.Application.ISourceRepository;

namespace CPMS.Admin.Core.Adapters
{
    public class SourceRepository:ISourceRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public SourceRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<SourceEvent> Get(Expression<Func<SourceEvent, bool>> criteria, params Expression<Func<SourceEvent, object>>[] includeProperties)
        {
            var sourceEventsSet = _unitOfWork.SourceEvents as IQueryable<SourceEvent>;

            sourceEventsSet = includeProperties.Aggregate(sourceEventsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return sourceEventsSet.Where(criteria).ToArray();
        }
    }
}
