using CirMin.DataAccess.Configurations;
using CirMin.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using CirMin.Contracts.Models.Conversations;

namespace CirMin.DataAccess.Repositories.Impl;

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
            .Select(c => new ConversationModel
            {
                Id = c.Id,
                Name = c.Name,
                IsGroup = c.IsGroup,
                LastMessage = new LastMessageModel
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
                        AvatarUrl = cm.User.AvatarUrl,
                        LastMessageId = cm.LastSeenMessageId
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
        return await _context.Conversations.Where(x => x.Id == conversationId).Select(x => x.LastMessageId)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<Guid, Guid>> GetDirectConversationIdsByParticipantIds(List<Guid> userIds,
        List<Guid> currentDirectConversationIds)
    {
        var result = await _context.Conversations
            .Where(conversation => !conversation.IsGroup
                                   && conversation.Members.Any(member => userIds.Contains(member.UserId)))
            .SelectMany(conversation => conversation.Members
                .Where(member => userIds.Contains(member.UserId))
                .Select(member => new
                {
                    member.UserId,
                    ConversationId = conversation.Id
                }))
            .ToDictionaryAsync(item => item.UserId, item => item.ConversationId);
        return result;
    }

    public async Task<List<Guid>> GetDirectConversationIdsByUserId(Guid userId)
    {
        var result = await _context.Conversations.AsNoTracking()
            .Where(conversation => !conversation.IsGroup && conversation.Members.Any(member => member.UserId == userId))
            .Select(x => x.Id)
            .ToListAsync();
        return result;
    }

    public async Task<ConversationModel> GetConversationById(Guid userId, Guid conversationId)
    {
        var result = await _context.Conversations.AsNoTracking()
            .Where(conversation => conversation.Id == conversationId)
            .Select(x => new ConversationModel
            {
                Id = x.Id,
                IsGroup = x.IsGroup,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                Members = x.Members.Select(member => new ConversationMemberModel
                {
                    UserId = member.UserId,
                    DisplayName = member.User.FullName,
                    AvatarUrl = member.User.AvatarUrl,
                    LastMessageId = member.LastSeenMessageId
                }).ToList(),
                LastMessage = new LastMessageModel
                {
                    Id = x.LastMessageId,
                    LastMessageContent = x.LastMessage,
                    LastMessageAt = x.LastMessageTime
                },
                UnreadCount = x.Members.Where(member => member.UserId == userId).Select(x => x.UnreadCount)
                    .FirstOrDefault()
            }).FirstOrDefaultAsync();
        return result;
    }
}