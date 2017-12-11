using System.Data.Entity;

namespace CPMS.Breach.Engine.Adapters
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PlannedEvent> PlannedEvents { get; set; }
        public DbSet<Pathway> Pathways { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasKey(p => p.NHSNumber);
            modelBuilder.Entity<Pathway>().HasKey(p => p.PPINumber);
        }
    }
}
