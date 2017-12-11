using System.Linq;

namespace CPMS.Authorization
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        
        public Role Role { get; set; }

        public bool IsActive { get; set; }
        
        public string RefreshToken { get; set; }

        public bool HasPermission(PermissionId permissionId)
        {
            return Role.Permissions.Any(permission => permission.Id == permissionId);
        }
    }
}
