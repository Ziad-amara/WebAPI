using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public DoctorController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _dbContext.Doctors.ToListAsync();
            return Ok(doctors);
        }

        [HttpGet]
        [Route("GetDoctorById/{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });

            }
            return Ok(doctor);
        }

        [HttpPost]
        [Route("AddDoctor")]
        public async Task<IActionResult> AddDoctor(AddDoctorRequest addDoctor)
        {
            var doctor = new Doctor
            {
                Name = addDoctor.Name,
                Specialization = addDoctor.Specialization
            };
            await _dbContext.Doctors.AddAsync(doctor);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Doctor added successfully", DoctorId = doctor.Id });
        }

        [HttpPut]
        [Route("updateDoctor/{id}")]
        public async Task<IActionResult> UpdateDoctor(UpdateDoctorRequest updateDoctor)
        {
            var Doctor = await _dbContext.FindAsync<Doctor>(updateDoctor.Id);

            if (Doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            Doctor.Name = updateDoctor.Name;
            Doctor.Specialization = updateDoctor.Specialization;

            return Ok(new { Message = "Doctor Updated Successfuly" });
        }

        [HttpDelete]
        [Route("DelteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _dbContext.FindAsync<Doctor>(id);

            if (doctor == null)
            {
                return NotFound(new { Message = "Doctor not found" });
            }

            _dbContext.Doctors.Remove(doctor);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Doctor deleted successfully" });

        }
    }

}