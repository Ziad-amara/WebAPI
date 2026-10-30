namespace WebApplication2.Dtos.AppointmentDtos
{
    public class GetAppointmentsResponse
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
