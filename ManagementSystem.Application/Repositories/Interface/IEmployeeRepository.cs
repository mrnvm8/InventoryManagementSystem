using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetEmailByEmailAddressAsync(string email, CancellationToken token = default);
    Task<bool> ExistingAsync(Employee employee, CancellationToken token = default);
}