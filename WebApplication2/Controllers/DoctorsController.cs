using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;
using WebApplication2.Results;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public DoctorController(ApplicationDbContext context, ILogger<DoctorController> logger, IMapper mapper)
        {
            _dbContext = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            List<Doctor> doctors;
            var totalCounts = 0;
            var message = "Get All Doctors EndPoint";

            if (searchTerm is not null)
            {
                doctors = _dbContext.Set<Doctor>()
                    .Where(d => d.Name.Contains(searchTerm))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                totalCounts = await _dbContext.Doctors.CountAsync(d => d.Name.Contains(searchTerm));
            }
            else
            {
                doctors = _dbContext.Set<Doctor>()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                totalCounts = await _dbContext.Doctors.CountAsync();

            }

            _logger.LogInformation(message, doctors);

            var doctorsResponse = _mapper.Map<List<Doctor>, List<GetDoctorResponse>>(doctors);

            var paginatedResponse = new PaginatedList<GetDoctorResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCounts,
                Totalpage = (int)Math.Ceiling(totalCounts / (double)pageSize),
                Data = doctorsResponse
            };

            return Ok(GenericResult<PaginatedList<GetDoctorResponse>>.Success(paginatedResponse));
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

            var response = _mapper.Map<GetDoctorResponse>(doctor);
            return Ok(response);
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