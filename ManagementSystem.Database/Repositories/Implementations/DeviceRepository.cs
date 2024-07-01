using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class DeviceRepository : GenericRepository<Device>, IDeviceResipotory
{
    public DeviceRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {}

    public override async Task<IEnumerable<Device>> GetAllAsync(CancellationToken token = default)
    {
        return await _dbSet
            .Include(d => d.Department)
                .ThenInclude(o => o!.Offices)
            .Include(t => t.DeviceType)
            .ToListAsync(token);
    }

    public override async Task<Device?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _dbSet
           .Include(d => d.Department)
                .ThenInclude(o => o!.Offices)
            .Include(t => t.DeviceType)
           .FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<IEnumerable<Device>> DevicesByDepamentAndDeviceType(
         Guid departmentId,
         Guid TypeId,
         CancellationToken token = default) =>
         await _dbSet
         .Include(d => d.Department)
         .Include(l => l.DevicesLoans)
             .ThenInclude(e => e.Employee)
         .Include(d => d.DeviceType)
         .Where(d => d.DepartmentId.Equals(departmentId) && d.DeviceTypeId.Equals(TypeId))
         .ToListAsync();

    public async Task<List<DeviceSummary>> DeviceSummary(Guid departmentId, CancellationToken token = default)
    {
        var date = Convert.ToDateTime("2009-01-01").ToString("yyyy-MM-dd");
        var sql = "";

        if (departmentId.Equals(Guid.Empty))
        {
            sql = String.Format("SELECT d.DeviceTypeId, p.Id AS DepartmentId, p.Name, " +
               "t.Name AS TypeName, o.Name AS OfficeName, COUNT(*) AS Total, " +
               "(SELECT COUNT(*) FROM DeviceLoans l  " +
               "LEFT JOIN Devices e ON l.DeviceId = e.Id " +
               "LEFT JOIN Departments s ON e.DepartmentId = s.Id " +
               "WHERE e.DeviceTypeId = d.DeviceTypeId AND e.DepartmentId = d.DepartmentId " +
               "AND l.ReturnDate = '{0}') AS Assigned, " +
               "(SELECT COUNT(*) FROM Devices u " +
               "LEFT JOIN Departments p ON u.DepartmentId = p.Id " +
               "WHERE u.DeviceTypeId = d.DeviceTypeId AND u.DepartmentId = d.DepartmentId " +
               "AND(u.Condition = '3' OR u.Condition = '4' OR u.Condition = '5' OR u.Condition = '6')) AS Unavailable " +
               "FROM Devices d " +
               "LEFT JOIN Departments p ON p.Id = d.DepartmentId " +
               "LEFT JOIN DeviceTypes t ON t.Id = d.DeviceTypeId " +
               "LEFT JOIN Offices o ON o.Id = p.OfficeId " +
               "GROUP BY d.DeviceTypeId, d.DepartmentId", date);
        }
        else
        {

            sql = String.Format("SELECT d.DeviceTypeId, p.Id AS DepartmentId, p.Name, " +
                "t.Name AS TypeName, o.Name AS OfficeName, COUNT(*) AS Total, " +
                "(SELECT COUNT(*) FROM DeviceLoans l  " +
                "LEFT JOIN Devices e ON l.DeviceId = e.Id " +
                "LEFT JOIN Departments s ON e.DepartmentId = s.Id " +
                "WHERE e.DeviceTypeId = d.DeviceTypeId AND e.DepartmentId = d.DepartmentId " +
                "AND l.ReturnDate = '{0}') AS Assigned, " +
                "(SELECT COUNT(*) FROM Devices u " +
                "LEFT JOIN Departments p ON u.DepartmentId = p.Id " +
                "WHERE u.DeviceTypeId = d.DeviceTypeId AND u.DepartmentId = d.DepartmentId " +
                "AND(u.Condition = '3' OR u.Condition = '4' OR u.Condition = '5' OR u.Condition = '6')) AS Unavailable " +
                "FROM Devices d " +
                "LEFT JOIN Departments p ON p.Id = d.DepartmentId " +
                "LEFT JOIN DeviceTypes t ON t.Id = d.DeviceTypeId " +
                "LEFT JOIN Offices o ON o.Id = p.OfficeId " +
                "WHERE d.DepartmentId = '{1}' " +
                "GROUP BY d.DeviceTypeId, d.DepartmentId", date, departmentId);
        }

        var result = await _context.DevicesSummaries!
            .FromSqlRaw(sql)
            .ToListAsync();

        return result;
    }

    public async Task<bool> DeviceExistAsync(Device device, CancellationToken token)
    {
        //checking if the device exist with same department
        //from the database return true/false;
        var exist = false;
        if (string.IsNullOrEmpty(device.DeviceSerialNo))
        {
            exist = await _dbSet
                    .AnyAsync(x => x.DeviceTypeId.Equals(device.DeviceTypeId)
                              && x.DeviceIMEINo!.ToLower().Equals(device.DeviceIMEINo!.ToLower()) 
                              && x.Id != device.Id);
        }
        else if (string.IsNullOrEmpty(device.DeviceIMEINo))
        {
            exist = await _dbSet
                    .AnyAsync(x => x.DeviceTypeId.Equals(device.DeviceTypeId)
                              && x.DeviceSerialNo!.ToLower().Equals(device.DeviceSerialNo!.ToLower())
                              && x.Id != device.Id);
        }

        return exist;
    }
}
