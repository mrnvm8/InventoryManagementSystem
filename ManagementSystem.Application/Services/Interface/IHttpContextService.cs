namespace ManagementSystem.Application.Services.Interface;

public interface IHttpContextService
{
    public Guid GetUserId();
    public string? GetUserRole();
    public string? GetUserName();
    public Guid GetEmployeeIdentifier();
    public Guid GetDepartmentIdentifier();
}
