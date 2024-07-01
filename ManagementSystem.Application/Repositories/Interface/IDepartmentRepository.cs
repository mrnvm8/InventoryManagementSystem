using ManagementSystem.Application.Entities;

namespace ManagementSystem.Application.Repositories.Interface;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<bool> DepartmentExist(Department department, CancellationToken token);
}

