using AutoMapper;
using Azure.Core;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.Repository.Repositories;
using WebApplication2.Results;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services.Repositories
{
    public class PatientService(IGenericRepositroy<Patient> repositroy,IGenericRepositroy<Appointment> appointmentRepository, IMapper mapper) : IpatientServices
    {
        private const int MaxNameLength = 100;
        private const int MaxAgeYears = 120;

        public async Task<GenericResult<GetPatientResponse>> CreateAsync(AddPatientRequest request)
        {
            var validation = Validate(request.Name, request.DateOfBirth);
            
            if (validation.IsFailure)
                return GenericResult<GetPatientResponse>.Failure(validation.Error);

            var patient = mapper.Map<Patient>(request);
            patient.Name = patient.Name.Trim();

            var created = await repositroy.CreateAsync(patient);
            return GenericResult<GetPatientResponse>.Success(mapper.Map<GetPatientResponse>(created));
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var patient = await repositroy.GetByIdAsync(id);
            if (patient is null)
                return Result.Failure("Patient not found");

            var appointments = await appointmentRepository.GetAllAsync();
            if (appointments.Any(a => a.PatientId == id))
                return Result.Failure("Cannot delete a patient who has appointments");

            return await repositroy.DeleteAsync(id)
                ? Result.Success()
                : Result.Failure("Failed to delete patient");
        }

        public async Task<GenericResult<IEnumerable<GetPatientResponse>>> GetAllAsync()
        {
            var patients = await repositroy.GetAllAsync();
            var result = mapper.Map<IEnumerable<GetPatientResponse>>(patients);

            return GenericResult<IEnumerable<GetPatientResponse>>.Success(result);
        }

        public async Task<GenericResult<PaginatedList<GetPatientResponse>>> GetAllAsync(int pageSize, string? searchTerm, int pageNumber = 1)
        {
            var patients = await repositroy.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                patients = patients.Where(p => p.Name.Contains(searchTerm.Trim(), StringComparison.OrdinalIgnoreCase));

            var totalRecords = patients.Count();

            var page = new PaginatedList<GetPatientResponse> { PageNumber = pageNumber < 1 ? 1 : pageNumber };
            if (pageSize > 0)
                page.PageSize = pageSize;

            var items = patients
                .OrderBy(p => p.Id)
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .ToList();

            page.TotalRecords = totalRecords;
            page.Totalpage = (int)Math.Ceiling(totalRecords / (double)page.PageSize);
            page.Data = mapper.Map<IEnumerable<GetPatientResponse>>(items);

            return GenericResult<PaginatedList<GetPatientResponse>>.Success(page);
        }

        public async Task<GenericResult<GetPatientResponse>> GetByIdAsync(int id)
        {
            var patient = await repositroy.GetByIdAsync(id);

            if (patient is null)
                return GenericResult<GetPatientResponse>.Failure("Patient not found");

            return GenericResult<GetPatientResponse>.Success(mapper.Map<GetPatientResponse>(patient));
        }

        public async Task<Result> UpdateAsync(UpdatePatientRequest request)
        {
            var validation = Validate(request.Name, request.DateOfBirth);
            if (validation.IsFailure)
                return validation;

            var patient = await repositroy.GetByIdAsync(request.Id);
            if (patient is null)
                return Result.Failure("Patient not found");

            mapper.Map(request, patient);
            patient.Name = patient.Name.Trim();

            return await repositroy.UpdateAsync(patient)
                ? Result.Success()
                : Result.Failure("Failed to update patient");
        }

        private static Result Validate(string? name, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Patient name is required");

            if (name.Trim().Length > MaxNameLength)
                return Result.Failure($"Patient name cannot exceed {MaxNameLength} characters");

            if (dateOfBirth.Date > DateTime.Today)
                return Result.Failure("Date of birth cannot be in the future");

            if (dateOfBirth.Date < DateTime.Today.AddYears(-MaxAgeYears))
                return Result.Failure($"Date of birth cannot be more than {MaxAgeYears} years ago");

            return Result.Success();
        }
    }
}
