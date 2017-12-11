using System.Data.Entity;
using CPMS.Configuration;
using CPMS.Patient.Domain;

namespace GenerateData
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PlannedEvent> PlannedEvents { get; set; }
        public DbSet<Pathway> Pathways { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<SourceEvent> SourceEvents { get; set; }
        public DbSet<DestinationEvent> DestinationEvents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
            modelBuilder.Entity<SourceEvent>().HasKey(p => p.SourceCode);
        }
    }
}