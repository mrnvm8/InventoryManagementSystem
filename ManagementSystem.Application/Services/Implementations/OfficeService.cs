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

internal class OfficeService(
    IUnitOfWork _unitOfWork,
    ILogger<OfficeService> _logger) : IOfficeService
{

    #region Adding office to the database
    public async Task<Result<OfficeDto>> CreateAsync(OfficeRequest request, CancellationToken token = default)
    {

        //map to office model
        var office = request.MapToOffice();

        //Check if the request name is same as any on the database
        var existed = await _unitOfWork.Offices.OfficeNameExist(office, token);
        //if true return Error message duplicate
        if (existed)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                $"Error : {new ServicesErrorMessages(request.Name).Duplicate}");
            return Result<OfficeDto>.Failure(new ServicesErrorMessages(request.Name).Duplicate);
        }

        //add new office record 
        await _unitOfWork.Offices.CreateAsync(office, token);
        var added = await _unitOfWork.SaveChangesAsync(token);
        if (!added.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {added.Message}");
            return Result<OfficeDto>.Failure(added.Message);
        }

        var dbOffice = await GetByIdAsync(office.Id);
        return Result<OfficeDto>.Success(dbOffice.Data, Messages.Added);
    }
    #endregion

    #region Get All office record
    public async Task<Result<IEnumerable<OfficeDto>>> GetAllAsync(CancellationToken token = default)
    {
        var offices = await _unitOfWork.Offices.GetAllAsync(token);
        if (!offices.Any())
        {
            return Result<IEnumerable<OfficeDto>>.Failure(Messages.NoData);
        }
        return Result<IEnumerable<OfficeDto>>
            .Success(offices.Select(office => office.MapToOfficeDto()), string.Empty);
    }
    #endregion

    #region Get office by Id
    public async Task<Result<OfficeDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //get office record by id
        var dbOffice = await _unitOfWork.Offices.GetByIdAsync(id, token);
        //Id does not exist
        if (dbOffice is null)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<OfficeDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        return Result<OfficeDto>.Success(dbOffice.MapToOfficeDto(), string.Empty);
    }
    #endregion

    #region Updating Office record from database
    public async Task<Result<OfficeDto>> UpdateAsync(OfficeRequest request, Guid id, CancellationToken token = default)
    {
        //get office record by id
        var dbOffice = await _unitOfWork.Offices.GetByIdAsync(id, token);
        //Id does not exist
        if (dbOffice is null)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<OfficeDto>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }

        //map request to office model
        var office = request.MapToOffice(dbOffice);

        //check if the updating name do not create duplicate  from Database
        var existed = await _unitOfWork.Offices.OfficeNameExist(office, token);
        //if true return Error message duplicate
        if (existed)
        {
            //if (request.Name.Equals())
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
                $"Error : {new ServicesErrorMessages(request.Name).Duplicate}");
            return Result<OfficeDto>.Failure(new ServicesErrorMessages(request.Name).Duplicate);
        }

        //update database record
        await _unitOfWork.Offices.UpdateAsync(office, token);
        var updated = await _unitOfWork.SaveChangesAsync();
        if (!updated.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {updated.Message}");
            return Result<OfficeDto>.Failure(updated.Message);
        }

        //get updated record
        var updatedOffice = await GetByIdAsync(office.Id, token);

        return Result<OfficeDto>.Success(updatedOffice.Data, Messages.Updated);
    }
    #endregion

    #region Deleting office Record from database
    public async Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        //get office record by id
        var dbOffice = await _unitOfWork.Offices.GetByIdAsync(id, token);
        //Id does not exist
        if (dbOffice is null)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
              $"Error : {new ServicesErrorMessages(id).IdNotFound}");
            return Result<bool>.Failure(new ServicesErrorMessages(id).IdNotFound);
        }
        await _unitOfWork.Offices.DeleteAsync(dbOffice, token);
        var deleted = await _unitOfWork.SaveChangesAsync();
        if (!deleted.IsSuccess)
        {
            _logger.LogError($"Service : {nameof(OfficeService)}, Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
             $"Error : {deleted.Message}");
            return Result<bool>.Failure(deleted.Message);
        }
        return Result<bool>.Success(deleted.IsSuccess, Messages.Deleted);
    }
    #endregion
}
