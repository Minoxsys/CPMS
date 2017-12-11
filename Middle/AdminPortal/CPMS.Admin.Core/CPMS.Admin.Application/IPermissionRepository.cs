using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CPMS.Authorization;

namespace CPMS.Admin.Application
{
    public interface IPermissionRepository
    {
        IEnumerable<Permission> Get(Expression<Func<Permission, bool>> criteria, params Expression<Func<Permission, object>>[] includeProperties);
    }
}
