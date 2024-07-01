using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;
public interface IEmployeeService : IGenericService<EmployeeRequest, EmployeeDto> 
{
    Task<Result<IEnumerable<DeviceDto>>> GetAllLoanDevicesForEmployee(Guid employeeId, CancellationToken token = default);
}