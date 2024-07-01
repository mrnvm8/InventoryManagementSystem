namespace ManagementSystem.Shared.Requests;

public class TicketRequest
{
    public Guid? Id { get; set; }   
    public Guid DeviceId { get; set; }
    public string TicketTitle { get; set; } = string.Empty;
    public string TicketIssue { get; set; } = string.Empty;
    public DateTime TicketCreatedDate { get; set; }
    public DateTime? ArangedDate { get; init; }
    public DateTime? FixedDate { get; init; }
    public string? TicketSolution { get; init; }
    public string? TicketUpdate { get; init; }
}
