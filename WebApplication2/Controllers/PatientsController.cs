using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public PatientController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _dbContext.Patients.ToListAsync();
            return Ok(patients);
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
            return Ok(patient);
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