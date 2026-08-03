using FluentValidation;
using Microsoft.Extensions.Configuration;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Services.Conversations;
using Moji.Contracts.Models.Auth;
using Moji.Contracts.Models.Auth.ChangePassword;
using Moji.Contracts.Models.Auth.Login;
using Moji.Contracts.Models.Auth.Register;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Commons.Exceptions;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Models;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
    private readonly IConfiguration _configuration;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly ISessionNotificationService _sessionNotificationService;
    private readonly ITokenService _tokenService;
    private readonly IDbTransactionManager _txManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService,
        IConfiguration configuration, IUserTokenRepository userTokenRepository, IDbTransactionManager txManager,
        IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator,
        ISessionNotificationService sessionNotificationService)
    {
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
        _userTokenRepository = userTokenRepository;
        _txManager = txManager;
        _changePasswordValidator = changePasswordValidator; ;
        _sessionNotificationService = sessionNotificationService;
    }

    public async Task SignUp(RegisterRequest request)
    {
        //validation request
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        var existedUser = await _userRepository.FindByUsernameAsync(request.Username);
        if (existedUser != null) throw new MojiConflictException("Tên tài khoản này đã tồn tại trong hệ thống!");

        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.Email);
        if (!isEmailUnique) throw new MojiConflictException("Email đã sử dụng!");

        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var noromalizeUsername = request.Username.Trim().ToLower();
        var normalizeEmail = request.Email?.Trim().ToLower();
        var user = new User
        {
            Username = noromalizeUsername,
            HashedPassword = hashedPassword,
            Email = normalizeEmail,
            FullName = request.FirstName + " " + request.LastName
        };
        _userRepository.Add(user);
        await _txManager.SaveChangesAsync();
    }

    public async Task<AuthResponse> SignIn(LoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        var user = await _userRepository.FindByUsernameAsync(request.Username);

        if (user == null) throw new MojiUnauthorizedException("Username hoặc password không chính xác!");

        var isMatchPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, request.Password);

        if (!isMatchPassword) throw new MojiUnauthorizedException("Username hoặc password không chính xác!");

        var accessToken = _tokenService.GenerateAccessToken(user, user.AuthVersion);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var userToken = new UserToken
        {
            Token = refreshToken,
            AuthVersion = user.AuthVersion,
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationInDays"] ??
                                                                   "15"))
        };
        await _userTokenRepository.Add(userToken);
        await _txManager.SaveChangesAsync();

        var authResponse = new AuthResponse
        (
            new UserResponse
            (
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.AvatarUrl,
                user.Bio,
                user.CreatedAt,
                user.UpdatedAt
            ),
            accessToken,
            refreshToken
        );
        return authResponse;
    }

    public async Task RevokeRefreshToken(string token)
    {
        var userToken = await _userTokenRepository.FindByTokenAsync(token);
        if (userToken == null || userToken.IsRevoked) return;

        userToken.IsRevoked = true;
        _userTokenRepository.RevokeToken(userToken);
        await _txManager.SaveChangesAsync();
    }

    public async Task<UserModel> GetUser(Guid id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) throw new MojiNotFoundException("User not found or disabled");

        var userModel = new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
        return userModel;
    }

    public async Task<AuthResponse> RefreshToken(string oldToken)
    {
        //1. Find token in db
        var token = await _userTokenRepository.FindByTokenAsync(oldToken);
        if (token == null) throw new MojiUnauthorizedException("Invalid token");

        //2. Check revoke and expiredTime
        if (token.IsRevoked || token.ExpiresAt < DateTimeOffset.UtcNow)
            throw new MojiUnauthorizedException("Token is revoked or expired");

        // check user exist and auth version is same
        var user = await _userRepository.FindByIdAsync(token.UserId);
        if (user == null || user.AuthVersion != token.AuthVersion)
            throw new MojiUnauthorizedException("Session has been revoked");


        //3. Generate new access token and refresh token
        var newAccessToken = _tokenService.GenerateAccessToken(user, token.AuthVersion);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        //4. Invalidate old refresh token

        //begin transaction
        await using var transaction = await _txManager.BeginTransactionAsync();
        try
        {
            var updateResult = await _userTokenRepository.RevokeTokenAsync(token);
            if (updateResult != UpdatedResult.Updated)
                throw new MojiUnauthorizedException("Token is revoked or expired");

            //5. Add new token in db
            var userToken = new UserToken
            {
                Token = newRefreshToken,
                AuthVersion = user.AuthVersion,
                UserId = user.Id,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(
                    _configuration["Jwt:RefreshTokenExpirationInDays"] ??
                    "15"))
            };
            await _userTokenRepository.Add(userToken);
            await _txManager.SaveChangesAsync();
            await _txManager.CommitAsync();
        }
        catch (DataConcurrencyException)
        {
            await _txManager.RollbackAsync();
            throw new MojiConflictException("User has been updated by another user");
        }

        //6. Create res

        var authResponse = new AuthResponse
        (
            new UserResponse(user.Id, user.Username, user.Email, user.FullName, user.AvatarUrl, user.Bio,
                user.CreatedAt, user.UpdatedAt),
            newAccessToken,
            newRefreshToken
        );
        //return res
        return authResponse;
    }

    public async Task ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        //validate request
        var validationResult = await _changePasswordValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        //check user exist
        var user = await _userRepository.GetTrackedUser(userId, cancellationToken);
        if (user == null) throw new MojiNotFoundException("User not found");

        //check password
        var isMatchPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, request.OldPassword);
        if (!isMatchPassword) throw new MojiBadRequestException("Old password is incorrect");

        var newHashedPassword = _passwordHasher.HashPassword(request.NewPassword);
        await using var transaction = await _txManager.BeginTransactionAsync(cancellationToken);
        try
        {
            user.HashedPassword = newHashedPassword;
            user.AuthVersion++;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _userTokenRepository.RevokeAllTokensAsync(user.Id);
            await _txManager.SaveChangesAsync(cancellationToken);
            await _txManager.CommitAsync(cancellationToken);
        }
        catch (DataConcurrencyException)
        {
            await _txManager.RollbackAsync(cancellationToken);
            throw new MojiConflictException("User has been updated by another user");
        }

        try
        {
            await _sessionNotificationService.BroadcastClientLogoutAsync(user.Id.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi khi thông báo cho client logout.");
        }
    }
}