namespace Moji.BusinessLogic.Services.Conversations;

public interface IConversationService
{
    Task CreateConversation(Guid userId);
}