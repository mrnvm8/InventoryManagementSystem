using ManagementSystem.Shared.Enums;
using ManagementSystem.Shared.Helpers.CustomisedAttribute;
using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Requests;

public class EmployeeRequest
{
    public Guid? Id { get; set; }
    public required Guid DepartmentId { get; init; }

    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "Capitalization is needed and Numbers are not allow.")]
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public required string Name { get; init; }

    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "Capitalization is needed and Numbers are not allow.")]
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public required string Surname { get; init; }
    public required Gender Gender { get; init; }
    public bool CanRegister { get; init; } = false;

    [Required(ErrorMessage = "Your Work email is required"),
    StringLength(60, MinimumLength = 3),
    EmailAddress,
    AllowedEmailDomain("axiumeducation.org")]
    public required string WorkEmail { get; init; }
}
