using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;
public static class EmployeeMappings
{
    public static Employee MapToEmployee(this EmployeeRequest request)
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            DepartmentId = request.DepartmentId,
            Name = request.Name,
            Surname = request.Surname,
            Gender = request.Gender,
            WorkEmail = request.WorkEmail,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }

    public static Employee MapToEmployee(this EmployeeRequest request, Employee employee)
    {
        return new Employee
        {
            Id = employee.Id,
            DepartmentId = request.DepartmentId,
            Name = request.Name,
            Surname = request.Surname,
            Gender = request.Gender,
            WorkEmail = request.WorkEmail,
            CanRegister = request.CanRegister,
            Created = employee.Created,
            Updated = DateTime.Now
        };
    }

    public static EmployeeDto MapToEmployeeDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            DepartmentId = employee.DepartmentId,
            Name = employee.Name,
            Surname = employee.Surname,
            Gender = employee.Gender,
            WorkEmail = employee.WorkEmail.ToLower(),
            CanRegister = employee.CanRegister,
            Department = employee.Department!.Name,
            OfficeName = employee.Department.Offices!.Name,
            FullName = $"{employee.Name.Substring(0, 1).ToUpper() + employee.Name.Substring(1, employee.Name.Length - 1).ToLower()}, " +
                       $"{employee.Surname.Substring(0, 1).ToUpper() + employee.Surname.Substring(1, employee.Surname.Length - 1).ToLower()}"
                       
        };
    }
}