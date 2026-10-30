using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PatientController(ApplicationDbContext dbContext, ILogger<PatientController> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;

        }

        [HttpGet]
        [Route("GetAllPatients")]
        public async Task<IActionResult> GetAll(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            List<Patient> patients;
            var totalCounts = 0;
            var message = "Get All patients Endpoint";

            if (searchTerm is not null)
            {
                patients = _dbContext.Set<Patient>()
                    .Where(p => p.Name.Contains(searchTerm))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                totalCounts = await _dbContext.Patients.CountAsync(p => p.Name.Contains(searchTerm));
            }
            else
            {
                patients = _dbContext.Set<Patient>()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                totalCounts = await _dbContext.Patients.CountAsync();

            }


            _logger.LogInformation(message, patients);

            var patientsResponse = _mapper.Map<List<Patient>, List<GetPatientResponse>>(patients);
            var paginatedResponse = new PaginatedList<GetPatientResponse>()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling(totalCounts / (double)pageSize),
                Data = patientsResponse
            };

            var result = GenericResult<PaginatedList<GetPatientResponse>>.Success(paginatedResponse);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetPatientById/{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _dbContext.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound(new { Message = "Patient not found" });
            }

            var response = _mapper.Map<GetPatientResponse>(patient);
            return Ok(response);
        }

        [HttpPost]
        [Route("AddPatient")]
        public async Task<IActionResult> AddPatient(AddPatientRequest addPatient)
        {
            var patient = new Patient
            {
                Name = addPatient.Name,
                DateOfBirth = addPatient.DateOfBirth
            };

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Patient added successfully", PatientId = patient.Id });
        }

        [HttpPut]
        [Route("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient(UpdatePatientRequest updatePatient)
        {
            var ExistedPaitent = await _dbContext.FindAsync<Patient>(updatePatient.Id);

            if (ExistedPaitent == null)
            {
                return NotFound(new { Message = "Patient not found" });
            }

            ExistedPaitent.Name = updatePatient.Name;
            ExistedPaitent.DateOfBirth = updatePatient.DateOfBirth;

            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Patient updated successfully" });
        }

        [HttpDelete]
        [Route("DeletePatient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _dbContext.FindAsync<Patient>(id);

            if (patient == null)
            {
                return NotFound(new { Message = "Patient not found" });
            }

            _dbContext.Patients.Remove(patient);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Patient deleted successfully" });
        }
    }
}