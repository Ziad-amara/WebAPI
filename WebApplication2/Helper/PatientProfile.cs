using AutoMapper;
using WebApplication2.Dtos.PatientDtos;
using WebApplication2.Entities;

namespace WebApplication2.Helper
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, GetPatientResponse>();
            CreateMap<AddPatientRequest, Patient>();
            CreateMap<UpdatePatientRequest, Patient>();
        }
    }
}
