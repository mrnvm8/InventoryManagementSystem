using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Helpers.Errors.Auth;
using ManagementSystem.Application.Mappings;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Application.Services.Interface;
using ManagementSystem.Shared.Requests.Authentication;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Shared.Responses.Authentication;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ManagementSystem.Application.Services.Implementations;

internal class AuthService(
    IUnitOfWork _unitOfWork,
    IAuthRepository _authRepository,
    ILogger<AuthService> _logger) : IAuthService
{

    #region Registering user login
    public async Task<Result<AuthResponse>> Register(UserRequest request, CancellationToken token)
    {
        //Validate the password and making sure is not a bad passwaord 
        var validate = ValidatePassaword(request!.Password);
        if (validate.Count != 0)
        {
            var _messages = string.Empty;
            foreach (var item in validate)
            {
                _messages = item + "/" + _messages;
            }
            return Result<AuthResponse>.Failure(_messages); ;
        }

        //check if email exist from the employee table
        //want to make the system to only register employee that are existig
        //sending the 
        var employee = await _unitOfWork.Employees.GetEmailByEmailAddressAsync(request!.Email);
        if (employee is null)
        {
            _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.NotFoundError}");
            return Result<AuthResponse>.Failure(AuthErrorMessage.NotFoundError);
        }

        //checking ig you are allowed to register to the system
        if (employee.CanRegister)
        {
            //map request to user
            var user = request?.MapToUser(employee.Id);

            //check if the user already registered
            var userExist = await _authRepository.GetUserByEmployeeId(employee.Id, token);
            if (userExist is not null)
            {
                _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.UserExist}");
                return Result<AuthResponse>.Failure(AuthErrorMessage.UserExist);
            }

            //Add user record for login
            var createdUser = await _authRepository.CreateAsync(user);
            //check if we are getting a null or user back
            if (createdUser is null)
            {
                _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.CreateFailError}");
                return Result<AuthResponse>.Failure(AuthErrorMessage.CreateFailError);
            }

            //get the created user by Id
            var dbUser = await _authRepository.GetUserById(createdUser.Id);
            if (dbUser is null)
            {
                _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.CreateFailError}");
                return Result<AuthResponse>.Failure(AuthErrorMessage.CreateFailError);
            }
            //get the generated token
            var generatedToken = await GenerateUserToken(dbUser, token);

            //return the generated token
            return Result<AuthResponse>.Success(generatedToken, AuthSuccessMessage.CreateSuccess);
        }
        _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.Fobidden}");
        return Result<AuthResponse>.Failure(AuthErrorMessage.IncorrectError);
    }
    #endregion

    #region Login the user to application
    public async Task<Result<AuthResponse>> Login(UserLoginRequest request, CancellationToken token)
    {

        //Check if the email exist on the employees table
        var employee = await _unitOfWork.Employees.GetEmailByEmailAddressAsync(request!.Email, token);
        if (employee is null)
        {
            _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.NotFoundError}");
            return Result<AuthResponse>.Failure(AuthErrorMessage.NotFoundError);
        }

        //Check if the email is register for login
        var dbUser = await _authRepository.GetUserByEmployeeId(employee!.Id, token);
        if (dbUser is null)
        {
            _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.IncorrectError}");
            return Result<AuthResponse>.Failure(AuthErrorMessage.IncorrectError);
        }

        //Map request to user
        var user = request?.MapForLogin(dbUser);

        //checking if the password matches
        if (!BCrypt.Net.BCrypt.Verify(user!.Password, dbUser.Password))
        {
            _logger.LogError($"{nameof(AuthService)}: {AuthErrorMessage.IncorrectError}");
            return Result<AuthResponse>.Failure(AuthErrorMessage.IncorrectError);
        }

        //get the generated token
        var generatedToken = await GenerateUserToken(dbUser, token);
        //return the generated token
        return Result<AuthResponse>.Success(generatedToken, AuthSuccessMessage.LogniSuccess);
    }
    #endregion

    #region Get all users
    public async Task<Result<IEnumerable<UsersDto?>>> GetUsers(CancellationToken token = default)
    {
        var users = await _authRepository.GetAllUsersAsync(token);
        if(!users.Any())
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: {AuthErrorMessage.NoUsers}");
            return Result<IEnumerable<UsersDto?>>.Failure(AuthErrorMessage.NoUsers);
        }
        return Result<IEnumerable<UsersDto?>>
            .Success(users.Select(u => u.MapToUserDto()), string.Empty);
    }
    #endregion

    #region Get user by their id
    public async Task<Result<UsersDto?>> GetUserById(Guid userId, CancellationToken token = default)
    {
        var user = await _authRepository.GetUserById(userId, token);
        if (user is null)
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: " +
                $"{AuthErrorMessage.NotFoundError}");
            return Result<UsersDto?>.Failure(AuthErrorMessage.NotFoundError);
        }
        return Result<UsersDto?>.Success(user.MapToUserDto(), string.Empty);
    }
    #endregion

    #region changing user password
    public async Task<Result<bool>> ChangeUserPassword(Guid userId, string newPassword, CancellationToken token = default)
    {
        var user = await _authRepository.GetUserById(userId, token);
        if (user is null)
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: " +
                $"{AuthErrorMessage.NotFoundError}");
            return Result<bool>.Failure(AuthErrorMessage.NotFoundError);
        }

        //mapping user for changing password
        var userMapped = user.MapToChangePassword(newPassword);

        //change password
        var changed = await _authRepository.ChangePassword(userMapped);
        if (!changed)
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: " +
                $"{AuthErrorMessage.NotFoundError}");
            return Result<bool>.Failure(AuthErrorMessage.NotFoundError);
        }
        return Result<bool>.Success(true, string.Empty);
    }

    #endregion

    #region removing user login from the system
    public async Task<Result<bool>> RemoveUserAsync(Guid userId, CancellationToken token = default)
    {
        var user = await _authRepository.GetUserById(userId, token);
        if (user is null)
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: " +
                $"{AuthErrorMessage.NotFoundError}");
            return Result<bool>.Failure(AuthErrorMessage.NotFoundError);
        }

        var deleted = await _authRepository.DeleteUser(user, token);
        if (!deleted)
        {
            _logger.LogError($"{nameof(AuthService)}, Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}: " +
                $"{AuthErrorMessage.DeleteError}");
            return Result<bool>.Failure(AuthErrorMessage.DeleteError);
        }
        return Result<bool>.Success(true, string.Empty);
    }
    #endregion

    #region Generating User Token 
    private async Task<AuthResponse> GenerateUserToken(AppUser dbUser, CancellationToken token = default)
    {
        //get user role 
        var getUserRole = await _authRepository.UserRole(dbUser.Id, token);
        //get System Role
        var getSystemRole = await _authRepository.SystemRole(getUserRole!.RoleId, token);

        //generate login token
        var jwtToken = _authRepository.GenerateToken(dbUser, getSystemRole!.Name);

        return dbUser.MapToResponse(jwtToken, getSystemRole!.Name);
    }
    #endregion

    #region validating passward checkin g if it meet requirement
    private List<string> ValidatePassaword(string password)
    {
        var _messages = new List<string>();

        //break down the reg into pattern to list
        var patterns = new List<string>
        {
           "(?=^.{8,120}$)", "(?=.*?[A-Z])", "(?=.*?[a-z])",
           "(?=.*?[0-9])", "(?=.*?[#?!@$%^&*-])"
        };

        //Error message of the pattern to list
        var ErrorMessagse = new List<string>
        {
                "Password must at least have 8 - 120 characters in length.",
                "Password must at least have one uppercase letter (A-Z).",
                "Password must at least have one lowercase letter (a-z).",
                "Password must at least have one digit (0-9).",
                "Password must at least have one special character (@,#,%,&,!,$,etc…).",
        };

        //looping through the pattern list match with the password
        //if match is true continue, if match false display the error
        for (var x = 0; x < patterns.Count; x++)
        {
            var validate = new Regex(patterns[x]);

            if (!validate.IsMatch(password))
            {
                _messages.Add(ErrorMessagse[x]);
            }
            else
            {
                continue;
            }
        }

        //checking for consecutive repeating characters
        if (ConsecutiveRepeatingCh(password))
        {
            var message = "Password can not have consecutive repeating characters or digit";
            _messages.Add(message);
        }
        else if (BlacklistedWords(password))
        {
            var message = "Password contains blacklisted words or digits e.g 123 or axium";
            _messages.Add(message);
        }

        return _messages;
    }

    //checking for repeating of character or digit 
    //eg. pas666 or paaa@1
    private bool ConsecutiveRepeatingCh(string password)
    {
        for (var x = 0; x < password.Length - 2; x++)
        {
            if (password[x].Equals(password[x + 1]) && password[x].Equals(password[x + 2]))
            {
                return true;
            }
        }
        return false;
    }

    //Forbidden words as password
    private bool BlacklistedWords(string password)
    {
        var blacklisted = new List<string> { "123", "password", "axium", "admin", "administrator" };

        var validator = blacklisted
                        .Select(word => new Regex($@"\w*{Regex.Escape(word)}\w*", RegexOptions.IgnoreCase))
                        .ToList();
        if (validator.Any(valid => valid.IsMatch(password)))
        {
            return true;
        }
        return false;
    }
    #endregion

}