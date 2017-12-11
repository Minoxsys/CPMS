using System.Collections.Generic;
using CPMS.Authorization;

namespace CPMS.Admin.Application
{
    public class RolePermissionsInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<PermissionId> PermissionIds { get; set; }
    }
}
