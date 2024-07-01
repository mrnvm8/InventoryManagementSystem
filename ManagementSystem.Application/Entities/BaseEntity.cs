using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Application.Entities;
public class BaseEntity
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
