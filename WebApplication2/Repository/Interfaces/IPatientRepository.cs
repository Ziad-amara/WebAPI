using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication2.Repository.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<PaginatedList<Patient>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1);
        Task<Patient?> GetPatientById(int id);
        Task<Patient> AddPatient(AddPatientRequest addPatient);
        Task<bool> UpdatePatient(UpdatePatientRequest updatePatient);
        Task<bool> DeletePatient(int id);
    }
}
