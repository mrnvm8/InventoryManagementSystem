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

internal class DeviceLoanService(
    IUnitOfWork _unitOfWork, 
    IHttpContextService _http, 
    IDeviceService _deviceService,
    ILogger<DeviceLoanService> _logger) : IDeviceLoanService
{

    #region Assigning Device to employee
    public async Task<Result<DeviceLoanDto>> AssignDevice(LoanRequest request, CancellationToken token = default)
    {
        //get the user Id
        var userId = _http.GetUserId();
        //get the devie from the 
        //mapping the request to device loan model
        var deviceLoan = request.MapToDeviceLoan(userId);
        //get the device by id , this is for me to know that
        //if the device to assigned to exist on device table
        var device = await _deviceService.GetByIdAsync(deviceLoan.DeviceId, token);
        if (device is null)
        {
            _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
               $"Error device Id : {new ServicesErrorMessages(deviceLoan.DeviceId).IdNotFound}");
            return Result<DeviceLoanDto>.Failure(new ServicesErrorMessages(deviceLoan.DeviceId).IdNotFound);
        }

        await _unitOfWork.DeviceLoans.CreateAsync(deviceLoan, token);
        var assigned = await _unitOfWork.SaveChangesAsync(token);
        if (!assigned.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error  : {assigned.Message}");
            return Result<DeviceLoanDto>.Failure(assigned.Message);
        }

        var loanedDevice = await GetLoanByDeviceId(deviceLoan.Id, token);
        return Result<DeviceLoanDto>.Success(loanedDevice.Data, Messages.Added);
    }
    #endregion

    #region Get all loans of specific device by deviceId for history of the device
    public async Task<Result<IEnumerable<DeviceLoanDto>>> GetAllDeviceLoansById(Guid deviceId, CancellationToken token = default)
    {
        //get all the device history 
        var deviceLoan = await _unitOfWork.DeviceLoans.GetDevicesLoanHistoryByIdAsync(deviceId);
        if (!deviceLoan.Any())
        {
            return Result<IEnumerable<DeviceLoanDto>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<DeviceLoanDto>>.Success(deviceLoan.Select(loan => loan.MapToDeviceLoanDto()) , string.Empty);
    }
    #endregion

    #region Get a loan by a specific Device Id
    public async Task<Result<DeviceLoanDto>> GetLoanByDeviceId(Guid deviceId, CancellationToken token)
    {
        var deviceLoan = await _unitOfWork.DeviceLoans.GetByDeviceIdAsync(deviceId, token);
        if (deviceLoan is null)
        {
            _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
               $"Error device Id : {new ServicesErrorMessages(deviceId).IdNotFound}");
            return Result<DeviceLoanDto>.Failure(new ServicesErrorMessages(deviceId).IdNotFound);
        }
        return Result<DeviceLoanDto>.Success(deviceLoan.MapToDeviceLoanDto(),string.Empty);
    }
    #endregion

    #region Unassigning Device to employee
    public async Task<Result<DeviceLoanDto>> UnassignedDevice(Guid deviceId, LoanRequest request, CancellationToken token = default)
    {
       //get the user Id
        var userId = _http.GetUserId();
        //get the device by id , this is for me to know that
        //if the device to assigned to exist on device table
        var deviceLoan = await _unitOfWork.DeviceLoans.GetByDeviceIdAsync(deviceId, token);
        if (deviceLoan is null)
        {
            _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
               $"Error device Id : {new ServicesErrorMessages(deviceId).IdNotFound}");
            return Result<DeviceLoanDto>.Failure(new ServicesErrorMessages(deviceId).IdNotFound);
        }

        //mapping the request to device loan model
        //map device loan request
        var loan = request.MapToDeviceLoanForUnassigned(deviceId, userId, deviceLoan);

        //checking if assigned date and return date , if yes delete the information
        //we treat this information as a user mistake
        if (AssignedAndReturnDateSame(loan))
        {
            await _unitOfWork.DeviceLoans.DeleteAsync(deviceLoan, token);
            var deleted = await _unitOfWork.SaveChangesAsync(token);

            if (!deleted.IsSuccess)
            {
                //error fail to add
                _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                  $"Error  : {deleted.Message}");
                return Result<DeviceLoanDto>.Failure(deleted.Message);
            };
            return Result<DeviceLoanDto>.Success(new DeviceLoanDto(), Messages.Deleted);
        }

        //update the information if the assigned date  is different
        await _unitOfWork.DeviceLoans.UpdateAsync(loan, token);
        var unAssigned = await _unitOfWork.SaveChangesAsync(token);
        if (!unAssigned.IsSuccess)
        {
            //error fail to add
            _logger.LogError($"Service : {nameof(DeviceLoanService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error  : {unAssigned.Message}");
            return Result<DeviceLoanDto>.Failure(unAssigned.Message);
        };

        var loanedDevice = await GetLoanByDeviceId(loan.DeviceId, token);
        return Result<DeviceLoanDto>.Success(loanedDevice.Data, Messages.Updated);
    }
    #endregion

    #region checking if the dates are the same 
    private bool AssignedAndReturnDateSame(DeviceLoan loan)
    {
        if (loan!.AssignedDate.ToString("yyyy-MM-dd")
            .Equals(loan.ReturnDate.ToString("yyyy-MM-dd")))
        {
            return true;
        }
        return false;
    }
    #endregion
}
