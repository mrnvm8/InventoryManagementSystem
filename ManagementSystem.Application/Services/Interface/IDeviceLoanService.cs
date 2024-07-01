using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;

public interface IDeviceLoanService
{
    Task<Result<IEnumerable<DeviceLoanDto>>> GetAllDeviceLoansById(Guid deviceId, CancellationToken token = default);
    Task<Result<DeviceLoanDto>> AssignDevice(LoanRequest request, CancellationToken token = default);
    Task<Result<DeviceLoanDto>> UnassignedDevice(Guid deviceId, LoanRequest request, CancellationToken token = default);
    Task<Result<DeviceLoanDto>> GetLoanByDeviceId(Guid deviceId, CancellationToken token);
}
