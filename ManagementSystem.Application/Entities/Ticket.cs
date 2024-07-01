namespace ManagementSystem.Application.Entities;

public class Ticket
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid DeviceId { get; init; }
    public string TicketTitle { get; init; } = string.Empty;
    public string TicketIssue { get; init; } = string.Empty;
    public DateTime TicketCreatedDate { get; init; }
    public DateTime? ArangedDate { get; init; }
    public DateTime? FixedDate { get; init; }
    public string? TicketSolution { get; init; }
    public string? TicketUpdate { get; init; }
    public bool IssueSolved { get; init; } = false;
    public bool Updated { get; init; } = false;
    public virtual AppUser? Users { get; set; }
    public virtual Device? Devices { get; set; }

}