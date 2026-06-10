using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class FriendShip : BaseEntity
{
    public Guid Id { get; set; }
    
    public Guid RequesterId { get; set; }
    
    public Guid ReceiverId { get; set; }
    
    public string Status { get; set; } = "Pending";
    
    public string? Message { get; set; }
    
    //navigation
    public virtual User Requester { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;
}