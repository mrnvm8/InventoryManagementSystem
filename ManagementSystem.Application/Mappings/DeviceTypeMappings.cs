using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class DeviceTypeMappings
{
    //For create
    public static DeviceType MapToDeviceType(this DeviceTypeRequest request)
    {
        return new DeviceType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }

    //For Update
    public static DeviceType MapToDeviceType(this DeviceTypeRequest request, DeviceType type)
    {
        return new DeviceType
        {
            Id = type.Id,
            Name = request.Name,
            Description = request.Description,
            Created = type.Created,
            Updated = DateTime.Now
        };
    }

    //For Dto Response
    public static DeviceTypeDto MapToDeviceTypeDto(this DeviceType deviceType)
    {
        return new DeviceTypeDto
        {
            Id = deviceType.Id,
            Name = deviceType.Name,
            Description = deviceType.Description,
        };
    }
}