using Microsoft.EntityFrameworkCore;
using WebApplication1.Repositories;
using WebApplication2.Data;
using WebApplication2.Helper;
using WebApplication2.Interceptors;
using WebApplication2.Repositories;
using WebApplication2.Repository.Interfaces;
using WebApplication2.Repository.Repositories;
using WebApplication2.Services.Interfaces;
using WebApplication2.Services.Repositories;

namespace WebApplication2.Extensions
{
    public static class ApplicationServiceExtenstion
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            // Register DbContext as a scoped service in inversion of control (Ioc)
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(new SoftDeleteInterceptor(), new CreatedAtInterceptor(), new ModifiedAtInterceptor());
            });
          
                            
            // Register AutoMapper as a service in inversion of control (Ioc)
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PatientProfile>();
                cfg.AddProfile<DoctorProfile>();
                cfg.AddProfile<AppointmentProfile>();
            });

            services.AddScoped<IPatientRepository, PatientRepositories>();
            services.AddScoped<IDoctorRepository, DoctorRepositories>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            services.AddScoped(typeof(IGenericRepositroy<>), typeof(GenericRepository<>));

            services.AddScoped<IpatientServices, PatientService>();
            services.AddScoped<IDoctorServices, DoctorService>();
            services.AddScoped<IAppointmentServices, AppointmentService>();

            return services;
        }
    }
}
