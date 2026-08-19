namespace CirMin.DataAccess.Models;

public record AvatarUpdateSnapshot
{
    public Guid Id { get; init; }

    public string? AvatarUrl { get; init; }

    public string? AvatarId { get; init; }

    public uint RowVersion { get; init; }
}