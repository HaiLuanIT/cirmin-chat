using FluentValidation;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Helpers;
using Moji.BusinessLogic.Services.Friends;
using Moji.Contracts.Errors;
using Moji.Contracts.Models.Messages;
using Moji.Contracts.Models.Messages.SendMessage;
using Moji.Contracts.Models.Paginations.CursorPagination;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Messages;

public class MessageService : IMessageService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IFriendShipService _friendShipService;
    private readonly IMessageNotificationService _messageNotificationService;
    private readonly IMessageRepository _messageRepository;
    private readonly IValidator<SendMessageRequest> _sendMessageValidator;
    private readonly IDbTransactionManager _txManager;

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
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        //check conversation and sender is member
        var conversation = await _conversationRepository.FindByIdAsync(request.ConversationId);
        if (conversation == null) throw new MojiNotFoundException(ErrorCodes.Conversation.NotFound);

        var isMember = conversation.Members.Any(x => x.UserId == senderId);
        if (!isMember) throw new MojiBadRequestException(ErrorCodes.Conversation.Forbidden);

        var message = new Message
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

            //update lastmessage and unreadcount of member in conversations, instead of sender
            foreach (var member in conversation.Members)
            {
                member.LastSeenMessageId = message.Id;
                if (member.UserId != senderId) member.UnreadCount += 1;
            }

            _conversationRepository.Update(conversation);
            await _txManager.SaveChangesAsync();
            await _txManager.CommitAsync();
        }
        catch (Exception e)
        {
            await _txManager.RollbackAsync();
            throw;
        }

        var messageResponse = await _messageRepository.GetMessageById(message.Id);
        try
        {
            await _messageNotificationService.BroadcastMessageToConversationAsync(conversation.Id.ToString(),
                messageResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi khi thông báo tin nhắn mới cho client");
        }
    }

    public async Task<CursorPagingResult<MessageResponse>> GetConversationMessages(Guid currentUserId,
        Guid conversationId,
        int limit, string? cursor)
    {
        //check user is member of conversation
        var isMember = await _conversationRepository.IsMember(currentUserId, conversationId);

        if (!isMember) throw new MojiBadRequestException(ErrorCodes.Conversation.Forbidden);

        var (lastId, lastDate) = CursorPaginationHelper.Decode(cursor);
        var messages = await _messageRepository.GetPagedMessagesAsync(conversationId, lastId, lastDate, limit
        );

        var hasMore = messages.Count > limit;
        DateTimeOffset? nextDate = hasMore ? messages[^1].SentAt : null;
        long? nextId = hasMore ? messages[^1].Id : null;

        string? nextCursor = null;
        if (nextDate != null && nextId != null) nextCursor = CursorPaginationHelper.Encode(nextId, nextDate);

        if (hasMore) messages.RemoveAt(limit);

        return new CursorPagingResult<MessageResponse>
        {
            Items = messages,
            NextCursor = nextCursor,
            HasMore = hasMore
        };
    }

    public async Task<long?> MarkAsSeen(Guid currentUserId, Guid conversationId)
    {
        var member = await _conversationRepository.GetConversationMember(currentUserId, conversationId);
        if (member == null) throw new MojiForbiddenException(ErrorCodes.Conversation.Forbidden);

        var latestMessage = await _conversationRepository.GetLatestMessageId(conversationId);
        if (latestMessage == null) return null;

        member.LastSeenMessageId = latestMessage ?? 0;
        member.UnreadCount = 0;
        _conversationRepository.Update(member);
        await _txManager.SaveChangesAsync();
        return latestMessage;
    }
}