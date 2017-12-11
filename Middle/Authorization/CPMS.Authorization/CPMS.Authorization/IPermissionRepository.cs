using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CPMS.Authorization
{
    public interface IPermissionRepository
    {
        IEnumerable<Permission> Get(Expression<Func<Permission, bool>> criteria);
    }
}
