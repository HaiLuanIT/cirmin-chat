using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class Conversation : BaseEntity
{
  public Guid Id { get; set; }
  
  public string? Name { get; set; }

  public bool IsGroup { get; set; } = false;
  
  public long? LastMessageId { get; set; }
  
  public string? LastMessage { get; set; }
  
  public DateTime? LastMessageTime { get; set; }
  
  //navigation
  public virtual ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
  
  public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}