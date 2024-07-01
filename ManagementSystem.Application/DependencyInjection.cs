using ManagementSystem.Application.Services.Implementations;
using ManagementSystem.Application.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpContextService, HttpContextService>();
        //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDeviceTypeService, DeviceTypeService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IDeviceLoanService, DeviceLoanService>();
        services.AddScoped<ITicketService, TicketService>();
        return services;
    }
}
