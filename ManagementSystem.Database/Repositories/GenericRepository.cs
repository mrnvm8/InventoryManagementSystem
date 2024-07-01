using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    //add needed fields
    internal DbSet<TEntity> _dbSet;
    protected AppDbContext _context;
    public ILogger _logger;

    //Injecting the fields
    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;

        _dbSet = _context.Set<TEntity>();
    }

    public virtual async Task<bool> CreateAsync(TEntity entity, CancellationToken token = default)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Added;
        _logger.LogInformation($"Entity Repository{nameof(TEntity)}, Date : {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
            $"Action: Create");
        return await Task.FromResult(true);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default)
    {
        return await _dbSet
            .AsQueryable()
            .AsNoTracking()
            .ToListAsync(token);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _dbSet.FindAsync(id, token);
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken token = default)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Modified;
        _logger.LogInformation($"Entity Repository{nameof(TEntity)}, Date : {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
           $"Action: Update");
        return await Task.FromResult(true);
    }
    public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken token = default)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Deleted;
        _logger.LogInformation($"Entity Repository{nameof(TEntity)}, Date : {DateTime.Now.ToString("yyyy-MM-dd HH:mm")} " +
           $"Action: Delete");
        return await Task.FromResult(true);
    }
}
