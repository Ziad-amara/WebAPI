using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.filters;
using WebApplication2.Repository.Interfaces;
using WebApplication2.Results;
using WebApplication2.Services;
using WebApplication2.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ExceptionhandlingFilter))]
    [ServiceFilter(typeof(ActionExecutionLoggingFilter))]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IpatientServices _service;
        private readonly ILogger _logger;

        public PatientController(IpatientServices service, ILogger<PatientController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllPatients")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetAll(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var result = await _service.GetAllAsync(pageSize, searchTerm, pageNumber);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllPatientsWithoutPagination")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetAllWithoutPagination()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        [HttpGet]
        [Route("GetPatientById/{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("AddPatient")]
        public async Task<IActionResult> AddPatient(AddPatientRequest addPatient)
        {

            var result = await _service.CreateAsync(addPatient);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Patient added successfully" });
        }

        [HttpPut]
        [Route("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient(UpdatePatientRequest updatePatient)
        {
            var result = await _service.UpdateAsync(updatePatient);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Patient updated successfully" });
        }

        [HttpDelete]
        [Route("DeletePatient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Patient deleted successfully" });
        }


        private IActionResult FailureResponse(Result result)
            => result.Error == ServiceErrors.PatientNotFound
                ? NotFound(new { Message = result.Error })
                : BadRequest(new { Message = result.Error });
    }
}