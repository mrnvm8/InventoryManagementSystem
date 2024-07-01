using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Requests;

public class OfficeRequest
{
    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "Capitalization is needed and Numbers are not allow.")]
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public required string Name { get; init; }
    public required string Location { get; init; }
}
