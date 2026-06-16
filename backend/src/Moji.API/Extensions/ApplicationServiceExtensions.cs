using Moji.BusinessLogic.Services.Auth;
using Moji.BusinessLogic.Services.Friends;
using Moji.DataAccess.Repositories;
using Moji.DataAccess.Repositories.Impl;

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
               policy.WithOrigins("http://localhost:5173")
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
        
        //config services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFriendShipService, FriendShipService>();
        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        //config repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IFriendShipRepository, FriendShipRepository>();
        return services;
    }
}