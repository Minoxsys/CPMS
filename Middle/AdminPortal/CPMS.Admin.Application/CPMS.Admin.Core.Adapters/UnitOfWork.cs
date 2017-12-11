using System.Data.Entity;
using CPMS.Authorization;
using CPMS.Configuration;

namespace CPMS.Admin.Core.Adapters
{
    public class UnitOfWork: DbContext
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DestinationEvent> DestinationEvents { get; set; }
        public DbSet<SourceEvent> SourceEvents { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SourceEvent>().HasKey(p => p.SourceCode);
        }
    }
}
