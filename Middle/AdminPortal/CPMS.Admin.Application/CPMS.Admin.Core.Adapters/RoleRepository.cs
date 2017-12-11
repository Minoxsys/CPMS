using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CPMS.Authorization;

namespace CPMS.Admin.Core.Adapters
{
    public class RoleRepository:IRoleRepository
    {
        public void Create(Role role)
        {
        }

        public void Update(Role role)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var currentRole = unitOfWork.Roles.Include(r=>r.Permissions).Single(r => r.Id == role.Id);

                if (currentRole != null)
                {
                    currentRole.Name = role.Name;

                    if (role.Permissions != null)
                    {
                        currentRole.Permissions.Clear();

                        var permissionIds = role.Permissions.Select(p => p.Id).ToArray();
                        var permissions = unitOfWork.Permissions.Where(p => permissionIds.Contains(p.Id)).ToArray();

                        foreach (var permission in permissions)
                        {
                            currentRole.Permissions.Add(permission);
                        }
                    }

                    unitOfWork.SaveChanges();
                }
            }
        }

        public IEnumerable<Role> Get(Expression<Func<Role, bool>> criteria)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Roles.Include(r=>r.Permissions).Where(criteria.Compile()).ToArray();
            }
        }
    }
}
