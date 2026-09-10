using System.Linq.Expressions;
using CirMin.DataAccess.Entities;
using CirMin.Contracts.Models.Messages;

namespace CirMin.DataAccess.Repositories;

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