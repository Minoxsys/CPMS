using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public interface ISourceRepository
    {
        IEnumerable<SourceEvent> Get(Expression<Func<SourceEvent, bool>> criteria, params Expression<Func<SourceEvent, object>>[] includeProperties);
    }
}
