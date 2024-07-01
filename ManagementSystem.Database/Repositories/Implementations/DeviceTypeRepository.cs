using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class DeviceTypeRepository : GenericRepository<DeviceType>, IDeviceTypeRepository
{
    public DeviceTypeRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {}

    public async Task<bool> DeviceTypeNameExist(DeviceType type, CancellationToken token)
    {
        return await _dbSet.AnyAsync(x => x.Name.ToLower().Equals(type.Name.ToLower()) & x.Id != type.Id, token);
    }
}
