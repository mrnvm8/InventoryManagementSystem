using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class OfficeMappings
{
    //For create
    public static Office MapToOffice(this OfficeRequest request)
    {
        return new Office
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
    }

    //For Update
    public static Office MapToOffice(this OfficeRequest request, Office office)
    {
        return new Office
        {
            Id = office.Id,
            Name = request.Name,
            Location = request.Location,
            Created = office.Created,
            Updated = DateTime.Now
        };
    }

    //For Dto Response
    public static OfficeDto MapToOfficeDto(this Office office)
    {
        return new OfficeDto
        {
            Id = office.Id,
            Name = office.Name,
            Location = office.Location,
        };
    }
}