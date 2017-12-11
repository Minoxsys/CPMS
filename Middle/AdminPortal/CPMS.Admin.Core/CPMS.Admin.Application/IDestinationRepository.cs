using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Configuration;

namespace CPMS.Admin.Application
{
    public interface IDestinationRepository
    {
        IEnumerable<DestinationEvent> Get(Expression<Func<DestinationEvent, bool>> criteria);
    }
}
