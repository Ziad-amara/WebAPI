using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppoinmentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AppoinmentController> _logger;
        private readonly IMapper _mapper;

        public AppoinmentController(ApplicationDbContext dbContext, ILogger<AppoinmentController> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Get All Appointments")]
        public async Task<IActionResult> GetAllAppointments(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var totalCounts = 0;
            var message = "getting all appointments";

            var searchedResult = _dbContext.Appointments
                .AsQueryable();


            if (searchTerm is not null)
            {
                if (int.TryParse(searchTerm, out int id))
                {
                    searchedResult = _dbContext.Appointments.Where(a =>
                    a.PatientId == id || a.DoctorId == id);
                }
                else if (DateTime.TryParse(searchTerm, out DateTime date))
                {
                    searchedResult = _dbContext.Appointments.Where(d => d.AppointmentDate == date);
                }
                else
                {
                    return BadRequest("Invalid Search Term!");
                }
            }

            totalCounts = await searchedResult.CountAsync();

            var appointments = await searchedResult
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.PatientId)
                .ThenBy(a => a.DoctorId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation(message, appointments);

            var appointmentsResult = _mapper.Map<List<Appointment>, List<GetAppointmentsResponse>>(appointments);

            var paginatedResult = new PaginatedList<GetAppointmentsResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling(totalCounts / (double)pageSize),
                Data = appointmentsResult
            };

            return Ok(GenericResult<PaginatedList<GetAppointmentsResponse>>
            .Success(paginatedResult));

        }

        [HttpGet]
        [Route("GetAllAppointmentsByPatientId/{patientId}")]
        public async Task<IActionResult> GetPatientAppointments(int patientId, int pageSize, int pageNumber = 1)
        {
            var totalCounts = 0;
            var message = "Getting all appointments for patient by patient id";

            if (pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var appointments = await _dbContext.Appointments
                .Where(p => p.PatientId == patientId)
                .OrderBy(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            totalCounts = await _dbContext.Appointments.CountAsync(p => p.PatientId == patientId);

            _logger.LogInformation(message, appointments);

            var patientAppointmentsResponse = _mapper.Map<List<Appointment>, List<GetAppointmentsResponse>>(appointments);

            var paginatedResponse = new PaginatedList<GetAppointmentsResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling(totalCounts / (double)pageSize),
                Data = patientAppointmentsResponse
            };

            return Ok(GenericResult<PaginatedList<GetAppointmentsResponse>>.Success(paginatedResponse));
        }

        [HttpGet]
        [Route("GetAllAppointmentsByDoctorId/{doctorId}")]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId, int pageSize, int pageNumber = 1)
        {
            var totalCounts = 0;
            var message = "getting all appointments for the doctor by doctor id";

            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var appointments = await _dbContext.Appointments
                .Where(p => p.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            totalCounts = await _dbContext.Appointments.CountAsync(d => d.DoctorId == doctorId);

            _logger.LogInformation(message, appointments);

            var doctorAppointmentsResponses = _mapper.Map<List<Appointment>, List<GetAppointmentsResponse>>(appointments);

            var paginatedResponse = new PaginatedList<GetAppointmentsResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling(totalCounts / (double)pageSize),
                Data = doctorAppointmentsResponses
            };

            return Ok(GenericResult<PaginatedList<GetAppointmentsResponse>>.Success(paginatedResponse));
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
