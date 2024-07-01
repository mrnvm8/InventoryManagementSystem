using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class TicketMappings
{
    public static Ticket CreateMapToTicket(this TicketRequest request, Guid userId)
    {
        return new Ticket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceId = request.DeviceId,
            TicketTitle = request.TicketTitle,
            TicketIssue = request.TicketIssue,
            TicketCreatedDate = DateTime.Now
        };
    }


    public static Ticket MapToTicket(this TicketRequest request, Guid ticketId, Ticket ticket)
    {
        return new Ticket
        {
            Id = ticketId,
            DeviceId = ticket.DeviceId,
            UserId = ticket.UserId,
            TicketTitle = ticket.TicketTitle,
            TicketIssue = ticket.TicketIssue,
            TicketCreatedDate = ticket.TicketCreatedDate,
            ArangedDate = request.ArangedDate.HasValue ? request.ArangedDate : ticket.ArangedDate,
            TicketUpdate = request.TicketUpdate is null ? ticket.TicketUpdate : request.TicketUpdate,
            FixedDate = request.TicketSolution is null ? ticket.FixedDate : DateTime.Now,
            TicketSolution = request.TicketSolution is null ? ticket.TicketSolution : request.TicketSolution,
            Updated = request.TicketUpdate is null ? ticket.Updated : true,
            IssueSolved = request.TicketSolution is null ? ticket.IssueSolved : true
        };
    }
    public static TicketDto MapTicketDto(this Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            DeviceId = ticket.DeviceId,
            DepartmentId = ticket.Devices!.DepartmentId,
            DeviceTypeId = ticket.Devices!.DeviceTypeId,
            TicketTitle = ticket.TicketTitle,
            TicketIssue = ticket.TicketIssue,
            TicketSolution = ticket.TicketSolution,
            TicketCreatedDate = ticket.TicketCreatedDate,
            ArangedDate = ticket.ArangedDate,
            FixedDate = ticket.FixedDate,
            TicketUpdate = ticket.TicketUpdate,
            IssueSolved = ticket.IssueSolved,
            Updated = ticket.Updated,
            DeviceName = ticket.Devices!.Name,
            DeviceType = ticket.Devices!.DeviceType?.Name,
            Department = ticket.Devices!.Department?.Name,
        };
    }
}

