using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class DepartmentMappings
{
    public static Department MapToDepartment(this DepartmentRequest request)
    {
        return new Department
        {
            Id = Guid.NewGuid(),
            OfficeId = request.OfficeId,
            Name = request.DepartmentName,
            Description = request.Description,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }

    public static Department MapToDepartment(this DepartmentRequest request, Department department)
    {
        return new Department
        {
            Id = department.Id,
            OfficeId = request.OfficeId,
            Name = request.DepartmentName,
            Description = request.Description,
            Created = department.Created,
            Updated = DateTime.Now
        };
    }

    public static DepartmentDto MapToDepartmentDto(this Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            OfficeId = department.OfficeId,
            DepartmentName = department.Name,
            Description = department.Description,
            DepartmentOfficeName = department.Offices!.Name
        };
    }
}