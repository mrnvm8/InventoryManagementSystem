namespace ManagementSystem.Application.Entities;

public class SystemUserRole
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid RoleId { get; set; }
    public virtual AppUser? User { get; set; }
    public virtual SystemRole? Role { get; set; }
}

