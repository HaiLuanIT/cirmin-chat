namespace CirMin.Contracts.Models.Users.SearchUser;

public record SearchUserRequest
{
    public string Username { get; init; } = string.Empty;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}