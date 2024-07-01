using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using ManagementSystem.Database.Repositories.Implementations;
using ManagementSystem.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ManagementSystem.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryData(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<JwtSection>(configuration.GetSection(JwtSection.SectionName));

        services.AddScoped<IAuthRepository, AuthRepository>();

        services.AddLogging();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(option =>
        {
            option.UseMySQL(connectionString).
            UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        return services;
    }
}
