using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories.Impl;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Message message)
    {
        _context.Messages.Add(message);
    }

    public async Task<List<TResult>> GetPagedMessagesAsync<TResult>(Guid conversationId, long? lastId,
        DateTimeOffset? lastDate, int limit, Expression<Func<Message, TResult>> selector)
    {
        var query = _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId);

        if (lastId != null && lastDate != null)
        {
            query = query.Where(m => m.CreatedAt < lastDate || (m.CreatedAt == lastDate && m.Id < lastId.Value));
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(limit + 1)
            .Select(selector)
            .ToListAsync();
    }

    public async Task<TResult> GetMessageById<TResult>(long id, Expression<Func<Message, TResult>> selector)
    {
        return await _context.Messages.AsNoTracking().Where(x => x.Id == id).Select(selector).FirstOrDefaultAsync();
    }
}