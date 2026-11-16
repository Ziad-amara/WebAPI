using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.DoctorDtos;
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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorServices _service;
        private readonly ILogger _logger;

        public DoctorController(IDoctorServices service, ILogger<DoctorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllDoctorsWithoutPagination")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetAllWithoutPagination()
        {
            var doctors = await _service.GetAllAsync();

            return Ok(doctors);
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        [ServiceFilter(typeof(ActionTimingFilter))]
        public async Task<IActionResult> GetAllDoctors(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var doctors = await _service.GetAllAsync(pageSize, searchTerm, pageNumber);

            return Ok(doctors);
        }

        [HttpGet]
        [Route("GetDoctorById/{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("AddDoctor")]
        public async Task<IActionResult> AddDoctor(AddDoctorRequest addDoctor)
        {
            var result = await _service.CreateAsync(addDoctor);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Doctor added successfully" });

        }

        [HttpPut]
        [Route("updateDoctor/{id}")]
        public async Task<IActionResult> UpdateDoctor(UpdateDoctorRequest updateDoctor)
        {
            var result = await _service.UpdateAsync(updateDoctor);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Doctor updated successfully" });
        }

        [HttpDelete]
        [Route("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (result.IsFailure)
                return FailureResponse(result);

            return Ok(new { Message = "Doctor deleted successfully" });

        }

        private IActionResult FailureResponse(Result result)
            => result.Error == ServiceErrors.DoctorNotFound
                ? NotFound(new { Message = result.Error })
                : BadRequest(new { Message = result.Error });
    }

}