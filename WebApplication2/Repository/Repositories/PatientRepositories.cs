using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.Repository.Interfaces;
using WebApplication2.Results;

namespace WebApplication2.Repository.Repositories
{
    public class PatientRepositories : IPatientRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PatientRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
                    => await _dbContext.Patients.ToListAsync();

        public async Task<PaginatedList<Patient>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            List<Patient> patients;
            var totalCounts = 0;
            if (searchTerm is not null)
            {
                patients = _dbContext.Set<Patient>()
                    .Where(p => p.Name.Contains(searchTerm))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                totalCounts = await _dbContext.Patients.CountAsync(p => p.Name.Contains(searchTerm));
            }
            else
            {
                patients = _dbContext.Set<Patient>()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                totalCounts = await _dbContext.Patients.CountAsync();
            }
            var paginatedResponse = new PaginatedList<Patient>()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling((double)totalCounts / pageSize),
                Data = patients
            };
            return paginatedResponse;
        }

        public async Task<Patient?> GetPatientById(int id)
            => await _dbContext.Patients.FindAsync(id);

        public async Task<Patient> AddPatient(AddPatientRequest addPatient)
        {
            var patient = new Patient
            {
                Name = addPatient.Name,
                DateOfBirth = addPatient.DateOfBirth
            };

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();
            return patient;
        }



        public async Task<bool> UpdatePatient(UpdatePatientRequest updatePatient)
        {
            var existingPatient = await GetPatientById(updatePatient.Id);

            if(existingPatient is null)
                return false;

            existingPatient.Name = updatePatient.Name;
            existingPatient.DateOfBirth = updatePatient.DateOfBirth;

            await _dbContext.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> DeletePatient(int id)
        {
            var entity = await GetPatientById(id);

            if (entity is null)
                return false;

            _dbContext.Patients.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
