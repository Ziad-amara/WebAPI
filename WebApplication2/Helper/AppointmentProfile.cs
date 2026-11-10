using AutoMapper;
using WebApplication2.Dtos.AppointmentDtos;
using WebApplication2.Entities;

namespace WebApplication2.Helper
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {

            CreateMap<Appointment, GetAppointmentsResponse>();
            CreateMap<AddAppointmentRequest, Appointment>();
            CreateMap<UpdateAppointmentRequest, Appointment>();

        }
    }
}
