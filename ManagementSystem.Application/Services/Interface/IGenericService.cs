using ManagementSystem.Shared.Responses;

namespace ManagementSystem.Application.Services.Interface;

public interface IGenericService<TRequest, TDto>
    where TRequest : class
    where TDto : class
{
    Task<Result<TDto>> CreateAsync(TRequest request, CancellationToken token = default);
    Task<Result<TDto>> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<IEnumerable<TDto>>> GetAllAsync(CancellationToken token = default);
    Task<Result<TDto>> UpdateAsync(TRequest request, Guid id, CancellationToken token = default);
    Task<Result<bool>> DeleteByIdAsync(Guid id, CancellationToken token = default);
}