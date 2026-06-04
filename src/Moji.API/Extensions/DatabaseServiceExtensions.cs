using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Configurations;

namespace Moji.API.Extensions;

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