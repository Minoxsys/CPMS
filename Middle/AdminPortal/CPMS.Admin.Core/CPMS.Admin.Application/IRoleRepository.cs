using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Authorization;

namespace CPMS.Admin.Application
{
    public interface IRoleRepository
    {
        IEnumerable<Role> Get(Expression<Func<Role, bool>> criteria, params Expression<Func<Role, object>>[] includeProperties);
    }
}
