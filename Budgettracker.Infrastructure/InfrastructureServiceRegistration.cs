using BudgetTracker.Domain;
using BudgetTracker.Infrastructure.Configuration;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetTracker.Infrastructure.Repositories;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(
            configuration.GetSection("DatabaseSettings"));

        services.AddDbContext<BudgetTrackerDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
                mysqlOptions =>
                {
                    mysqlOptions.CommandTimeout(30);
                    mysqlOptions.EnableRetryOnFailure(3);
                });
            
            // Disable sensitive data log
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != "Development")
            {
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
            }
        });

        services.AddScoped<IRecordRepository, RecordRepository>();
        services.AddScoped<IMonthCategoryBudgetRepository, MonthCategoryBudgetRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        return services;
    }
}