using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IMessageRepository
{
    void Add(Message message);
}