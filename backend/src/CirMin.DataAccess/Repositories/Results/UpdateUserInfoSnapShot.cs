namespace CirMin.DataAccess.Models;

public record UpdateUserInfoSnapShot
{
    public string DisplayName { get; init; }

    public string? Bio { get; init; }

    public string? Email { get; init; }
}