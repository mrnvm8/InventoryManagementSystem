using ManagementSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class DeviceDto
{
    public required Guid Id { get; set; }
    public required Guid DepartId { get; set; }
    public required Guid TypeId { get; set; }

    [Required, StringLength(20, MinimumLength = 3),
    DataType(DataType.Text),
    Display(Name = "Device Brand Name")]
    public required string DeviceName { get; set; }
    public string? SerialNo { get; set; }
    public string? IMEINo { get; set; }

    public decimal PurchasedPrice { get; set; }

    [DataType(DataType.Date),
    Display(Name = "Purchased Date"),
    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public required DateTime PurchasedDate { get; set; }
    public required Condition Condition { get; set; }
    [Display(Name = "Department Name")]
    public string? DepartmentName { get; set; }

    [Display(Name = "Device Type Name")]
    public required string TypeName { get; set; }

    [Display(Name = "Identity Number")]
    public required string IdentityNumber { get; set; }
    public string? FullName { get; set; }
}
