using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Configuration
{
    public interface ISourceRepository
    {
        IEnumerable<SourceEvent> Get(Expression<Func<SourceEvent, bool>> criteria);
    }
}
