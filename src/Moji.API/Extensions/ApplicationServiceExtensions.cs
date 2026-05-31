namespace Moji.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
       // config CORS
       var corsPolicyName = "CorsPolicy";
       services.AddCors(options =>
       {
           options.AddPolicy(name: corsPolicyName, policy =>
           {
               policy.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
           });
       });
        
        return services;
    }

    public static IServiceCollection AddServiceServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        return services;
    }
}