using Microsoft.EntityFrameworkCore;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Services.Friends;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Messages;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IFriendShipService _friendShipService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IDbTransactionManager _txManager;

    public MessageService(IFriendShipService friendShipService, IDbTransactionManager txManager,
        IMessageRepository messageRepository, IConversationRepository conversationRepository)
    {
        _friendShipService = friendShipService;
        _txManager = txManager;
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<SendMessageResponse> SendMessage(Guid senderId, SendMessageRequest request)
    {
        //check conversation and sender is member
        var conversation = await _conversationRepository.FindByIdAsync(request.ConversationId);
        if (conversation == null) throw new MojiNotFoundException("Không tìm thấy đoạn hội thoại");

        var isMember = conversation.Members.Any(x => x.UserId == senderId);
        if (!isMember)
        {
            throw new MojiBadRequestException("Bạn không có quyền gửi tin vào đoạn hội thoại này!");
        }

        //check if send direct
        if (conversation.IsGroup == false)
        {
            var recipient = conversation.Members.FirstOrDefault(x => x.UserId == request.ReceiverId);
            if (recipient != null)
            {
                var isFriend = await _friendShipService.IsFriend(senderId, request.ReceiverId);
                if (!isFriend) throw new MojiBadRequestException("Không thể gửi tin nhắn cho người lạ");
            }
        }

        //create message entity
        var message = new Message()
        {
            Content = request.Content,
            SenderId = senderId,
            ConversationId = conversation.Id
        };

        await using var transaction = await _txManager.BeginTransactionAsync();

        try
        {
            _messageRepository.Add(message);
            await _txManager.SaveChangesAsync();

            conversation.LastMessageId = message.Id;
            conversation.LastMessage = request.Content;

            _conversationRepository.Update(conversation);
            await _txManager.SaveChangesAsync();
            await _txManager.CommitAsync();
        }
        catch (Exception e)
        {
            await _txManager.RollbackAsync();
            throw;
        }

        return new SendMessageResponse(message.Id, message.SenderId, message.ConversationId, message.Content,
            message.CreatedAt);
    }

}