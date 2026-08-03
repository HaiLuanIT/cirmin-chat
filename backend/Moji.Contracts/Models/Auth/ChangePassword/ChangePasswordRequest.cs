namespace Moji.Contracts.Models.Auth.ChangePassword;

public record ChangePasswordRequest
{
    public string OldPassword { get; init; }

    public string NewPassword { get; init; }
}