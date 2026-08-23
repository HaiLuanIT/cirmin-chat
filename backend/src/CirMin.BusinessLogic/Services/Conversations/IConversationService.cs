using CirMin.Contracts.Models.Conversations;
using CirMin.Contracts.Models.Conversations.CreateConversation;

namespace CirMin.BusinessLogic.Services.Conversations;

public interface IConversationService
{
    Task CreateConversation(Guid currentUserId, CreateConversationRequest request);

    Task<ListConversationResponse> GetConversations(Guid currentUserId);

    Task<bool> IsMember(Guid currentUserId, Guid conversationId);

    Task<List<string>> GetJoinedConversationId(Guid currentUserId);

    Task<List<string>> GetConversationMemberIds(Guid conversationId);
}