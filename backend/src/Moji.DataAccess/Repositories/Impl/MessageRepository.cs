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
}