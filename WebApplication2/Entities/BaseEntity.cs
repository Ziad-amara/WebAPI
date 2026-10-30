namespace WebApplication2.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        // Audit 

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        public DateTime ModifiedBy { get; set; }
    }
}
