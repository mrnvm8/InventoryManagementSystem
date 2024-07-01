using ManagementSystem.Shared.Requests.Authentication;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Shared.Responses.Authentication;

namespace ManagementSystem.Application.Services.Interface;

public interface IAuthService
{
    Task<Shared.Responses.Result<AuthResponse>> Register(UserRequest request, CancellationToken token = default);
    Task<Shared.Responses.Result<AuthResponse>> Login(UserLoginRequest request, CancellationToken token = default);
    Task<Shared.Responses.Result<IEnumerable<UsersDto?>>> GetUsers(CancellationToken token = default);
    Task<Shared.Responses.Result<UsersDto?>> GetUserById(Guid userId, CancellationToken token = default);
    Task<Shared.Responses.Result<bool>> ChangeUserPassword(Guid userId, string newPassword, CancellationToken token = default);
    Task<Shared.Responses.Result<bool>> RemoveUserAsync(Guid userId, CancellationToken token = default);
}

