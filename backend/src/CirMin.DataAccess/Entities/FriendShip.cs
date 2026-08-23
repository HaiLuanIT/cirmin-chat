using CirMin.DataAccess.Commons;
using CirMin.DataAccess.Commons.Constants;

namespace CirMin.DataAccess.Entities;

public class FriendShip : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserLeftId { get; set; }

    public Guid UserRightId { get; set; }

    public Guid RequesterId { get; set; }

    public string Status { get; set; }

    public string? Message { get; set; }

    //navigation
    public virtual User UserLeft { get; set; } = null!;

    public virtual User UserRight { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}