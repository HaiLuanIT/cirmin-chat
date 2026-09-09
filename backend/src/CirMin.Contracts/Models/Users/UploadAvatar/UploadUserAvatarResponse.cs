namespace CirMin.Contracts.Models.Users.UploadAvatar;

public record UploadUserAvatarResponse
{
    public Guid UserId { get; init; }

    public string AvatarUrl { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}