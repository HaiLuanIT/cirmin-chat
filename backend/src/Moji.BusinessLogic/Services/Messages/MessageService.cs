using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Helpers;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.CursorPagination;
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
    private readonly IValidator<SendMessageRequest> _sendMessageValidator;

    public MessageService(IFriendShipService friendShipService, IDbTransactionManager txManager,
        IMessageRepository messageRepository, IConversationRepository conversationRepository,
        IValidator<SendMessageRequest> sendMessageValidator)
    {
        _sendMessageValidator = sendMessageValidator;
        _friendShipService = friendShipService;
        _txManager = txManager;
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<SendMessageResponse> SendMessage(Guid senderId, SendMessageRequest request)
    {
        //validate request
        var validationResult = await _sendMessageValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new MojiValidationException(validationResult.Errors);
        }

        //check conversation and sender is member
        var conversation = await _conversationRepository.FindByIdAsync(request.ConversationId);
        if (conversation == null) throw new MojiNotFoundException("Không tìm thấy đoạn hội thoại");

        var isMember = conversation.Members.Any(x => x.UserId == senderId);
        if (!isMember)
        {
            throw new MojiBadRequestException("Bạn không có quyền gửi tin vào đoạn hội thoại này!");
        }

        // //check if send direct
        // if (conversation.IsGroup == false)
        // {
        //     var recipient = conversation.Members.FirstOrDefault(x => x.UserId == request.ReceiverId);
        //     if (recipient != null)
        //     {
        //         var isFriend = await _friendShipService.IsFriend(senderId, request.ReceiverId);
        //         if (!isFriend) throw new MojiBadRequestException("Không thể gửi tin nhắn cho người lạ");
        //     }
        // }

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

    public async Task<CursorResponse<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId,
        int limit, string? cursor)
    {
        //check user is member of conversation
        var isMember = await _conversationRepository.IsMember(currentUserId, conversationId);

        if (!isMember) throw new MojiBadRequestException("Bạn không có quyền truy cập đoạn hội thoại này");
        
        var (lastId, lastDate) = CursorPaginationHelper.Decode(cursor);
        var messages = await _messageRepository.GetPagedMessagesAsync(conversationId, lastId, lastDate, limit, m =>
            new MessageResponse
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
            });

        var hasMore = messages.Count > limit;
        DateTimeOffset? nextDate = hasMore ? messages[^1].SentAt : null;
        long? nextId = hasMore ? messages[^1].Id : null;

        string? nextCursor = null;
        if (nextDate != null && nextId != null)
        {
            nextCursor = CursorPaginationHelper.Encode(nextId, nextDate);
        }

        if (hasMore) messages.RemoveAt(limit);

        return new CursorResponse<MessageResponse>()
        {
            Items = messages,
            NextCursor = nextCursor,
            HasMore = hasMore
        };
    }
}