namespace Moji.BusinessLogic.Models.Auth;

public record UserResponse(Guid Id, string UserName, string Email, string FullName, string AvatarUrl);

public record AuthResponse(UserResponse User, string AccessToken, string RefreshToken);