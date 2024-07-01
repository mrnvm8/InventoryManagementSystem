using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;

public interface ITicketService
{
    Task<Result<IEnumerable<TicketDto>>> GetTickets(CancellationToken token = default);
    Task<Result<IEnumerable<TicketDto>>> GetTicketsOfDevice(Guid deviceId, CancellationToken token = default);
    Task<Result<TicketDto>> GetTicketById(Guid ticketId, CancellationToken token = default);
    Task<Result<TicketDto>> AddTicket(TicketRequest request, CancellationToken token = default);
    Task<Result<bool>> DeleteTicket(Guid ticketId, CancellationToken token = default);
    Task<Result<TicketDto>> TicketArchived(TicketRequest request, Guid ticketId, CancellationToken token = default);
    Task<Result<TicketDto>> TicketAcknowledge(TicketRequest request, Guid ticketId, CancellationToken token = default);
}
