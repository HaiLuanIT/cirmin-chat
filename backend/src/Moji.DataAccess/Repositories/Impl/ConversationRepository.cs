using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories.Models;

namespace Moji.DataAccess.Repositories.Impl;

public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
    }

    public void Update(Conversation conversation)
    {
        _context.Entry(conversation).State = EntityState.Modified;
    }

    public async Task<Conversation?> FindByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ConversationRawData>> GetConversations(Guid userId)
    {
        return await _context.Conversations
            .Where(x => x.Members.Any(m => m.UserId == userId))
            .OrderByDescending(x => x.LastMessageTime ?? x.CreatedAt)
            .Select(c => new ConversationRawData
            {
                Id = c.Id,
                Name = c.Name,
                IsGroup = c.IsGroup,
                LastMessage = new LastMessageRawData
                {
                    Id = c.LastMessageId,
                    LastMessage = c.LastMessage,
                    LastMessageAt = c.LastMessageTime
                },
                CreatedAt = c.CreatedAt,
                UnreadCount = c.Members.Where(m => m.UserId == userId).Select(m => m.UnreadCount).FirstOrDefault(),
                Members = c.Members.OrderBy(m => m.JoinedAt)
                    .Take(c.IsGroup ? 4 : 2)
                    .Select(cm => new ConversationMemberRawData
                    {
                        UserId = cm.UserId,
                        DisplayName = cm.User.FullName,
                        AvatarUrl = cm.User.AvatarUrl
                    })
            })
            .ToListAsync();
    }

    public async Task<bool> IsMember(Guid userId, Guid conversationId)
    {
        return await _context.Conversations
            .AnyAsync(x => x.Id == conversationId && x.Members.Any(m => m.UserId == userId));
    }

    public async Task<List<string>> GetJoinerConversationIdsAsync(Guid userId)
    {
        var result = await _context.ConversationMembers.Where(x => x.UserId == userId)
            .Select(x => x.ConversationId.ToString()).ToListAsync();
        return result;
    }
}