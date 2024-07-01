namespace ManagementSystem.Shared.Requests;

public class LoanRequest
{
    public required Guid DeviceId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid EmployeeId { get; init; }
    public DateTime AssignedDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public bool IsApproved { get; init; }
}
