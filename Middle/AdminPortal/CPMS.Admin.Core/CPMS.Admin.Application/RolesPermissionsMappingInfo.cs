using System.Collections.Generic;
using CPMS.Authorization;

namespace CPMS.Admin.Application
{
    public class RolesPermissionsMappingInfo
    {
        public IEnumerable<Role> Roles;

        public IEnumerable<Permission> Permissions;
      
    }
}
