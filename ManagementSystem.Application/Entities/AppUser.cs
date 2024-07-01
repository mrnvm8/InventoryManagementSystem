namespace ManagementSystem.Application.Entities;

public class AppUser
{
    public required Guid Id { get; set; }
    public required Guid EmployeeId { get; set; }
    public required string Password { get; set; }
    public required DateTime LastSignIn { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime LastUpdated { get; set; }
    public Employee? Employee { get; init; }
    public virtual ICollection<Ticket> Tickets { get; set; }
          = new HashSet<Ticket>();

}
