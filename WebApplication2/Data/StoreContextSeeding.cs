using System.Text.Json;
using WebApplication2.Data;
using WebApplication2.Entities;

namespace WebApplication2.Data
{
    public static class StoreContextSeeding
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if(!dbContext.Patients.Any())
            {
                var patientsData = File.ReadAllText("D:\\ziyad\\DEPI\\technical\\WebAPI\\WebApplication2\\WebApplication2\\Data\\Seeding\\Patients.json");
                var patients = JsonSerializer.Deserialize<List<Patient>>(patientsData);

                if(patients is null)
                {
                    return;
                }

                await dbContext.Patients.AddRangeAsync(patients);
                await dbContext.SaveChangesAsync();
            }
            if(!dbContext.Doctors.Any())
            {
                var doctorsData = File.ReadAllText("D:\\ziyad\\DEPI\\technical\\WebAPI\\WebApplication2\\WebApplication2\\Data\\Seeding\\Doctors.json");
                var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorsData);

                if (doctors is null)
                {
                    return;
                }

                await dbContext.Doctors.AddRangeAsync(doctors);
                await dbContext.SaveChangesAsync();
            }
            if(!dbContext.Appointments.Any())
            {
                var appointmentsData = File.ReadAllText("D:\\ziyad\\DEPI\\technical\\WebAPI\\WebApplication2\\WebApplication2\\Data\\Seeding\\Appointments.json");
                var appointements = JsonSerializer.Deserialize<List<Appointment>>(appointmentsData);

                if (appointements is null)
                {
                    return;
                }

                await dbContext.Appointments.AddRangeAsync(appointements);
                await dbContext.SaveChangesAsync();

            }
        }
    }
}
