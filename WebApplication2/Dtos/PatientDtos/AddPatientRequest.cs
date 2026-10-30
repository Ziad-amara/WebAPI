namespace WebApplication2.Dtos.PatientDtos
{
    public class AddPatientRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
