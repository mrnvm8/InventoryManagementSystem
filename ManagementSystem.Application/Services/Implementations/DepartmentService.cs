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

internal class DepartmentService(
    IUnitOfWork _unitOfWork,
    ILogger<DepartmentService> _logger) : IDepartmentService
{

    #region Adding the department record to the Database
    public async Task<Result<DepartmentDto>> CreateAsync(DepartmentRequest request, CancellationToken token = default)
    {
        //map to department model 
        var department = request.MapToDepartment();

        //check if the department name does exist, if yes it is duplication
        var departmentNameExist = await _unitOfWork.Departments.DepartmentExist(department, token);
        if (departmentNameExist)
        {
           
            return Result<DepartmentDto>.Failure(new ServicesErrorMessages(request.DepartmentName).Duplicate);
        }

        //add new department record 
        await _unitOfWork.Departments.CreateAsync(department, token);
        var added = await _unitOfWork.SaveChangesAsync(token);
        if (!added.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {added.Message}");
            return Result<DepartmentDto>.Failure(added.Message);
        }

        //get add record from DB
        var dbDepartment = await GetByIdAsync(department.Id, token);

        return Result<DepartmentDto>.Success(dbDepartment.Data, Messages.Added);
    }
    #endregion

    #region Get all department from database
    public async Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync(CancellationToken token = default)
    {
        //get all department
        var departments = await _unitOfWork.Departments.GetAllAsync(token);
        if (!departments.Any())
        {
            return Result<IEnumerable<DepartmentDto>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<DepartmentDto>>
            .Success(departments.Select(dep => dep.MapToDepartmentDto()), string.Empty);
    }
    #endregion

    #region Get record by id
    public async Task<Result<DepartmentDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //get department record by id
        var department = await _unitOfWork.Departments.GetByIdAsync(id, token);
        if (department is null)
        {
            //error it does not exist 
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DepartmentDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        //return the record
        return Result<DepartmentDto>.Success(department.MapToDepartmentDto(), string.Empty);
    }
    #endregion

    #region Updating the department record
    public async Task<Result<DepartmentDto>> UpdateAsync(DepartmentRequest request, Guid id, CancellationToken token = default)
    {

        //get department record by id
        var dbDepartment = await _unitOfWork.Departments.GetByIdAsync(id, token);
        if (dbDepartment is null)
        {
            //error it does not exist 
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<DepartmentDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        //map to department model
        var department = request.MapToDepartment(dbDepartment);

        //check if the department name does exist, if yes it is duplication
        var departmentNameExist = await _unitOfWork.Departments
            .DepartmentExist(department, token);
        if (departmentNameExist)
        {
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                     $"Error : {new ServicesErrorMessages(request.DepartmentName).Duplicate}");
            return Result<DepartmentDto>.Failure(new ServicesErrorMessages(request.DepartmentName).Duplicate);
        }

        //update the department
        await _unitOfWork.Departments.UpdateAsync(department, token);
        var updated = await _unitOfWork.SaveChangesAsync(token);
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {updated.Message}");
            return Result<DepartmentDto>.Failure(updated.Message);
        }

        //get uppdated record
        var updatedDepartment = await GetByIdAsync(department.Id, token);
        return Result<DepartmentDto>.Success(updatedDepartment.Data, Messages.Updated);
    }
    #endregion

    #region Delete record by id
    public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        //get department record by id
        var dbDepartment = await _unitOfWork.Departments.GetByIdAsync(id, token);
        if (dbDepartment is null)
        {
            //error it does not exist 
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        //delete record 
        await _unitOfWork.Departments.DeleteAsync(dbDepartment, token);
        var deleted = await _unitOfWork.SaveChangesAsync(token);
        if (!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(DepartmentService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }

        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion
}
