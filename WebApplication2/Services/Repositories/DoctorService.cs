using AutoMapper;
using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.Results;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Repositories
{
    public class DoctorService(IGenericRepositroy<Doctor> repositroy, IGenericRepositroy<Appointment> appointmentRepository, IMapper mapper) : IDoctorServices
    {
        private const int MaxLength = 100; 

        public async Task<GenericResult<IEnumerable<GetDoctorResponse>>> GetAllAsync()
        {
            var doctors = await repositroy.GetAllAsync();
            var result = mapper.Map<IEnumerable<GetDoctorResponse>>(doctors);

            return GenericResult<IEnumerable<GetDoctorResponse>>.Success(result);
        }

        public async Task<GenericResult<PaginatedList<GetDoctorResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var doctors = await repositroy.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                doctors = doctors.Where(d => d.Name.Contains(searchTerm.Trim(), StringComparison.OrdinalIgnoreCase));

            var totalRecords = doctors.Count();

            var page = new PaginatedList<GetDoctorResponse> { PageNumber = pageNumber < 1 ? 1 : pageNumber };
            if (pageSize > 0)
                page.PageSize = pageSize;

            var items = doctors
                .OrderBy(d => d.Id)
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToList();

            page.TotalRecords = totalRecords;
            page.Totalpage = (int)Math.Ceiling(totalRecords / (double)page.PageSize);
            page.Data = mapper.Map<IEnumerable<GetDoctorResponse>>(items);

            return GenericResult<PaginatedList<GetDoctorResponse>>.Success(page);
        }

        public async Task<GenericResult<GetDoctorResponse>> GetByIdAsync(int id)
        {
            var doctor = await repositroy.GetByIdAsync(id);

            if (doctor is null)
                return GenericResult<GetDoctorResponse>.Failure("Doctor not found");

            var result = GenericResult<GetDoctorResponse>.Success(mapper.Map<GetDoctorResponse>(doctor));
            return result;
        }

        public async Task<GenericResult<GetDoctorResponse>> CreateAsync(AddDoctorRequest request)
        {
            var validation = Validate(request.Name, request.Specialization);
            
            if (validation.IsFailure)
                return GenericResult<GetDoctorResponse>.Failure(validation.Error);

            var doctor = mapper.Map<Doctor>(request);
            Normalize(doctor);

            if (await IsDuplicateAsync(doctor, excludeId: null))
                return GenericResult<GetDoctorResponse>
                        .Failure("A doctor with the same name and specialization already exists");

            var created = await repositroy.CreateAsync(doctor);

            return GenericResult<GetDoctorResponse>.Success(mapper.Map<GetDoctorResponse>(created));
        }

        public async Task<Result> UpdateAsync(UpdateDoctorRequest request)
        {
            var validation = Validate(request.Name, request.Specialization);
            if (validation.IsFailure)
                return validation;

            var doctor = await repositroy.GetByIdAsync(request.Id);
            if (doctor is null)
                return Result.Failure("Doctor not found");

           
            mapper.Map(request, doctor);
            Normalize(doctor);

            if (await IsDuplicateAsync(doctor, excludeId: doctor.Id))
                return Result.Failure("A doctor with the same name and specialization already exists");

            var updated = await repositroy.UpdateAsync(doctor);

            return updated ? Result.Success() : Result.Failure("Failed to update doctor");
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var doctor = await repositroy.GetByIdAsync(id);
            if (doctor is null)
                return Result.Failure("Doctor not found");

            var appointments = await appointmentRepository.GetAllAsync();
            if (appointments.Any(a => a.DoctorId == id))
                return Result.Failure("Cannot delete a doctor who has appointments");

            var deleted = await repositroy.DeleteAsync(id);

            return deleted ? Result.Success() : Result.Failure("Failed to delete doctor");
        }

        private static Result Validate(string? name, string? specialization)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Doctor name is required");

            if (name.Trim().Length > MaxLength)
                return Result.Failure($"Doctor name cannot exceed {MaxLength} characters");

            if (string.IsNullOrWhiteSpace(specialization))
                return Result.Failure("Specialization is required");

            if (specialization.Trim().Length > MaxLength)
                return Result.Failure($"Specialization cannot exceed {MaxLength} characters");

            return Result.Success();
        }

        private static void Normalize(Doctor doctor)
        {
            doctor.Name = doctor.Name.Trim();
            doctor.Specialization = doctor.Specialization.Trim();
        }

        private async Task<bool> IsDuplicateAsync(Doctor candidate, int? excludeId)
        {
            var doctors = await repositroy.GetAllAsync();

            return doctors.Any(d =>
                d.Id != excludeId &&
                string.Equals(d.Name, candidate.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(d.Specialization, candidate.Specialization, StringComparison.OrdinalIgnoreCase));
        }
    }
}
