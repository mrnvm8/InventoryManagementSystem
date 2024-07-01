using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using ManagementSystem.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace ManagementSystem.Database.Repositories.Implementations;

internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;

    public IOfficeRepository Offices { get; }
    public IDepartmentRepository Departments { get; }
    public IEmployeeRepository Employees { get; }
    public IDeviceTypeRepository DeviceType { get; }
    public IDeviceResipotory Devices { get; }
    public IDeviceLoanRepository DeviceLoans { get; }
    public ITicketRepository Tickets { get; }

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        var logger = loggerFactory.CreateLogger("logs");

        Offices = new OfficeRepository(context, logger);
        Departments = new DepartmentRepository(context, logger);
        Employees = new EmployeeRepository(_context, logger);
        DeviceType = new DeviceTypeRepository(context, logger);
        Devices = new DeviceRepository(context, logger);
        DeviceLoans = new DeviceLoanRepository(context, logger);
        Tickets = new TicketRepository(context, logger);
    }

    #region Saving change from database
    public async Task<Result<int>> SaveChangesAsync(CancellationToken token)
    {
        try
        {
            // Save changes asynchronously to the database
            var affectedRows = await _context.SaveChangesAsync(token);

            // If successful, return a successful Result object containing the entity
            return Result<int>.Success(affectedRows, "Save changes was Succefull");
        }
        catch (DbUpdateException ex)
        {
            // Return a failure Result object with a specific error message
            return Result<int>.Failure(ex.InnerException?.Message);
        }
        catch (MySqlException ex)
        {
            // Return a failure Result object with a specific error message
            return Result<int>.Failure($"SqlException: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Return a failure Result object with a specific error message
            return Result<int>.Failure($"Exception: {ex.Message}");
        }
    }
    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }


}
