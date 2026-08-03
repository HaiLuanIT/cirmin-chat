using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Moji.BusinessLogic.Services.Auth;

namespace Moji.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                //add check token for signalR
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var authVersionValue = context.Principal?.FindFirst("auth_version")?.Value;

                        if (!Guid.TryParse(userIdValue, out var userId) ||
                            !int.TryParse(authVersionValue, out var tokenAuthVersion))
                        {
                            context.Fail("Access token is missing required claims");
                            return;
                        }

                        var validator = context.HttpContext.RequestServices.GetRequiredService<IAccessTokenValidator>();

                        var isValid = await validator.IsValidAsync(userId, tokenAuthVersion,
                            context.HttpContext.RequestAborted);
                        if (!isValid) context.Fail("Access token has been revoked");
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}