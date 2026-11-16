using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Extensions;
using WebApplication2.MiddleWares;

namespace WebApplication2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddServices(builder.Configuration);

            var app = builder.Build();

            // Automatic Migration
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                await StoreContextSeeding.SeedAsync(context);
            }

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<RequestTimingiddleware>();

            // Needs Authentication and Authorization to work properly
            //app.UseMiddleware<RequestIdHeaderMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
