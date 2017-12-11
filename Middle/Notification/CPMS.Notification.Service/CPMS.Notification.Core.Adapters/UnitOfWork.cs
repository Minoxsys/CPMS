using System.Data.Entity;
using CPMS.Patient.Domain;

namespace CPMS.Notification.Core.Adapters
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<PlannedEvent> PlannedEvents { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Pathway> Pathways { get; set; }
        public DbSet<Period> Periods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient.Domain.Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
        }
    }
}
