using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Configuration;
using System.Data.Entity;

namespace CPMS.Admin.Core.Adapters
{
    public class SourceRepository:ISourceRepository
    {
        public IEnumerable<SourceEvent> Get(Expression<Func<SourceEvent, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.SourceEvents.Include(s=>s.NextPossibleEvents).Where(criteria.Compile()).ToArray();
            }
        }
    }
}
