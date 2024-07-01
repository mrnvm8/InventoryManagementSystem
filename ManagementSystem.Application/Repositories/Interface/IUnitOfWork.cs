using ManagementSystem.Shared.Responses;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IUnitOfWork
{
    IOfficeRepository Offices { get; }
    IDepartmentRepository Departments { get; }
    IEmployeeRepository Employees { get; }
    IDeviceTypeRepository DeviceType { get; }
    IDeviceResipotory Devices { get; }
    IDeviceLoanRepository DeviceLoans { get; }
    ITicketRepository Tickets { get; }
    Task<Result<int>> SaveChangesAsync(CancellationToken token = default);
}
