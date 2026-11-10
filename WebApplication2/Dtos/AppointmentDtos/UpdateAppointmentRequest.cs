using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos.AppointmentDtos
{
    public class UpdateAppointmentRequest
    {
        [Required]
        public int? PatientId { get; set; }

        [Required]
        public int? DoctorId { get; set; }

        [Required]
        public DateTime? AppointmentDate { get; set; }
    }
}
