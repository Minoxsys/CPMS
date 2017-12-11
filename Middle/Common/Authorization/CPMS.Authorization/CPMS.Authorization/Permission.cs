using System.Collections.Generic;

namespace CPMS.Authorization
{
    public class Permission
    {
        public PermissionId Id { get; set; }

        public string Name { get; set; }

        public IList<Role> Roles { get; set; }
    }
}
