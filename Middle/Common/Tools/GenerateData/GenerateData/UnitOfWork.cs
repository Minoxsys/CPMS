using System.Data.Entity;
using CPMS.Configuration;
using CPMS.Domain;

namespace GenerateData
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<CompletedEvent> CompletedEvents { get; set; }
        public DbSet<EventMilestone> EventMilestones { get; set; }
        public DbSet<EventName> EventNames { get; set; }
        public DbSet<Pathway> Pathways { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<RuleViolation> RuleViolations { get; set; }
        public DbSet<SourceEvent> SourceEvents { get; set; }
        public DbSet<DestinationEvent> DestinationEvents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
            modelBuilder.Entity<EventName>().HasKey(p => p.Code);
        }
    }
}