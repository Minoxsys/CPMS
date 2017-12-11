using System.Data.Entity;
using CPMS.Domain;

namespace CPMS.Core.Adapters
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Domain.Patient> Patients { get; set; }
        public DbSet<CompletedEvent> CompletedEvents { get; set; }
        public DbSet<EventMilestone> EventMilestones { get; set; }
        public DbSet<EventName> EventNames { get; set; }
        public DbSet<Pathway> Pathways { get; set; }
        public DbSet<Period> Periods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
            modelBuilder.Entity<EventName>().HasKey(p => p.Code);
        }
    }
}
