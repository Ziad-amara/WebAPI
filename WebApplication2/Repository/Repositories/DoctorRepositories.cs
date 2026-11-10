using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;
using WebApplication2.Repository.Interfaces;
using WebApplication2.Results;

namespace WebApplication2.Repository.Repositories
{
    public class DoctorRepositories : IDoctorRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DoctorRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
                    => await _dbContext.Doctors.ToListAsync();

        public async Task<PaginatedList<Doctor>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            List<Doctor> doctors;
            var totalCounts = 0;
            if (searchTerm is not null)
            {
                doctors = _dbContext.Set<Doctor>()
                    .Where(d => d.Name.Contains(searchTerm))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                totalCounts = await _dbContext.Doctors.CountAsync(d => d.Name.Contains(searchTerm));
            }
            else
            {
                doctors = _dbContext.Set<Doctor>()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                totalCounts = await _dbContext.Doctors.CountAsync();
            }
            var paginatedResponse = new PaginatedList<Doctor>()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling((double)totalCounts / pageSize),
                Data = doctors  
            };
            return paginatedResponse;
        }

        public async Task<Doctor?> GetDoctorById(int id)
            => await _dbContext.Doctors.FindAsync(id);

        public async Task<Doctor> AddDoctor(AddDoctorRequest addDoctor)
        {
            var doctor = new Doctor
            {
                Name = addDoctor.Name,
                Specialization = addDoctor.Specialization
            };

            await _dbContext.Doctors.AddAsync(doctor);
            await _dbContext.SaveChangesAsync();
            return doctor;
        }



        public async Task<bool> UpdateDoctor(UpdateDoctorRequest updateDoctor)
        {
            var existingDoctor = await GetDoctorById(updateDoctor.Id);

            if (existingDoctor is null)
                return false;

            existingDoctor.Name = updateDoctor.Name;
            existingDoctor.Specialization = updateDoctor.Specialization;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteDoctor(int id)
        {
            var entity = await GetDoctorById(id);

            if (entity is null)
                return false;

            _dbContext.Doctors.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
