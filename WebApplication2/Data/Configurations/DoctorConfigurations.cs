using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Entities;

namespace WebApplication2.Data.Configurations
{
    public class DoctorConfigurations :IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasQueryFilter(p => !p.IsDeleted);

        }
    }
}
