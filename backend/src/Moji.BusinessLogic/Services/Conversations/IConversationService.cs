using Moji.BusinessLogic.Models.Conversations;

namespace Moji.BusinessLogic.Services.Conversations;

public interface IConversationService
{
    Task<CreateConversationResponse> CreateConversation(Guid currentUserId, CreateConversationRequest request);

    Task<ListConversationResponse> GetConversations(Guid currentUserId);

    Task<bool> IsMember(Guid currentUserId, Guid conversationId);

    Task<List<string>> GetJoinedConversationId(Guid currentUserId);

    Task<List<string>> GetConversationMemberIds(Guid conversationId);
}