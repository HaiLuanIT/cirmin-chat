using CirMin.API.Integrations.Cloudinary;
using CirMin.API.Models.Users.UploadAvatar;
using CirMin.API.RealTimes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using CirMin.BusinessLogic.Services.Auth;
using CirMin.BusinessLogic.Services.Conversations;
using CirMin.BusinessLogic.Services.Friends;
using CirMin.BusinessLogic.Services.Messages;
using CirMin.BusinessLogic.Services.Storage;
using CirMin.BusinessLogic.Services.Users;
using CirMin.Contracts.Models.Auth.Register;
using CirMin.DataAccess.Commons.DbTransactionManagers;
using CirMin.DataAccess.Repositories;
using CirMin.DataAccess.Repositories.Impl;

namespace CirMin.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        // config CORS
        var corsPolicyName = "CorsPolicy";
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddServiceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        //add db manager transaction service
        services.AddScoped<IDbTransactionManager, DbTransactionManager>();

        //add fluent validation
        services.AddValidatorsFromAssembly(typeof(RegisterRequest).Assembly);
        services.AddValidatorsFromAssembly(typeof(UpdateAvatarHttpRequest).Assembly);

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        //config services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFriendShipService, FriendShipService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IPresenceService, PresenceService>();
        services.AddScoped<IMessageNotificationService, MessageNotificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IConversationNotificationService, ConversationNotificationService>();
        services.AddScoped<IAccessTokenValidator, AccessTokenValidator>();
        services.AddScoped<ISessionNotificationService, SessionNotificationService>();

        //add cloudinary
        services.Configure<CloudinaryOptions>(configuration.GetSection("Cloudinary"));
        services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

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