using Moji.BusinessLogic.Models.Auth;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task SignUp(RegisterRequest request)
    {
        var existedUser = await _userRepository.FindByUserNameAsync(request.UserName);
        if (existedUser != null)
        {
            throw new ApplicationException("Tên tài khoản này đã tồn tại trong hệ thống!");
        }

        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(request.Email);
        if (!isEmailUnique)
        {
            throw new ApplicationException("Email đã sử dụng!");
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User()
        {
            UserName = request.UserName,
            HashedPassword = hashedPassword,
            Email = request.Email,
            FullName = request.FullName
        };
        await _userRepository.AddAsync(user);
    }

    public async Task<AuthResponse> SignIn(LoginRequest request)
    {
        var user = await _userRepository.FindByUserNameAsync(request.Username);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Username hoặc password không chính xác!");
        }

        var isMatchPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, request.Password);

        if (!isMatchPassword)
        {
            throw new UnauthorizedAccessException("Username hoặc password không chính xác!");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var authResponse = new AuthResponse
        (
            new UserResponse
            (
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.AvatarUrl
            ),
            accessToken,
            refreshToken
        );
        return authResponse;
    }
}