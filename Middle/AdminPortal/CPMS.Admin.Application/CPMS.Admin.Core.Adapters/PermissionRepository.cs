using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using CPMS.Authorization;
using IPermissionRepository = CPMS.Admin.Application.IPermissionRepository;

namespace CPMS.Admin.Core.Adapters
{
    public class PermissionRepository:IPermissionRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public PermissionRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Permission> Get(Expression<Func<Permission, bool>> criteria, params Expression<Func<Permission, object>>[] includeProperties)
        {
            var permissionsSet = _unitOfWork.Permissions as IQueryable<Permission>;

            permissionsSet = includeProperties.Aggregate(permissionsSet,
                (current, includeProperty) => current.Include(includeProperty));

            return permissionsSet.Where(criteria).ToArray();
        }
    }
}
