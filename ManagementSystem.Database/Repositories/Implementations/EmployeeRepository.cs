using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context, ILogger logger) : base(context, logger) { }


    //override the get all method for the employees 
    public override async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken token = default) =>
        await _dbSet
            .Include(d => d.Department)
                .ThenInclude(o => o!.Offices)
            .ToListAsync(token);


    public override async Task<Employee?> GetByIdAsync(Guid id, CancellationToken token = default) =>
        await _dbSet
            .Include(d => d.Department)
                .ThenInclude(o => o!.Offices)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), token);
   
    public async Task<Employee?> GetEmailByEmailAddressAsync(string email, CancellationToken token = default) =>
        await _dbSet
             .FirstOrDefaultAsync(e => e.WorkEmail.ToLower()
             .Equals(email.ToLower()), token);

    public async Task<bool> ExistingAsync(Employee employee, CancellationToken token)=>
        await _dbSet.AnyAsync(e => e.WorkEmail.ToLower().Equals(employee.WorkEmail.ToLower())
                && e.Id != employee.Id, token);


}