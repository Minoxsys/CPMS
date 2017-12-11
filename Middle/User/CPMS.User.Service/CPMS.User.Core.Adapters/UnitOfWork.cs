using System.Data.Entity;
using CPMS.Authorization;

namespace CPMS.User.Core.Adapters
{
    public class UnitOfWork: DbContext
    {
        public DbSet<Authorization.User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}
