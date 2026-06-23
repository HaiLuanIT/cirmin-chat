using Moji.BusinessLogic.Models;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task<SendMessageResponse> SendMessage(Guid senderId, SendMessageRequest request);
}