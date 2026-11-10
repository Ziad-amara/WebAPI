using AutoMapper;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.Results;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Repositories
{
    public class AppointmentService(IGenericRepositroy<Appointment> repositroy,
        IGenericRepositroy<Patient> patientRepository,
        IGenericRepositroy<Doctor> doctorRepository,
        IMapper mapper) : IAppointmentServices
    {
        public async Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var appointments = await repositroy.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out var id))
                    appointments = appointments.Where(a => a.PatientId == id || a.DoctorId == id);
                
                else if (DateTime.TryParse(searchTerm, out var date))
                {
                    var searchDate = ToUtc(date);
                    appointments = appointments.Where(a => a.AppointmentDate == searchDate);
                }
                
                else
                    return GenericResult<PaginatedList<GetAppointmentsResponse>>.Failure("Invalid search term");
            }

            return GenericResult<PaginatedList<GetAppointmentsResponse>>.Success(ToPage(appointments, pageSize, pageNumber));
        }

        public async Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetByPatientIdAsync(int patientId, int pageSize, int pageNumber = 1)
        {
            var appointments = await repositroy.GetAllAsync();

            return GenericResult<PaginatedList<GetAppointmentsResponse>>.Success(
                ToPage(appointments.Where(a => a.PatientId == patientId), pageSize, pageNumber));
        }

        public async Task<GenericResult<PaginatedList<GetAppointmentsResponse>>> GetByDoctorIdAsync(int doctorId, int pageSize, int pageNumber = 1)
        {
            var appointments = await repositroy.GetAllAsync();

            return GenericResult<PaginatedList<GetAppointmentsResponse>>.Success(
                ToPage(appointments.Where(a => a.DoctorId == doctorId), pageSize, pageNumber));
        }

        public async Task<Result> CreateAsync(AddAppointmentRequest request)
        {
            if (request.PatientId is not int patientId || request.DoctorId is not int doctorId)
                return Result.Failure("Patient and doctor are required");

            var validation = await ValidateAsync(
                patientId, doctorId, request.AppointmentDate, excludeId: null, requireFutureDate: true);
            if (validation.IsFailure)
                return validation;

            var appointment = mapper.Map<Appointment>(request);
            appointment.AppointmentDate = ToUtc(request.AppointmentDate);

            await repositroy.CreateAsync(appointment);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateAppointmentRequest request, int patientId, int doctorId, DateTime appointmentDate)
        {
            if (request.PatientId is not int newPatientId ||
                request.DoctorId is not int newDoctorId ||
                request.AppointmentDate is not DateTime newDate)
                return Result.Failure("Patient, doctor and appointment date are required");

            var appointment = await FindAsync(patientId, doctorId, appointmentDate);
            if (appointment is null)
                return Result.Failure(ServiceErrors.AppointmentNotFound);

            var dateChanged = ToUtc(newDate) != appointment.AppointmentDate;

            var validation = await ValidateAsync(
                newPatientId, newDoctorId, newDate, excludeId: appointment.Id, requireFutureDate: dateChanged);
            if (validation.IsFailure)
                return validation;

            mapper.Map(request, appointment);
            appointment.AppointmentDate = ToUtc(newDate);

            var updated = await repositroy.UpdateAsync(appointment);

            return updated ? Result.Success() : Result.Failure("Failed to update appointment");
        }

        public async Task<Result> DeleteAsync(int patientId, int doctorId, DateTime appointmentDate)
        {
            var appointment = await FindAsync(patientId, doctorId, appointmentDate);
            if (appointment is null)
                return Result.Failure(ServiceErrors.AppointmentNotFound);

            var deleted = await repositroy.DeleteAsync(appointment.Id);

            return deleted ? Result.Success() : Result.Failure("Failed to delete appointment");
        }

        private async Task<Appointment?> FindAsync(int patientId, int doctorId, DateTime appointmentDate)
        {
            var date = ToUtc(appointmentDate);
            var appointments = await repositroy.GetAllAsync();

            return appointments.FirstOrDefault(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate == date);
        }

        private async Task<Result> ValidateAsync(int patientId, int doctorId, DateTime appointmentDate, int? excludeId, bool requireFutureDate)
        {
            var date = ToUtc(appointmentDate);

            if (requireFutureDate && date <= DateTime.UtcNow)
                return Result.Failure("Appointment date must be in the future");

            if (await patientRepository.GetByIdAsync(patientId) is null)
                return Result.Failure(ServiceErrors.PatientNotFound);

            if (await doctorRepository.GetByIdAsync(doctorId) is null)
                return Result.Failure(ServiceErrors.DoctorNotFound);

            var others = (await repositroy.GetAllAsync())
                .Where(a => a.Id != excludeId && a.AppointmentDate == date)
                .ToList();

            if (others.Any(a => a.PatientId == patientId && a.DoctorId == doctorId))
                return Result.Failure("This appointment already exists");

            if (others.Any(a => a.DoctorId == doctorId))
                return Result.Failure("The doctor already has an appointment at this time");

            if (others.Any(a => a.PatientId == patientId))
                return Result.Failure("The patient already has an appointment at this time");

            return Result.Success();
        }

        private static DateTime ToUtc(DateTime date)
            => date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : date;

        private PaginatedList<GetAppointmentsResponse> ToPage(IEnumerable<Appointment> source, int pageSize, int pageNumber)
        {
            var ordered = source
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.PatientId)
                .ThenBy(a => a.DoctorId)
                .ToList();

            var page = new PaginatedList<GetAppointmentsResponse> { PageNumber = pageNumber < 1 ? 1 : pageNumber };
            if (pageSize > 0)
                page.PageSize = pageSize;

            var items = ordered
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToList();

            page.TotalRecords = ordered.Count;
            page.Totalpage = (int)Math.Ceiling(ordered.Count / (double)page.PageSize);
            page.Data = mapper.Map<IEnumerable<GetAppointmentsResponse>>(items);

            return page;
        }
    }

}
