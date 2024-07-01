using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Requests;
public class DepartmentRequest 
{
    [Display(Name = "Office Name")]
    public required Guid OfficeId { get; init; }

    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "Capitalization is needed and Numbers are not allow.")]
    [Required, Display(Name = "Department Name")]
    [StringLength(30, MinimumLength = 3)]
    public required string DepartmentName { get; init; }
    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "Capitalization is needed and Numbers are not allow.")]
    [Required]
    [StringLength(250, MinimumLength = 3)]
    public required string Description { get; init; } 
}
