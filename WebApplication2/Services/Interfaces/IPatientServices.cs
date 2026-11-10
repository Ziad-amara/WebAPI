using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication2.Services.Interfaces
{
    public interface IpatientServices
    {
        Task<GenericResult<IEnumerable<GetPatientResponse>>> GetAllAsync();
        Task<GenericResult<PaginatedList<GetPatientResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1);
        Task<GenericResult<GetPatientResponse>> GetByIdAsync(int id);
        Task<GenericResult<GetPatientResponse>> CreateAsync(AddPatientRequest request);
        Task<Result> UpdateAsync(UpdatePatientRequest request);
        Task<Result> DeleteAsync(int id);
    }
}
