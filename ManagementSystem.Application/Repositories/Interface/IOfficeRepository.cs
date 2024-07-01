using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IOfficeRepository: IGenericRepository<Office>
{
    Task<bool> OfficeNameExist(Office office, CancellationToken token);
}
