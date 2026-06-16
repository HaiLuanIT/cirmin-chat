using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class ConversationMember : BaseEntity
{
    public Guid ConversationId { get; set; }
    
    public Guid UserId { get; set; }
    
    public int UnreadCount { get; set; }
    
    public long LastSeenMessageId { get; set; }
    
    //time join conversation
    public DateTime JoinedAt { get; set; }
    
    //navigation
    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}