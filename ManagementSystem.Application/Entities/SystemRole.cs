namespace ManagementSystem.Application.Entities;

public class SystemRole : BaseEntity 
{
    public virtual ICollection<SystemUserRole> UserRoles { get; set; } 
        = new HashSet<SystemUserRole>();
}
