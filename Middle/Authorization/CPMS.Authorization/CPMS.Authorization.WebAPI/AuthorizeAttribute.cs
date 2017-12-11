using System.Linq;

namespace CPMS.Authorization.WebAPI
{
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private PermissionId _permissionId;
        private bool permissionHasBeenSet;

        public PermissionId RequiresPermission
        {
            get { return _permissionId; }
            set
            {
                _permissionId = value;
                permissionHasBeenSet = true;
            }
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Contains(Constants.AccessTokenHeader))
            {
                return false;
            }

            var accessToken =
                actionContext.Request.Headers.GetValues(Constants.AccessTokenHeader).FirstOrDefault();

            var existingRoleData = MemcachedClient.Instance.Get<RoleData>(accessToken);

            if (existingRoleData == null)
            {
                return false;
            }

            return !permissionHasBeenSet ||
                   existingRoleData.Permissions.Any(permission => permission.Id == RequiresPermission);
        }
    }
}
