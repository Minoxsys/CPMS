using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Authorization;
using IRoleRepository = CPMS.Admin.Application.IRoleRepository;

namespace CPMS.Admin.Core.Adapters
{
    public class RoleRepository:IRoleRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public RoleRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Role> Get(Expression<Func<Role, bool>> criteria, params Expression<Func<Role, object>>[] includeProperties)
        {
            var rolesSet = _unitOfWork.Roles as IQueryable<Role>;

            rolesSet = includeProperties.Aggregate(rolesSet,
                (current, includeProperty) => current.Include(includeProperty));

            return rolesSet.Where(criteria).ToArray();
        }
    }
}
