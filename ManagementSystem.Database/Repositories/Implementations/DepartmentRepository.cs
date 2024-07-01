using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {}

    public override async Task<IEnumerable<Department>> GetAllAsync(CancellationToken token = default) =>
        await _dbSet
            .Include(o => o.Offices)
            .ToListAsync(token);


    public override async Task<Department?> GetByIdAsync(Guid id, CancellationToken token = default) =>
        await _dbSet
           .Include(o => o.Offices)
           .FirstOrDefaultAsync(x => x.Id.Equals(id));

    public async Task<bool> DepartmentExist(Department department ,CancellationToken token) =>
        await _dbSet
        .AnyAsync(x => x.Name.ToLower().Equals(department.Name.ToLower()) &&
                   x.OfficeId.Equals(department.OfficeId) && x.Id != department.Id, token);
}
