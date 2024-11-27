using CheckEyePro.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CheckEyePro.EF.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Observation>()
                    .HasOne(p => p.Patient)
                    .WithMany(d => d.Observations)
                    .HasForeignKey(p => p.PId)
                    .HasPrincipalKey(d => d.UserId);

            modelBuilder.Entity<Observation>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Observations)
                .HasForeignKey(p => p.DId)
                .HasPrincipalKey(d => d.UserId);
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<History> Histories { get; set; }


    }
}