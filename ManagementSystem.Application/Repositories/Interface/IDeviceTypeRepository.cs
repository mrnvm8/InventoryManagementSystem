using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IDeviceTypeRepository : IGenericRepository<DeviceType>
{
    Task<bool> DeviceTypeNameExist(DeviceType type, CancellationToken token);
}
