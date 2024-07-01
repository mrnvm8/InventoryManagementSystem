using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;
public interface IDeviceTypeService : IGenericService<DeviceTypeRequest, DeviceTypeDto> { }