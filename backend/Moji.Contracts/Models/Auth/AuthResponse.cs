namespace Moji.Contracts.Models.Auth;

public record UserResponse(Guid Id, string UserName, string Email, string DisplayName, string? AvatarUrl, string? Bio, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);

public record AuthResponse(UserResponse User, string AccessToken, string RefreshToken);