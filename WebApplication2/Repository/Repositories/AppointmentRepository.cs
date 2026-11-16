using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using WebApplication2.Data;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;
using WebApplication2.Repository.Interfaces;
using WebApplication2.Results;

namespace WebApplication2.Repository.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AppointmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAppointment(AddAppointmentRequest request)
        {
            if (request.PatientId is not int patientId ||
                request.DoctorId is not int doctorId)
                return false;

            var patientExists = await _dbContext.Patients
                .AnyAsync(p => p.Id == patientId);

            if (!patientExists)
                return false;

            var doctorExists = await _dbContext.Doctors
                .AnyAsync(d => d.Id == doctorId);

            if (!doctorExists)
                return false;

            var appointmentExists = await _dbContext.Appointments
                .AnyAsync(a =>
                    a.PatientId == patientId &&
                    a.DoctorId == doctorId &&
                    a.AppointmentDate == request.AppointmentDate);

            if (appointmentExists)
                return false;

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = request.AppointmentDate
            };

            await _dbContext.Appointments.AddAsync(appointment);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAppointment(int patientId, int doctorId, DateTime appointmentDate)
        {
            var appointment = await _dbContext.Appointments.FindAsync(
                patientId, doctorId, appointmentDate);

            if (appointment == null)
                return false;

            _dbContext.Appointments.Remove(appointment);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<PaginatedList<Appointment>> GetAllAppointments(int pageSize, string? searchTerm, int pageNumber)
        {
            var appointments = _dbContext.Appointments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int id))
                    appointments = appointments.Where(a =>a.PatientId == id || a.DoctorId == id);
                
                else if (DateTime.TryParse(searchTerm, out DateTime date))
                    appointments = appointments.Where(a => a.AppointmentDate == date);
                
                else
                    throw new ArgumentException("Invalid Search Term!");
                
            }

            var totalRecords = await appointments.CountAsync();

            await appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.PatientId)
                .ThenBy(a => a.DoctorId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Appointment>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Totalpage = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = appointments
            };
        }

        public async Task<PaginatedList<Appointment>> GetAllAppointmentsByDoctorId(int doctorId, int pageSize, int pageNumber)
        {
            var doctorAppointments = _dbContext.Appointments.Where(a => a.DoctorId == doctorId);

            var totalRecords = await doctorAppointments.CountAsync();

            var appointments = await doctorAppointments
                .OrderBy(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Appointment>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Totalpage = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = appointments
            };
        }

        public async Task<PaginatedList<Appointment>> GetAllAppointmentsByPatientId(int patientId, int pageSize, int pageNumber)
        {
            var patientAppointments = _dbContext.Appointments.Where(a => a.PatientId == patientId);

            var totalRecords = await patientAppointments.CountAsync();

            var appointments = await patientAppointments
                .OrderBy(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Appointment>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Totalpage = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = appointments
            };
        }

        public async Task<bool> UpdateAppointment(UpdateAppointmentRequest request, int patientId, int doctorId, DateTime appointmentDate)
        {
            var appointment = await _dbContext.Appointments.FindAsync(patientId, doctorId, appointmentDate);

            if (appointment == null)
                return false;

            if (request.PatientId is not int newPatientId ||
                request.DoctorId is not int newDoctorId ||
                request.AppointmentDate is not DateTime newDate)
                return false;

            var patientExists = await _dbContext.Patients.AnyAsync(p => p.Id == newPatientId);

            if (!patientExists)
                return false;

            var doctorExists = await _dbContext.Doctors.AnyAsync(d => d.Id == newDoctorId);

            if (!doctorExists)
                return false;

            var duplicateAppointment = await _dbContext.Appointments
                .AnyAsync(a =>
                    a.PatientId == newPatientId &&
                    a.DoctorId == newDoctorId &&
                    a.AppointmentDate == newDate &&
                    !(a.PatientId == patientId &&
                      a.DoctorId == doctorId &&
                      a.AppointmentDate == appointmentDate));

            if (duplicateAppointment)
                return false;

            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            _dbContext.Appointments.Remove(appointment);

            var updatedAppointment = new Appointment
            {
                PatientId = newPatientId,
                DoctorId = newDoctorId,
                AppointmentDate = newDate
            };

            await _dbContext.Appointments.AddAsync(updatedAppointment);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
    }
}
