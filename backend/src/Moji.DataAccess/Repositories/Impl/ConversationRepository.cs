using Microsoft.EntityFrameworkCore;
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

    public async Task<Conversation?> FindByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}