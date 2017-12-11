using System.Data.Entity;
using CPMS.Authorization;
using CPMS.Configuration;
using CPMS.Domain;

namespace CPMS.Admin.Core.Adapters
{
    public class UnitOfWork: DbContext
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DestinationEvent> DestinationEvents { get; set; }
        public DbSet<SourceEvent> SourceEvents { get; set; }
        public DbSet<EventName> EventNames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventName>().HasKey(p => p.Code);
        }
    }
}
