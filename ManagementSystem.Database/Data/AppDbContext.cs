using ManagementSystem.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Database.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    //Office, Department, Employee
    public DbSet<Office> Offices { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    //User , User Role, System Roles
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<SystemRole> SystemRoles { get; set; }
    public DbSet<SystemUserRole> SystemUserRoles { get; set; }

    //Device Type, Device, DeviceLoan, DeviceSummary
    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceSummary>? DevicesSummaries { get; set; }
    public DbSet<DeviceLoan> DeviceLoans { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}
