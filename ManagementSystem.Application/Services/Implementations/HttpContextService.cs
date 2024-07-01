using ManagementSystem.Application.Helpers;
using ManagementSystem.Application.Services.Interface;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ManagementSystem.Application.Services.Implementations;

internal class HttpContextService(IHttpContextAccessor _httpContext) : IHttpContextService
{
  
    public Guid GetUserId()
    {
        //getting the user Id from the http
        var userId = _httpContext.HttpContext?.User.Claims.SingleOrDefault(u => u.Type.Equals(ClaimTypes.NameIdentifier));
        //checking if it a guid and return it
        if (Guid.TryParse(userId?.Value, out var Id))
        {
            return Id;
        }
        return Guid.Empty;

    }

    public string? GetUserRole()
    {
        //getting the use role from the http
        var role = _httpContext.HttpContext?.User.Claims.SingleOrDefault(u => u.Type.Equals(ClaimTypes.Role));
        //checking if it a guid and return it
        if (!string.IsNullOrEmpty(role?.Value))
        {
            return role.Value;
        }
        return null;
    }

    public string? GetUserName()
    {
        //getting the use role from the http
        var role = _httpContext.HttpContext?.User.Claims.SingleOrDefault(u => u.Type.Equals(ClaimTypes.Name));
        //checking if it a guid and return it
        if (!string.IsNullOrEmpty(role?.Value))
        {
            return role.Value;
        }
        return null;
    }

    public Guid GetEmployeeIdentifier()
    {
        //getting the use employee id from the http
        var employeeId = _httpContext.HttpContext?.User.Claims.SingleOrDefault(u => u.Type.Equals(AuthConstants.EmployeeIdentifier));
        //checking if it a guid and return it
        if (Guid.TryParse(employeeId?.Value, out var Id))
        {
            return Id;
        }
        return Guid.Empty;
    }
    public Guid GetDepartmentIdentifier()
    {
        //getting the use depaetment Id from the http
        var departmentId = _httpContext.HttpContext?.User.Claims.SingleOrDefault(u => u.Type.Equals(AuthConstants.DepartmentIdentifier));
        //checking if it a guid and return it
        if (Guid.TryParse(departmentId?.Value, out var Id))
        {
            return Id;
        }
        return Guid.Empty;
    }


}
