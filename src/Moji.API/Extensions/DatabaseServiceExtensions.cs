namespace Moji.API.Extensions;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        //config DBContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //config Redis
        return services;
    }
}