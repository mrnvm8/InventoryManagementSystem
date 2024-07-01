namespace ManagementSystem.Application.Entities
{
    public class Office : BaseEntity
    {
        public required string Location { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
            = new HashSet<Department>();
    }
}
