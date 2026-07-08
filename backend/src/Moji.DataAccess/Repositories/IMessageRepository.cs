using System.Linq.Expressions;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IMessageRepository
{
    void Add(Message message);

    Task<List<TResult>> GetPagedMessagesAsync<TResult>(
        Guid conversationId,
        long? lastId,
        DateTimeOffset? lastDate,
        int limit,
        Expression<Func<Message, TResult>> selector);

    Task<TResult> GetMessageById<TResult>(long id, Expression<Func<Message, TResult>> selector);
}