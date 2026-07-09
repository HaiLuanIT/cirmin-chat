using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Conversations.CreateConversation;

namespace Moji.BusinessLogic.Services.Conversations;

public interface IConversationService
{
    Task<CreateConversationResponse> CreateConversation(Guid currentUserId, CreateConversationRequest request);

    Task<ListConversationResponse> GetConversations(Guid currentUserId);

    Task<bool> IsMember(Guid currentUserId, Guid conversationId);

    Task<List<string>> GetJoinedConversationId(Guid currentUserId);

    Task<List<string>> GetConversationMemberIds(Guid conversationId);
}