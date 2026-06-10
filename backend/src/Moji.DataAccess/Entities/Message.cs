using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class Message : BaseEntity
{
    public long Id { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid ConversationId { get; set; }
    
    public string Content { get; set; }
    
    public string? ImageUrl { get; set; }
    
    //navigation
    public virtual User Sender { get; set; } = null!;

    public virtual Conversation Conversation { get; set; } = null!;
}
