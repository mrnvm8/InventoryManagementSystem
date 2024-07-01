using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;
public class DeviceLoanDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid EmployeeId { get; set; }

    [DataType(DataType.Date),
    Display(Name = "Assigned Date"),
    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime AssignedDate { get; set; } = DateTime.Now;

    [DataType(DataType.Date),
    Display(Name = "Return Date"),
    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ReturnDate { get; set; }

    [Display(Name = "Department Name")]
    public string? DepartmentName { get; set; }

    public bool IsApproved { get; set; } = false;
    public string? EmployeeName { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? DeviceTypeId { get; set; }

}
