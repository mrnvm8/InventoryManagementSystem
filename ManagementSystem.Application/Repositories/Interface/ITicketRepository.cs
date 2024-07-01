using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Repositories.Interface;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<bool> ExistTicketAsync(Ticket ticket);
    Task<IEnumerable<Ticket>> GetTicketsOfDeviceAsync(Guid deviceId, CancellationToken token);
}
