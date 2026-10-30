using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AppointmentsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Get All Appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _dbContext.Appointments.ToListAsync();
            return Ok(appointments);
        }

        [HttpGet]
        [Route("GetAllAppointmentsByPatientId/{patientid}")]
        public async Task<IActionResult> GetPatientAppointments(int patientid)
        {
            var appointments = await _dbContext.Set<Appointment>()
                .Where(a => a.PatientId == patientid)
                .ToListAsync();

            return Ok(appointments);
        }
        
        
        [HttpGet]
        [Route("GetAllAppointmentsByDoctorId/{patientid}")]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId)
        {
            var appointments = await _dbContext.Set<Appointment>()
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost]
        [Route("AddAppointment")]
        public async Task<IActionResult> AddAppointment(AddAppointmentRequest addAppointment)
        {
            if (addAppointment.PatientId is not int patientId)
                return BadRequest("PatientId is required");

            if (addAppointment.DoctorId is not int doctorId)
                return BadRequest("DoctorId is required");

            var PatientExists = await _dbContext.Set<Patient>()
                .AnyAsync(p => p.Id == addAppointment.PatientId);

            if (!PatientExists)
                return NotFound("Patient Not Found");

            var DoctorExists = await _dbContext.Set<Doctor>()
                .AnyAsync(d => d.Id == addAppointment.DoctorId);

            if (!DoctorExists)
                return NotFound("Doctor Not Found");

            var appointmentExists = await _dbContext.Appointments
                .AnyAsync(a =>
                    a.PatientId == addAppointment.PatientId &&
                    a.DoctorId == addAppointment.DoctorId &&
                    a.AppointmentDate == addAppointment.AppointmentDate);

            if (appointmentExists)
                return Conflict("Appointment already exists");

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = addAppointment.AppointmentDate
            };

            await _dbContext.Appointments.AddAsync(appointment);
            await _dbContext.SaveChangesAsync();
            return Ok("Appointment Added Successfully");
        }

        [HttpPut]
        [Route("UpdateAppointment/{patientId}/{doctorId}")]
        public async Task<IActionResult> UpdateAppointment(UpdateAppoinmentRequest updateAppoinment, int patientId, int doctorId, [FromQuery] DateTime appointmentDate)
        {
            var appointment = await _dbContext.Appointments
                .FindAsync(patientId, doctorId, appointmentDate);

            if (appointment == null)
                return NotFound("Appointment Not Found");

            if (updateAppoinment.PatientId is not int newPatientId ||
                updateAppoinment.DoctorId is not int newDoctorId ||
                updateAppoinment.AppointmentDate is not DateTime newDate)
            {
                return BadRequest("Invalid appointment data");
            }

            var patientExists = await _dbContext.Set<Patient>()
                .AnyAsync(p => p.Id == newPatientId);

            if (!patientExists)
                return NotFound("Patient Not Found");

            var doctorExists = await _dbContext.Set<Doctor>()
                .AnyAsync(d => d.Id == newDoctorId);

            if (!doctorExists)
                return NotFound("Doctor Not Found");

            var duplicateAppointment = await _dbContext.Appointments
                    .AnyAsync(a =>
                        a.PatientId == newPatientId &&
                        a.DoctorId == newDoctorId &&
                        a.AppointmentDate == newDate &&
                        !(a.PatientId == patientId &&
                            a.DoctorId == doctorId &&
                            a.AppointmentDate == appointmentDate));

            if (duplicateAppointment)
                return Conflict("Appointment Already Exists");

            appointment.PatientId = newPatientId;
            appointment.DoctorId = newDoctorId;
            appointment.AppointmentDate = newDate;

            await _dbContext.SaveChangesAsync();

            return Ok("Appointment Updated Successfully");
        }

        [HttpDelete]
        [Route("DeleteAppointment/{patientId}/{doctorId}")]
        public async Task<IActionResult> DeleteAppointment(int patientId, int doctorId, [FromQuery] DateTime appointmentDate)
        {
            var appointment = await _dbContext.Appointments
                .FindAsync(patientId, doctorId, appointmentDate);

            if (appointment == null)
                return NotFound("Appointment Not Found");

            _dbContext.Appointments.Remove(appointment);
            await _dbContext.SaveChangesAsync();

            return Ok("Appointment Deleted");

        }
    }
}
