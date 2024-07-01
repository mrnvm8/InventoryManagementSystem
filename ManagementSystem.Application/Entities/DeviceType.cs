namespace ManagementSystem.Application.Entities;

public class DeviceType : BaseEntity
{
    public required string Description { get; set; }
    public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();
}
