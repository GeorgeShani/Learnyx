using learnyx.Data;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Configuration;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
            
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("learnyx");
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            });
                
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
    }
}