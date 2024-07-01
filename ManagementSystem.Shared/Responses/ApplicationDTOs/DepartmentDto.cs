using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public Guid OfficeId { get; set; }
    [Display(Name = "Department Name")]
    public required string DepartmentName { get; set; }

    [Display(Name = "Office Name")]
    public required string DepartmentOfficeName { get; set; }
    public string? Description { get; set; }
}
