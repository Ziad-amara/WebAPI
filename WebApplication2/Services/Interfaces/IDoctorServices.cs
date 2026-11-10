using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Results;

namespace WebApplication2.Services.Interfaces
{
    public interface IDoctorServices
    {
        Task<GenericResult<IEnumerable<GetDoctorResponse>>> GetAllAsync();
        Task<GenericResult<PaginatedList<GetDoctorResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1);
        Task<GenericResult<GetDoctorResponse>> GetByIdAsync(int id);
        Task<GenericResult<GetDoctorResponse>> CreateAsync(AddDoctorRequest request);
        Task<Result> UpdateAsync(UpdateDoctorRequest request);
        Task<Result> DeleteAsync(int id);
    }
}
