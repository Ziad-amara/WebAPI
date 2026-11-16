using Microsoft.EntityFrameworkCore;
using WebApplication1.Repositories;
using WebApplication2.Data;
using WebApplication2.filters;
using WebApplication2.Helper;
using WebApplication2.Interceptors;
using WebApplication2.MiddleWares;
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


            // Repositories Design Pattern
            services.AddScoped<IPatientRepository, PatientRepositories>();
            services.AddScoped<IDoctorRepository, DoctorRepositories>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            services.AddScoped(typeof(IGenericRepositroy<>), typeof(GenericRepository<>));


            // Services
            services.AddScoped<IpatientServices, PatientService>();
            services.AddScoped<IDoctorServices, DoctorService>();
            services.AddScoped<IAppointmentServices, AppointmentService>();

            // Middlewares
            services.AddTransient<RequestIdHeaderMiddleware>();

            // Filters
            services.AddScoped<ActionExecutionLoggingFilter>();
            services.AddScoped<ActionTimingFilter>();
            services.AddScoped<ExceptionhandlingFilter>();


            // CORS Configuration
            var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            
            const string corspolicyName = "ConfigurationOrigins";
            services.AddCors(options =>
                options.AddPolicy(corspolicyName, policy =>
                {
                   if (corsOrigins.Length > 0)
                   {
                       policy.WithOrigins(corsOrigins);
                   }

                    // Credintials stay disabled; only exlicitly configured origin receives browser access.
                    policy.AllowAnyHeader()
                         .AllowAnyMethod();

                }));
            

            return services;

        }
    }
}
