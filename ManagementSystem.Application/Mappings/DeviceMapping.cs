using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class DeviceMappings
{
    //For create
    public static Device MapToDevice(this DeviceRequest request)
    {
        return new Device
        {
            Id = Guid.NewGuid(),
            Name = request.DeviceName,
            DepartmentId = request.DepartId,
            DeviceTypeId = request.TypeId,
            DeviceSerialNo = request.SerialNo,
            DeviceIMEINo = request.IMEINo,
            Condition = request.Condition,
            PurchasedDate = request.PurchasedDate,
            PurchasedPrice = request.PurchasedPrice,
            Year = request.PurchasedDate.Year,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }

    //For Update
    public static Device MapToDevice(this DeviceRequest request, Device device)
    {
        return new Device
        {
            Id = device.Id,
            Name = request.DeviceName,
            DepartmentId = request.DepartId,
            DeviceTypeId = request.TypeId,
            DeviceSerialNo = string.IsNullOrEmpty(request.SerialNo) ? device.DeviceSerialNo : request.SerialNo,
            DeviceIMEINo = string.IsNullOrEmpty(request.IMEINo) ? device.DeviceIMEINo: request.IMEINo,
            Condition = request.Condition,
            PurchasedDate = request.PurchasedDate,
            PurchasedPrice = request.PurchasedPrice,
            Year = request.PurchasedDate.Year,
            Created = device.Created,
            Updated = DateTime.Now
        };
    }

    //For Dto Response
    public static DeviceDto MapToDeviceDto(this Device device, IEnumerable<DeviceLoan?> loan)
    {
        return new DeviceDto
        {
            Id = device.Id,
            DeviceName = device.Name,
            DepartId = device.DepartmentId,
            TypeId = device.DeviceTypeId,
            Condition = device.Condition,
            PurchasedPrice = device.PurchasedPrice,
            PurchasedDate = device.PurchasedDate,
            SerialNo = device.DeviceSerialNo == null ? "" : device.DeviceSerialNo,
            IMEINo = device.DeviceIMEINo == null ? "" : device.DeviceIMEINo,
            IdentityNumber = device.DeviceSerialNo != null ?
                                   $"S/N: {device.DeviceSerialNo!.ToUpper()}" : $"IMEI: {device.DeviceIMEINo!.ToUpper()}",
            DepartmentName = device.Department?.Name,
            TypeName = device.DeviceType!.Name,
            FullName = loan.SingleOrDefault(x => x!.DeviceId.Equals(device.Id)) is null ? string.Empty :
                        $"{loan.SingleOrDefault(x => x!.DeviceId.Equals(device.Id))?.Employee!.Name} , " +
                        $"{loan.SingleOrDefault(x => x!.DeviceId.Equals(device.Id))?.Employee!.Surname}"
        };
    }
}