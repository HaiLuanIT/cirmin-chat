using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moji.API.RealTimes;
using Moji.BusinessLogic.Models.Auth;
using Moji.BusinessLogic.Services.Auth;
using Moji.BusinessLogic.Services.Conversations;
using Moji.BusinessLogic.Services.Friends;
using Moji.BusinessLogic.Services.Messages;
using Moji.BusinessLogic.Services.Users;
using Moji.DataAccess.Commons.DbTransactionManagers;
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
        
        //add db manager transaction service
        services.AddScoped<IDbTransactionManager, DbTransactionManager>();
        
        //add fluent validation
        services.AddValidatorsFromAssembly(typeof(RegisterRequest).Assembly);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        //config services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFriendShipService, FriendShipService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IPresenceService, PresenceService>();
        services.AddScoped<IMessageNotificationService, MessageNotificationService>();
        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        //config repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IFriendShipRepository, FriendShipRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        return services;
    }
}