namespace CirMin.DataAccess.Models;

public record UserSearchProjection
{
    public Guid Id { get; init; }

    public string Username { get; init; }

    public string DisplayName { get; init; }

    public string? AvatarUrl { get; init; }
}