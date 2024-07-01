using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class DeviceTypeDto
{
    public required Guid Id { get; set; }
    [Display(Name = "Device Type Name")]
    public required string Name { get; set; }
    public required string Description { get; set; }
}
