namespace WebApplication2.Entities
{
    public class Doctor : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
