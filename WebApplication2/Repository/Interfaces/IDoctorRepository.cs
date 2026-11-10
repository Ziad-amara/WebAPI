using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication2.Repository.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<PaginatedList<Doctor>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1);
        Task<Doctor?> GetDoctorById(int id);
        Task<Doctor> AddDoctor(AddDoctorRequest addDoctor);
        Task<bool> UpdateDoctor(UpdateDoctorRequest updateDoctor);
        Task<bool> DeleteDoctor(int id);
    }
}
