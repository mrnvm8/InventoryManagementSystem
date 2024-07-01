using ManagementSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementSystem.Shared.Requests;

public class DeviceRequest
{
    [Display(Name = "Department")]
    public Guid DepartId { get; init; }

    [Display(Name = "Device Type")]
    public Guid TypeId { get; init; }

    [Required, StringLength(20, MinimumLength = 3),
      DataType(DataType.Text),
      Display(Name = "Device Name")]
    public required string DeviceName { get; init; }
    public string? SerialNo { get; init; }
    public string? IMEINo { get; init; }

    [Range(typeof(decimal), "0", "100000", ErrorMessage = "Price value must be between 0 to 99999.99"),
    Display(Name = "Purchased Price"),
    Column(TypeName = "decimal(18, 2)")]
    public required decimal PurchasedPrice { get; init; }

    [DataType(DataType.Date),
     Display(Name = "Purchased Date"),
     DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public required DateTime PurchasedDate { get; init; }

    [Display(Name = "Device Condition")]
    public required Condition Condition { get; init; }
}
