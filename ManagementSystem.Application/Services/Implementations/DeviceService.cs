using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Helpers;
using ManagementSystem.Application.Helpers.Errors.Services;
using ManagementSystem.Application.Mappings;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Application.Services.Interface;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Application.Services.Implementations;

internal class DeviceService(
    IUnitOfWork _unitOfWork,
    ILogger<DeviceService> _logger,
    IHttpContextService _http) : IDeviceService
{
    #region Adding device record to DB
    public async Task<Result<DeviceDto>> CreateAsync(DeviceRequest request, CancellationToken token = default)
    {
        //create a new device record (Mapping)
        var device = request.MapToDevice();
        //check if the  Device Serial Number/IMIE Number does bot exist
        var deviceExist = await _unitOfWork.Devices.DeviceExistAsync(device, token);
        if (deviceExist)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {ServicesErrorMessages.DeviceExist}");
            return Result<DeviceDto>.Failure(ServicesErrorMessages.DeviceExist);
        }

        await _unitOfWork.Devices.CreateAsync(device, token);
        var added = await _unitOfWork.SaveChangesAsync(token);
        if (!added.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {added.Message}");
            return Result<DeviceDto>.Failure(added.Message);
        }
        
        var dbDevice = await GetByIdAsync(device.Id, token);
        return Result<DeviceDto>.Success(dbDevice.Data, Messages.Added);
    }
    #endregion

    #region Get all device records
    public async Task<Result<IEnumerable<DeviceDto>>> GetAllAsync(CancellationToken token = default)
    {
        //get all records
        var devices = await _unitOfWork.Devices.GetAllAsync(token);
        var loan = await _unitOfWork.DeviceLoans.GetAllAsync(token);
        var Role = _http.GetUserRole()!;
        var UserDepartmentId = _http.GetDepartmentIdentifier();

        if (!devices.Any())
        {
            return Result<IEnumerable<DeviceDto>>.Failure(Messages.NoData);
        }

        if (!Role.Contains(AuthConstants.AdminRole))
        {
            return Result<IEnumerable<DeviceDto>>
             .Success(devices.Where(d => d.DepartmentId.Equals(UserDepartmentId))
             .Select(device => device.MapToDeviceDto(loan)), string.Empty);
        }
        return Result<IEnumerable<DeviceDto>>
            .Success(devices.Select(device => device.MapToDeviceDto(loan)), string.Empty);
    }
    #endregion

    #region Get device record by record Id
    public async Task<Result<DeviceDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //get record by id
        var device = await _unitOfWork.Devices.GetByIdAsync(id, token);
        if (device is null)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
           $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DeviceDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        return Result<DeviceDto>.Success(device.MapToDeviceDto(Enumerable.Empty<DeviceLoan>()), string.Empty);
    }
    #endregion

    #region Selecting device by department and device Type
    public async Task<Result<IEnumerable<DeviceDto>>> GetDevicesByDepartmentAndDeviceType(Guid departId, Guid TypeId, CancellationToken token = default)
    {
        //get the devices with Department Id and TypeId
        var devices = await _unitOfWork.Devices.DevicesByDepamentAndDeviceType(departId, TypeId, token);
        //you have used the devices list above but you would get all Loan devices Assigned and unassigned, BAD.
        var loan = await _unitOfWork.DeviceLoans.GetAllAsync();
        return Result<IEnumerable<DeviceDto>>.Success(devices.Select(d => d.MapToDeviceDto(loan)), string.Empty);
    }
    #endregion

    #region Summary of devices
    public async Task<Result<IEnumerable<DeviceSummary>>> Summary(CancellationToken token = default)
    {
        var Role = _http.GetUserRole()!;
        var userDepartmentId = _http.GetDepartmentIdentifier();
        var _deviceSummary = Enumerable.Empty<DeviceSummary>();

        //get summary
        if (Role.Contains(AuthConstants.AdminRole))
        {
            _deviceSummary = await _unitOfWork.Devices.DeviceSummary(Guid.Empty, token);
        }
        else
        {
            _deviceSummary = await _unitOfWork.Devices.DeviceSummary(userDepartmentId, token);
        }

        if (!_deviceSummary.Any())
        {
            return Result<IEnumerable<DeviceSummary>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<DeviceSummary>>.Success(_deviceSummary, string.Empty);
    }
    #endregion

    #region Updating device record from the DB
    public async Task<Result<DeviceDto>> UpdateAsync(DeviceRequest request, Guid id, CancellationToken token = default)
    {
        //get record by id
        var dbDevice = await _unitOfWork.Devices.GetByIdAsync(id, token);
        if (dbDevice is null)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
           $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DeviceDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        //mapping to device
        var device = request.MapToDevice(dbDevice);
        //check if the serial/IMIE number do not exist on DB
        var devieExist = await _unitOfWork.Devices.DeviceExistAsync(device, token);
        if (devieExist)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {ServicesErrorMessages.DeviceExist}");
            return Result<DeviceDto>.Failure(ServicesErrorMessages.DeviceExist);
        }
        await _unitOfWork.Devices.UpdateAsync(device, token);
        var updated = await _unitOfWork.SaveChangesAsync();
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                 $"Error : {updated.Message}");
            return Result<DeviceDto>.Failure(updated.Message);
        }

        var updatedDevice = await GetByIdAsync(device.Id, token);
        return Result<DeviceDto>.Success(updatedDevice.Data, Messages.Updated);
    }
    #endregion

    #region deleting record by id
    public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        //get record by id
        var device = await _unitOfWork.Devices.GetByIdAsync(id, token);
        if (device is null)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
           $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        await _unitOfWork.Devices.DeleteAsync(device);
        var deleted = await _unitOfWork.SaveChangesAsync(token);
        if(!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }
        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion
}
