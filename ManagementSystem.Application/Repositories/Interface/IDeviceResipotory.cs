using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;
public interface IDeviceResipotory : IGenericRepository<Device>
{
    Task<List<DeviceSummary>> DeviceSummary(Guid departmentId, CancellationToken token);
    Task<IEnumerable<Device>> DevicesByDepamentAndDeviceType(Guid departmentId, Guid TypeId, CancellationToken token);
    Task<bool> DeviceExistAsync(Device device, CancellationToken token);
}
