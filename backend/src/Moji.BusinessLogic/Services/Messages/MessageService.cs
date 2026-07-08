using FluentValidation;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Helpers;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.Conversations;
using Moji.BusinessLogic.Models.CursorPagination;
using Moji.BusinessLogic.Services.Friends;
using Moji.DataAccess.Commons.DbTransactionManagers;
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
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageService(IFriendShipService friendShipService, IDbTransactionManager txManager,
        IMessageRepository messageRepository, IConversationRepository conversationRepository,
        IValidator<SendMessageRequest> sendMessageValidator,
        IMessageNotificationService messageNotificationService)
    {
        _sendMessageValidator = sendMessageValidator;
        _friendShipService = friendShipService;
        _txManager = txManager;
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _messageNotificationService = messageNotificationService;
    }

    public async Task SendMessage(Guid senderId, SendMessageRequest request)
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

        var messageResponse = await _messageRepository.GetMessageById(message.Id, m => new MessageResponse
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
        var conversationResponse = new ConversationModel
        (
            conversation.Id,
            conversation.Name,
            conversation.IsGroup,
            conversation.CreatedAt,
            new LastMessageModel(
                conversation.LastMessageId,
                conversation.LastMessage,
                conversation.LastMessageTime
            ),
            conversation.Members.Where(m => m.UserId == senderId).Select(m => m.UnreadCount)
                .FirstOrDefault(),
            conversation.Members.Select(x => new ConversationMemberModel(x.UserId, x.User.FullName, x.User.AvatarUrl))
                .ToList()
        );
        try
        {
            await _messageNotificationService.BroadcastMessageToConversationAsync(conversation.Id.ToString(),
                messageResponse, conversationResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi khi thông báo tin nhắn mới cho client");
        }
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