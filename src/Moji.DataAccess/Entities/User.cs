using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string HashedPassword { get; set; }
    
    public string? Email { get; set; }
    
    public string FullName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public string? AvatarId { get; set; }
    
    public string? Bio { get; set; }
    
    //navigation
    public virtual ICollection<FriendShip> SentFriendRequests { get; set; } = new List<FriendShip>();
    
    public virtual ICollection<FriendShip> ReceivedFriendRequests { get; set; } = new List<FriendShip>();
    public virtual ICollection<ConversationMember> Conversations { get; set; } = new List<ConversationMember>();
}