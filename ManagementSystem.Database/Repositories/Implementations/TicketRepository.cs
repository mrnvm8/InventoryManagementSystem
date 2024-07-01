using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context, ILogger logger) : base(context, logger)
    { }
    public override async Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken token = default) =>
        await _dbSet
        .AsNoTracking()
        .Include(d => d.Devices)
            .ThenInclude(x => x!.Department)
        .Include(d => d.Devices)
            .ThenInclude(d => d!.DeviceType)
        .Include(u => u.Users)
        .Where(t => t.IssueSolved.Equals(false))
        .ToListAsync();

    public override async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken token = default) =>
          await _dbSet
        .Include(d => d.Devices)
            .ThenInclude(x => x!.Department)
        .Include(d => d.Devices)
            .ThenInclude(d => d!.DeviceType)
        .Include(u => u.Users)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id));


    public async Task<bool> ExistTicketAsync(Ticket ticket) =>
        await _dbSet.AnyAsync((x => x.DeviceId.Equals(ticket.DeviceId)
        && x.IssueSolved.Equals(false)));

    public async Task<IEnumerable<Ticket>> GetTicketsOfDeviceAsync(Guid deviceId, CancellationToken token)
    {
        return await _dbSet
       .AsNoTracking()
       .Include(d => d.Devices)
           .ThenInclude(x => x!.Department)
       .Include(d => d.Devices)
           .ThenInclude(d => d!.DeviceType)
       .Include(u => u.Users)
       .Where(d => d.DeviceId.Equals(deviceId))
       .ToListAsync();
    }
}