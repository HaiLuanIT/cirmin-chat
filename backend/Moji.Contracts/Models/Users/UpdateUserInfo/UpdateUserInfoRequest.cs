namespace Moji.Contracts.Models.Users.UpdateUserInfo;

public record UpdateUserInfoRequest
{
    public string? DisplayName { get; init; }

    public string? Bio { get; init; }

    public string? Email { get; init; }
}