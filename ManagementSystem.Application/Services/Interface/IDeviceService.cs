using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;

public interface IDeviceService : IGenericService<DeviceRequest, DeviceDto>
{
    Task<Result<IEnumerable<DeviceDto>>> GetDevicesByDepartmentAndDeviceType(
        Guid departId,
        Guid TypeId,
        CancellationToken token = default);
    Task<Result<IEnumerable<DeviceSummary>>> Summary(CancellationToken token = default);
}

