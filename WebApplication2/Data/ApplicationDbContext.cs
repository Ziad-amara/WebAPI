using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.Configurations;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentConfigurations).Assembly);
        }

        public DbSet<Entities.Patient> Patients { get; set; }
        public DbSet<Entities.Doctor> Doctors { get; set; }
        public DbSet<Entities.Appointment> Appointments { get; set; }
    }
}
