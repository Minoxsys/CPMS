using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Authorization;
using  System.Data.Entity;

namespace CPMS.Admin.Core.Adapters
{
    public class PermissionRepository:IPermissionRepository
    {
        public IEnumerable<Permission> Get(Expression<Func<Permission, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Permissions.Include(a=>a.Roles).Where(criteria.Compile()).ToArray();
            }
        }
    }
}
