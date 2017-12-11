using System.Collections.Generic;

namespace CPMS.Admin.Presentation
{
    public class RolesPermissionsMappingViewModel
    {
        public IEnumerable<RolePermissionsViewModel> Roles;

        public IEnumerable<PermissionViewModel> Permissions;
    }
}
