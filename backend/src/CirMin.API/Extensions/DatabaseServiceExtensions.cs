using Microsoft.EntityFrameworkCore;
using CirMin.DataAccess.Configurations;

namespace CirMin.API.Extensions;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        //config DBContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        //config Redis
        return services;
    }
}