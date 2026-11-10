
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication2.Repository.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<PaginatedList<Appointment>> GetAllAppointments(int pageSize,string? searchTerm,int pageNumber);

        Task<PaginatedList<Appointment>> GetAllAppointmentsByPatientId(int patientId,int pageSize,int pageNumber);

        Task<PaginatedList<Appointment>> GetAllAppointmentsByDoctorId( int doctorId,int pageSize,int pageNumber);

        Task<bool> AddAppointment(AddAppointmentRequest request);

        Task<bool> UpdateAppointment(UpdateAppoinmentRequest request,int patientId,int doctorId,DateTime appointmentDate);

        Task<bool> DeleteAppointment(int patientId, int doctorId,DateTime appointmentDate);
    }
}
