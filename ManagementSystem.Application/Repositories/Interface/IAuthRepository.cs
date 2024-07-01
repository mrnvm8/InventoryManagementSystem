using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IAuthRepository
{
    Task<IEnumerable<AppUser>> GetAllUsersAsync(CancellationToken token = default);
    Task<AppUser?> CreateAsync(AppUser user, CancellationToken token = default);
    Task<AppUser?> GetUserByEmployeeId(Guid employeeId, CancellationToken token = default);
    Task<AppUser?> GetUserById(Guid UserId, CancellationToken token = default);
    Task<SystemUserRole?> UserRole(Guid userId, CancellationToken token = default);
    Task<SystemRole?> SystemRole(Guid roleId, CancellationToken token = default);
    string GenerateToken(AppUser user, string role);
    Task<bool> DeleteUser(AppUser user, CancellationToken token = default);
    Task<bool> ChangePassword(AppUser user, CancellationToken token = default);
}
