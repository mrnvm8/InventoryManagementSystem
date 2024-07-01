using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;
public class UsersDto
{
    public required Guid userId { get; set; }
    public required Guid EmployeeId { get; set; }
    [Display(Name = "User Name")]
    public required string UserName { get; set; }
    [Display(Name = "Employee Name")]
    public required string EmployeeName { get; set; }
    [Display(Name = "Department Name")]
    public required string DepartmentName { get; set; }
    [Display(Name = "Office Name")]
    public required string OfficeName { get; set;}
}
