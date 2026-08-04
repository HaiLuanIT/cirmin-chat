namespace Moji.DataAccess.Models;

public record UserRelationShipProjection
{
    public Guid UserId { get; init; }
    public string Status { get; init; }

    public Guid RequesterId { get; init; }
}