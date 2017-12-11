using System.Data.Entity;
using CPMS.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class UnitOfWork : DbContext
    {
        public DbSet<CompletedEvent> CompletedEvents { get; set; }
        public DbSet<EventMilestone> EventMilestones { get; set; }
        public DbSet<EventName> EventNames { get; set; }
        public DbSet<RuleViolation> RuleViolations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
            modelBuilder.Entity<EventName>().HasKey(p => p.Code);
        }
    }
}
