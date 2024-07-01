namespace ManagementSystem.Application.Entities
{
    public class Department: BaseEntity
    {
        public required Guid OfficeId { get; set; }
        public required string Description { get; set; }
        public virtual Office? Offices { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();
    }
}
