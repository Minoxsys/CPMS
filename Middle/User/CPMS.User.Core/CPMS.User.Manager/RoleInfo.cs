using System.Collections.Generic;
using CPMS.Authorization;

namespace CPMS.User.Manager
{
    public class RoleInfo
    {
        public string Name { get; set; }

        public IEnumerable<PermissionId> Permissions { get; set; }
    }
}
