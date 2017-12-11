using System.Linq;

namespace CPMS.Authorization.WebAPI
{
    public abstract class BaseAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private PermissionId _permissionId;
        private bool _permissionHasBeenSet;

        public PermissionId RequiresPermission
        {
            get { return _permissionId; }
            set
            {
                _permissionId = value;
                _permissionHasBeenSet = true;
            }
        }

        protected bool IsAuthorized(string accessToken)
        {
            var existingRoleData = MemcachedClient.Instance.Get<RoleData>(accessToken);

            if (existingRoleData == null)
            {
                return false;
            }

            return !_permissionHasBeenSet ||
                   existingRoleData.Permissions.Any(permission => permission.Id == RequiresPermission);
        }
    }
}
