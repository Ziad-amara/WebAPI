namespace WebApplication2.Dtos.PatientDtos
{
    public class UpdatePatientRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
