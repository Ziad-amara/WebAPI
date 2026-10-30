using AutoMapper;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;

namespace WebApplication1.Helper
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {

            CreateMap<Appointment, GetAppointmentsResponse>();
                 
        }
    }
}
