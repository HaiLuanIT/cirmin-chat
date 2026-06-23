using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    
    void Update(Conversation conversation);

    Task<Conversation?> FindByIdAsync(Guid id);
}