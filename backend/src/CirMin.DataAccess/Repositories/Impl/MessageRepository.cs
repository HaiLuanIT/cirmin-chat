using System.Linq.Expressions;
using CirMin.DataAccess.Configurations;
using CirMin.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using CirMin.Contracts.Models.Messages;

namespace CirMin.DataAccess.Repositories.Impl;

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

    public async Task<List<MessageResponse>> GetPagedMessagesAsync(Guid conversationId, long? lastId,
        DateTimeOffset? lastDate, int limit)
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
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                Content = m.Content,
                Sender = new SenderResponse
                {
                    SenderId = m.SenderId,
                    DisplayName = m.Sender.FullName,
                    AvatarUrl = m.Sender.AvatarUrl
                },
                SentAt = m.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<MessageResponse> GetMessageById(long id)
    {
        return await _context.Messages.AsNoTracking().Where(x => x.Id == id).Select(m => new MessageResponse
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            Content = m.Content,
            Sender = new SenderResponse
            {
                SenderId = m.SenderId,
                DisplayName = m.Sender.FullName,
                AvatarUrl = m.Sender.AvatarUrl
            },
            SentAt = m.CreatedAt
        }).FirstOrDefaultAsync();
    }
}