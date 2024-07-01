using ManagementSystem.Shared.Enums;

namespace ManagementSystem.Application.Entities;

public class Employee :BaseEntity
{
    public required Guid DepartmentId { get; set; }
    public required string Surname { get; set; }
    public required Gender Gender { get; set; }
    public required string WorkEmail { get; set; }
    public bool CanRegister { get; set; } = false;
    public virtual Department? Department { get; init; }

    public virtual ICollection<AppUser> Users { get; init; }
        = new HashSet<AppUser>();
    public virtual ICollection<DeviceLoan> DevicesLoans { get; set; }
           = new HashSet<DeviceLoan>();
}
