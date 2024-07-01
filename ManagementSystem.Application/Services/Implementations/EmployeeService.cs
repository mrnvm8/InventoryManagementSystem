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

internal class EmployeeService(
    IUnitOfWork _unitOfWork,
    ILogger<EmployeeService> _logger) : IEmployeeService
{

    #region Creating Employee from Database
    public async Task<Result<EmployeeDto>> CreateAsync(EmployeeRequest request, CancellationToken token = default)
    {
        //Map create request to employee
        var employee = request.MapToEmployee();

        //Checking if the employee exist on the Db or not
        //it will return true if they exist or otherwise
        var emailExist = await _unitOfWork.Employees.ExistingAsync(employee, token);
        if (emailExist)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {ServicesErrorMessages.EmailExist}");
            return Result<EmployeeDto>.Failure(ServicesErrorMessages.EmailExist);
        }
        
     

        //Create the employee
        await _unitOfWork.Employees.CreateAsync(employee, token);
        //check if creation was suceessful
        var createResult = await _unitOfWork.SaveChangesAsync(token);
        if (!createResult.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {createResult.Message}");
            return Result<EmployeeDto>.Failure(createResult.Message);
        }

        //get created employee and return
        var dbEmploee = await GetByIdAsync(employee.Id);
        //else return the response
        return Result<EmployeeDto>.Success(dbEmploee.Data, Messages.Added);
    }
    #endregion

    #region Getting Employee By Id from Database
    public async Task<Result<EmployeeDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //get the data from database using the Id
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, token);
        //if the employee is null
        if (employee is null)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(nameof(Employee)).IdNotFound}");
            return Result<EmployeeDto>.Failure(new ServicesErrorMessages(nameof(Employee)).IdNotFound);
        }
        //return DTo Employee
        return Result<EmployeeDto>.Success(employee.MapToEmployeeDto(), string.Empty);
    }
    #endregion

    #region Get All Employees from Database
    public async Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken token = default)
    {
        //Getting all Employees from DB
        var employees = await _unitOfWork.Employees.GetAllAsync(token);
        if (!employees.Any())
        {
            return Result<IEnumerable<EmployeeDto>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<EmployeeDto>>
            .Success(employees.Select(employees => employees.MapToEmployeeDto()), string.Empty);
    }
    #endregion

    #region Update Employee from Database
    public async Task<Result<EmployeeDto>> UpdateAsync(EmployeeRequest request, Guid id, CancellationToken token = default)
    {
        
        //get the data from database using the Id
        var dbEmployee = await _unitOfWork.Employees.GetByIdAsync(id, token);
        //if the employee is null
        if (dbEmployee is null)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(request.WorkEmail).IdNotFound}");
            return Result<EmployeeDto>.Failure(new ServicesErrorMessages(request.WorkEmail).IdNotFound);
        }

        //map the request 
        var employee = request.MapToEmployee(dbEmployee);

        //check if the email you are updating is not a duplicate
        var emailExist = await _unitOfWork.Employees.ExistingAsync(employee, token);
        if (emailExist)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {ServicesErrorMessages.EmailExist}");
            return Result<EmployeeDto>.Failure(ServicesErrorMessages.EmailExist);
        }

        //update employee
        await _unitOfWork.Employees.UpdateAsync(employee, token);
        var updated = await _unitOfWork.SaveChangesAsync(token);
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {updated.Message}");
            return Result<EmployeeDto>.Failure(updated.Message);
        }

        var updatedEmployee = await GetByIdAsync(employee.Id);
        return Result<EmployeeDto>.Success(updatedEmployee.Data, Messages.Updated);
    }
    #endregion

    #region Delete/Removinf Employee from Database
    public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        //get the data from database using the Id
        var dbEmployee = await _unitOfWork.Employees.GetByIdAsync(id, token);
        //if the employee is null
        if (dbEmployee is null)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(nameof(Employee)).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(nameof(Employee)).IdNotFound);
        }

        //Delete the employee
        await _unitOfWork.Employees.DeleteAsync(dbEmployee);
        var deleted = await _unitOfWork.SaveChangesAsync(token);
        if (!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(EmployeeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }
        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion

    #region Get all loan devices to an employee by employeeId
    public async Task<Result<IEnumerable<DeviceDto>>> GetAllLoanDevicesForEmployee(Guid employeeId, CancellationToken token = default)
    {
        var device = Enumerable.Empty <Device>();
        var deviceLoan = await _unitOfWork.DeviceLoans.GetDevicesByEmployeeIdAsync(employeeId);
        if (!deviceLoan.Any())
            return Result<IEnumerable<DeviceDto>>.Failure(Messages.NoData);

        foreach (var item in deviceLoan)
        {
            device = device.Append(item.Device!);
        }

        return Result<IEnumerable<DeviceDto>>
            .Success(device.Select(d => d.MapToDeviceDto(deviceLoan)), string.Empty);
    }
    #endregion


}
