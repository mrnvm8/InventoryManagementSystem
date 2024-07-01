namespace ManagementSystem.Application.Repositories.Interface;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> CreateAsync(TEntity entity, CancellationToken token = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken token = default);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken token = default);
}
