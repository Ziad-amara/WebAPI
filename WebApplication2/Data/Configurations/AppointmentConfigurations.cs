using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Entities;


namespace WebApplication2.Data.Configurations
{
    public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
                      
            builder.Property(a => a.AppointmentDate)
                .IsRequired()
                .HasColumnType("datetime");
            
            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
           
            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    
    }
}
