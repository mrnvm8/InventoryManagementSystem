using ManagementSystem.Shared.Enums;

namespace ManagementSystem.Application.Entities;

public class Device : BaseEntity
{
    public required Guid DepartmentId { get; init; }
    public required Guid DeviceTypeId { get; init; }
    public string? DeviceSerialNo { get; init; }
    public string? DeviceIMEINo { get; init; }
    public required Condition Condition { get; init; }
    public required decimal PurchasedPrice { get; init; }
    public required DateTime PurchasedDate { get; init; }
    public required int Year { get; init; }
    public virtual DeviceType? DeviceType { get; set; }
    public virtual Department? Department { get; set; }
    public virtual ICollection<DeviceLoan> DevicesLoans { get; set; }
            = new HashSet<DeviceLoan>();
    public virtual ICollection<Ticket> Tickets { get; set; }
        = new HashSet<Ticket>();
}
