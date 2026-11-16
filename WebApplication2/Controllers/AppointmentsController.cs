using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.filters;
using WebApplication2.Results;
using WebApplication2.Services;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ExceptionhandlingFilter))]
    [ServiceFilter(typeof(ActionExecutionLoggingFilter))]
    public class AppoinmentController : ControllerBase
    {
        private readonly IAppointmentServices _service;
        private readonly ILogger<AppoinmentController> _logger;

        public AppoinmentController(IAppointmentServices service, ILogger<AppoinmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllAppointments")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetAllAppointments(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination values.");

            var result = await _service.GetAllAsync(pageSize, searchTerm, pageNumber);

            if (result.IsFailure)
                return FailureResponse(result);

            _logger.LogInformation("Getting all appointments");

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllAppointmentsByPatientId/{patientId}")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetPatientAppointments(int patientId, int pageSize, int pageNumber = 1)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination values.");

            var result = await _service.GetByPatientIdAsync(patientId, pageSize, pageNumber);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllAppointmentsByDoctorId/{doctorId}")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId, int pageSize, int pageNumber = 1)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination values.");

            var result = await _service.GetByDoctorIdAsync(doctorId, pageSize, pageNumber);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("AddAppointment")]
        public async Task<IActionResult> AddAppointment(AddAppointmentRequest request)
        {
            var result = await _service.CreateAsync(request);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok("Appointment Added Successfully");
        }

        [HttpPut]
        [Route("UpdateAppointment/{patientId}/{doctorId}")]
        public async Task<IActionResult> UpdateAppointment(UpdateAppointmentRequest request,int patientId,int doctorId,[FromQuery] DateTime appointmentDate)
        {
            var result = await _service.UpdateAsync(request, patientId, doctorId, appointmentDate);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok("Appointment Updated Successfully");
        }

        [HttpDelete]
        [Route("DeleteAppointment/{patientId}/{doctorId}")]
        public async Task<IActionResult> DeleteAppointment(int patientId, int doctorId,[FromQuery] DateTime appointmentDate)
        {
            var result = await _service.DeleteAsync(patientId, doctorId, appointmentDate);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok("Appointment Deleted Successfully");
        }

        private IActionResult FailureResponse(Result result)
            => result.Error == ServiceErrors.AppointmentNotFound
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}
