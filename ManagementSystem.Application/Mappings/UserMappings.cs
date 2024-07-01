using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests.Authentication;
using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Shared.Responses.Authentication;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ManagementSystem.Application.Mappings;

public static class UserMappings
{
    public static AppUser MapToUser(this UserRequest request, Guid employeeId)
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmployeeId = employeeId,
            Created = DateTime.Now,
            LastUpdated = DateTime.Now,
            LastSignIn = DateTime.Now,
        };
    }
    public static AppUser MapForLogin(this UserLoginRequest request, AppUser user)
    {
        return new AppUser
        {
            Id = user.Id,
            Password = request.Password,
            EmployeeId = user.EmployeeId,
            Created = user.Created,
            LastUpdated = user.LastUpdated,
            LastSignIn = DateTime.Now,
        };
    }

    public static AuthResponse MapToResponse(this AppUser user, string token, string role)
    {
        return new AuthResponse
        {
            Id = user.Id,
            Token = token,
        };
    }
    public static UsersDto MapToUserDto(this AppUser user)
    {
        return new UsersDto
        {
            userId = user.Id,
            EmployeeId = user.EmployeeId,
            UserName = user.Employee!.WorkEmail,
            EmployeeName = $"{user.Employee!.Name}, {user.Employee!.Surname}",
            DepartmentName = user.Employee!.Department!.Name,
            OfficeName = user.Employee.Department.Offices!.Name
        };
    }

    public static AppUser MapToChangePassword(this AppUser user, string Password)
    {
        return new AppUser
        {
            Id = user.Id,
            Password = BCrypt.Net.BCrypt.HashPassword(Password),
            EmployeeId = user.EmployeeId,
            Created = user.Created,
            LastUpdated = user.LastUpdated,
            LastSignIn = user.LastSignIn,
        };
    }
}
