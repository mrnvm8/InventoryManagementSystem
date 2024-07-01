using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Responses.ApplicationDTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid DeviceTypeId { get; set; }

    [Required, Display(Name = "Ticket Title")]
    public string? TicketTitle { get; set; }

    [Required, Display(Name = "Device Issue")]
    public string? TicketIssue { get; set; }

    [Required, DataType(DataType.Date),
    Display(Name = "Created Date"),
    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime TicketCreatedDate { get; set; }

    [DataType(DataType.Date),
    Display(Name = "Aranged Date"),
    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? ArangedDate { get; set; }

    [DataType(DataType.Date),
     Display(Name = "Fixed Date"),
     DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? FixedDate { get; set; }

    [Display(Name = "Ticket Solution")]
    public string? TicketSolution { get; set; }
    public string? TicketUpdate { get; set; }

    [Display(Name = "Device Name")]
    public string? DeviceName { get; set; }
    [Display(Name = "Device Type")]
    public string? DeviceType { get; set; }
    public string? Department { get; set; }
    public bool IssueSolved { get; set; } = false;
    public bool Updated { get; set; } = false;
}
