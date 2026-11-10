using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Results;

namespace WebApplication2.Services.Interfaces
{
    public interface IAppointmentServices
    {
        Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1);
        Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetByPatientIdAsync(int patientId, int pageSize, int pageNumber = 1);
        Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetByDoctorIdAsync(int doctorId, int pageSize, int pageNumber = 1);
        Task<Result> CreateAsync(AddAppointmentRequest request);
        Task<Result> UpdateAsync(UpdateAppointmentRequest request, int patientId, int doctorId, DateTime appointmentDate);
        Task<Result> DeleteAsync(int patientId, int doctorId, DateTime appointmentDate);
    }
}
