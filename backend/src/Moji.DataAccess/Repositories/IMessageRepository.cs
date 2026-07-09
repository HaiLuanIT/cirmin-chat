using System.Linq.Expressions;
using Moji.Contracts.Models.Messages;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IMessageRepository
{
    void Add(Message message);

    Task<List<MessageResponse>> GetPagedMessagesAsync(
        Guid conversationId,
        long? lastId,
        DateTimeOffset? lastDate,
        int limit);

    Task<MessageResponse> GetMessageById(long id);
}