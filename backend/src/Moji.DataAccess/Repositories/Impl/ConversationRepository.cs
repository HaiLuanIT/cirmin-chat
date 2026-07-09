using Microsoft.EntityFrameworkCore;
using Moji.Contracts.Models.Conversations;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;

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

    public void Update(ConversationMember conversationMember)
    {
        _context.Entry(conversationMember).State = EntityState.Modified;
    }

    public async Task<Conversation?> FindByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ConversationModel>> GetConversations(Guid userId)
    {
        return await _context.Conversations
            .Where(x => x.Members.Any(m => m.UserId == userId))
            .OrderByDescending(x => x.LastMessageTime ?? x.CreatedAt)
            .Select(c => new ConversationModel()
            {
                Id = c.Id,
                Name = c.Name,
                IsGroup = c.IsGroup,
                LastMessage = new LastMessageModel()
                {
                    Id = c.LastMessageId,
                    LastMessageContent = c.LastMessage,
                    LastMessageAt = c.LastMessageTime
                },
                CreatedAt = c.CreatedAt,
                UnreadCount = c.Members.Where(m => m.UserId == userId).Select(m => m.UnreadCount).FirstOrDefault(),
                Members = c.Members.OrderBy(m => m.JoinedAt)
                    .Take(c.IsGroup ? 4 : 2)
                    .Select(cm => new ConversationMemberModel
                    {
                        UserId = cm.UserId,
                        DisplayName = cm.User.FullName,
                        AvatarUrl = cm.User.AvatarUrl
                    }).ToList()
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

    public async Task<ConversationMember> GetConversationMember(Guid userId, Guid conversationId)
    {
        return await _context.ConversationMembers
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ConversationId == conversationId);
    }

    public async Task<long?> GetLatestMessageId(Guid conversationId)
    {
        return await _context.Conversations.Where(x => x.Id == conversationId).Select(x => x.LastMessageId).FirstOrDefaultAsync();
    }
}