using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class OfficeDto
{
    public required Guid Id { get; set; }
    [Display(Name = "Office Name")]
    public required string Name { get; set; }
    public required string Location { get; set; }
}
