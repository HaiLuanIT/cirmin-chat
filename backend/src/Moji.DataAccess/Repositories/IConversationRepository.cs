using Moji.Contracts.Models.Conversations;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    
    void Update(Conversation conversation);
    
    void Update(ConversationMember conversationMember);

    Task<Conversation?> FindByIdAsync(Guid id);

    Task<List<ConversationModel>> GetConversations(Guid userId);

    Task<bool> IsMember(Guid userId, Guid conversationId);

    Task<List<string>> GetJoinerConversationIdsAsync(Guid userId);
    
    Task<ConversationMember> GetConversationMember(Guid userId, Guid conversationId);

    Task<long?> GetLatestMessageId(Guid conversationId);
}