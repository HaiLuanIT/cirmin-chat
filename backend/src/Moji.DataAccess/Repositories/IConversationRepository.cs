using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories.Models;

namespace Moji.DataAccess.Repositories;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    
    void Update(Conversation conversation);

    Task<Conversation?> FindByIdAsync(Guid id);

    Task<List<ConversationRawData>> GetConversations(Guid userId);

    Task<bool> IsMember(Guid userId, Guid conversationId);
}