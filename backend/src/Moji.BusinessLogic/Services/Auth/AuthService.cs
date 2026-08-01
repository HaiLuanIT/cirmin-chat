using FluentValidation;
using Microsoft.Extensions.Configuration;
using Moji.BusinessLogic.Exceptions;
using Moji.Contracts.Models.Auth;
using Moji.Contracts.Models.Auth.Login;
using Moji.Contracts.Models.Auth.Register;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly ITokenService _tokenService;
    private readonly IDbTransactionManager _txManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService,
        IConfiguration configuration, IUserTokenRepository userTokenRepository, IDbTransactionManager txManager,
        IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
        _userTokenRepository = userTokenRepository;
        _txManager = txManager;
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

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var userToken = new UserToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationInDays"] ??
                                                                   "15"))
        };
        _userTokenRepository.Add(userToken);
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

        // find user
        var user = await _userRepository.FindByIdAsync(token.UserId);
        if (user == null) throw new MojiNotFoundException("User not found or disabled");

        //3. Generate new access token and refresh token
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        //4. Invalidate old refresh token
        await RevokeRefreshToken(oldToken);

        //5. Add new token in db
        var userToken = new UserToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationInDays"] ??
                                                                   "15"))
        };
        _userTokenRepository.Add(userToken);
        await _txManager.SaveChangesAsync();
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
}