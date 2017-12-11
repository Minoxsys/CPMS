using System.Data.Entity;
using CPMS.Domain;

namespace CPMS.Trust.Core.Adapters
{
    public class UnitOfWork : DbContext
    {
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specialty>().HasKey(p => p.Code);
        }
    }
}
