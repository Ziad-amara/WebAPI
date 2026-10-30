using AutoMapper;
using WebApplication2.Dtos.DoctorDtos;
using WebApplication2.Entities;

namespace WebApplication2.Helper
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, GetDoctorResponse>();
        }
    }
}
