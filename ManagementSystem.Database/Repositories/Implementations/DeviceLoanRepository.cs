using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class DeviceLoanRepository : GenericRepository<DeviceLoan>, IDeviceLoanRepository
{
    public DeviceLoanRepository(AppDbContext context, ILogger logger) : base(context, logger)
    { }

    public override async Task<IEnumerable<DeviceLoan>> GetAllAsync(CancellationToken token = default)
    {
        return await _dbSet
            .Include(u => u.User)
            .Include(u => u.Device)
            .Include(u => u.Employee)
            .Where(x => x.ReturnDate.Equals(DateTime.Parse("2009-01-01")))
            .ToListAsync(token);
    }


    public async Task<DeviceLoan?> GetByDeviceIdAsync(Guid deviceId, CancellationToken token = default)
    {
        return await _dbSet
            .Include(u => u.Device)
            .Include(u => u.Employee)
                .ThenInclude(d => d!.Department)
            .Where(x => x.ReturnDate.Equals(DateTime.Parse("2009-01-01")))
            .FirstOrDefaultAsync(d => d.DeviceId.Equals(deviceId));
    }

    public override async Task<DeviceLoan?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _dbSet
           .Include(u => u.Device)
           .Include(u => u.Employee)
               .ThenInclude(d => d!.Department)
           .Where(x => x.ReturnDate.Equals(DateTime.Parse("2009-01-01")))
           .FirstOrDefaultAsync(d => d.Id.Equals(id));
    }

    public async Task<IEnumerable<DeviceLoan>> GetDevicesByEmployeeIdAsync(Guid employeeId, CancellationToken token = default)
    {
        return await _dbSet
           .Include(u => u.User)
           .Include(u => u.Device)
                .ThenInclude(t => t!.DeviceType)
           .Include(u => u.Device)
                .ThenInclude(d => d.Department)
            .Include(u => u.Employee)
           .Where(x => x.EmployeeId.Equals(employeeId) &&
                       x.ReturnDate.Equals(DateTime.Parse("2009-01-01")))
           .ToListAsync(token);
    }

    public async Task<IEnumerable<DeviceLoan>> GetDevicesLoanHistoryByIdAsync(Guid deviceId, CancellationToken token = default)
    {
        return await _dbSet
            .Include(u => u.User)
            .Include(u => u.Device)
            .Include(u => u.Employee)
            .Where(x => x.DeviceId.Equals(deviceId))
            .ToListAsync(token);
    }
}