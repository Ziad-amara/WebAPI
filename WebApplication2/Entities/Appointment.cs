using System.Numerics;

namespace WebApplication2.Entities
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }

        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
