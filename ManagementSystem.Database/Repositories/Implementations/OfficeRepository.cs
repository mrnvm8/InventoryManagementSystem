using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class OfficeRepository : GenericRepository<Office>, IOfficeRepository
{
    public OfficeRepository(AppDbContext context, ILogger logger) : base(context, logger){}

    public async Task<bool> OfficeNameExist(Office office, CancellationToken token)
    {
        return await _dbSet.AnyAsync(x => x.Name.ToLower().Equals(office.Name.ToLower()) && x.Id != office.Id, token);
    }
}
