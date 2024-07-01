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

internal class DeviceTypeService(
    IUnitOfWork _unitOfWork,
    ILogger<DeviceTypeService> _logger) : IDeviceTypeService
{
    #region Adding device type to the database
    public async Task<Result<DeviceTypeDto>> CreateAsync(DeviceTypeRequest request, CancellationToken token = default)
    {
        //map to DeviceType model
        var deviceType = request.MapToDeviceType();
        //check if device type already exist
        var deviceTypeExist = await _unitOfWork.DeviceType.DeviceTypeNameExist(deviceType, token);
        if (deviceTypeExist)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(request.Name).Duplicate}");
            return Result<DeviceTypeDto>.Failure(new ServicesErrorMessages(request.Name).Duplicate);
        }

        //add office information
        await _unitOfWork.DeviceType.CreateAsync(deviceType, token);
        var added = await _unitOfWork.SaveChangesAsync(token);
        if (!added.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {added.Message}");
            return Result<DeviceTypeDto>.Failure(added.Message);
        }
        //get new addded device
        var dbDeviceType = await GetByIdAsync(deviceType.Id);
        return Result<DeviceTypeDto>.Success(dbDeviceType.Data, Messages.Added);
    }
    #endregion

    #region Get All device type record
    public async Task<Result<IEnumerable<DeviceTypeDto>>> GetAllAsync(CancellationToken token = default)
    {
        var deviceTypes = await _unitOfWork.DeviceType.GetAllAsync(token);
        if (!deviceTypes.Any())
        {
            return Result<IEnumerable<DeviceTypeDto>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<DeviceTypeDto>>
            .Success(deviceTypes.Select(type => type.MapToDeviceTypeDto()), string.Empty);
    }
    #endregion

    #region Get device type by Id
    public async Task<Result<DeviceTypeDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //get record by id
        var deviceType = await _unitOfWork.DeviceType.GetByIdAsync(id, token);
        //Id does not exist
        if (deviceType is null)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DeviceTypeDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        return Result<DeviceTypeDto>.Success(deviceType.MapToDeviceTypeDto(), string.Empty);
    }
    #endregion

    #region Updating device type record from database
    public async Task<Result<DeviceTypeDto>> UpdateAsync(DeviceTypeRequest request, Guid id, CancellationToken token = default)
    {

        //get devicetype record by id
        var dbDeviceType = await _unitOfWork.DeviceType.GetByIdAsync(id, token);
        //Id does not exist
        if (dbDeviceType is null)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DeviceTypeDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        //map to office model
        var deviceType = request.MapToDeviceType(dbDeviceType);
        //check if device type already exist
        var deviceTypeExist = await _unitOfWork.DeviceType.DeviceTypeNameExist(deviceType, token);
        if (deviceTypeExist)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(request.Name).Duplicate}");
            return Result<DeviceTypeDto>.Failure(new ServicesErrorMessages(request.Name).Duplicate);
        }
       
        //update information
        await _unitOfWork.DeviceType.UpdateAsync(deviceType, token);
        var updated = await _unitOfWork.SaveChangesAsync();
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {updated.Message}");
            return Result<DeviceTypeDto>.Failure(updated.Message);
        }
        //get updated record
        var updatedOffice = await GetByIdAsync(id, token);

        return Result<DeviceTypeDto>.Success(updatedOffice.Data, Messages.Updated);
    }
    #endregion

    #region Deleting device type Record from database
    public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        //get devicetype record by id
        var dbDeviceType = await _unitOfWork.DeviceType.GetByIdAsync(id, token);
        //Id does not exist
        if (dbDeviceType is null)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        await _unitOfWork.DeviceType.DeleteAsync(dbDeviceType, token);
        var deleted = await _unitOfWork.SaveChangesAsync();
        if (!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceTypeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }

        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion
}
