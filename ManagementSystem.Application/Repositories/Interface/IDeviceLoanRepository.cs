using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IDeviceLoanRepository : IGenericRepository<DeviceLoan>
{
    Task<DeviceLoan?> GetByDeviceIdAsync(Guid deviceId, CancellationToken token = default);
    Task<IEnumerable<DeviceLoan>> GetDevicesLoanHistoryByIdAsync(Guid deviceId, CancellationToken token = default);
    Task<IEnumerable<DeviceLoan>> GetDevicesByEmployeeIdAsync(Guid employeeId, CancellationToken token = default);
}
