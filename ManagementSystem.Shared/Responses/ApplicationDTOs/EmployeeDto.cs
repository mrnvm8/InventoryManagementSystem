using ManagementSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations;
namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class EmployeeDto
{
    public required Guid Id { get; init; }
    public required Guid DepartmentId { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required Gender Gender { get; init; }
    public bool CanRegister { get; set; }

    [Display(Name = "Work Email Address")]
    public required string WorkEmail { get; init; }
    public required string Department { get; init; }

    [Display(Name = "Office Name")]
    public required string OfficeName { get; init; }
    public required string FullName { get; set; }
}
