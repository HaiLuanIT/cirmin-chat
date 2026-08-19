using CirMin.BusinessLogic.Exceptions;
using CirMin.BusinessLogic.Services.Friends;
using FluentValidation;
using CirMin.Contracts.Errors;
using CirMin.Contracts.Models.Conversations;
using CirMin.Contracts.Models.Conversations.CreateConversation;
using CirMin.DataAccess.Commons.DbTransactionManagers;
using CirMin.DataAccess.Entities;
using CirMin.DataAccess.Repositories;

namespace CirMin.BusinessLogic.Services.Conversations;

public class ConversationService : IConversationService
{
    private readonly IConversationNotificationService _conversationNotificationService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IValidator<CreateConversationRequest> _createConversationValidator;
    private readonly IFriendShipService _friendShipService;
    private readonly IDbTransactionManager _txManager;

    public ConversationService(IValidator<CreateConversationRequest> createConversationValidator,
        IConversationRepository conversationRepository, IDbTransactionManager txManager,
        IFriendShipService friendShipService, IConversationNotificationService conversationNotificationService)
    {
        _createConversationValidator = createConversationValidator;
        _conversationRepository = conversationRepository;
        _txManager = txManager;
        _friendShipService = friendShipService;
        _conversationNotificationService = conversationNotificationService;
    }


    public async Task CreateConversation(Guid currentUserId,
        CreateConversationRequest request)
    {
        //validation request
        var validationResult = await _createConversationValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new CirMinValidationException(validationResult.Errors);

        if (request.UserIds.Any(x => x == currentUserId))
            throw new CirMinBadRequestException(ErrorCodes.Conversation.CannotInviteSelf);

        foreach (var userId in request.UserIds)
            if (!await _friendShipService.IsFriend(currentUserId, userId))
                throw new CirMinBadRequestException(ErrorCodes.Conversation.NotIsFriend);

        Conversation conversation;
        await using var transaction = await _txManager.BeginTransactionAsync();
        try
        {
            conversation = new Conversation
            {
                Name = request.Name,
                IsGroup = true
            };
            foreach (var requestUserId in request.UserIds) conversation.AddMember(requestUserId);
            conversation.AddMember(currentUserId);

            _conversationRepository.Add(conversation);
            await _txManager.SaveChangesAsync();
            await _txManager.CommitAsync();
        }
        catch (Exception)
        {
            await _txManager.RollbackAsync();
            throw;
        }


        var conversationModel = await _conversationRepository.GetConversationById(currentUserId, conversation.Id);
        try
        {
            await _conversationNotificationService.NotifyConversationCreatedAsync(conversationModel);
        }
        catch (Exception)
        {
            Console.WriteLine("Lỗi khi thông báo tạo nhóm mới cho client");
        }
    }

    public async Task<ListConversationResponse> GetConversations(Guid currentUserId)
    {
        var conversations = await _conversationRepository.GetConversations(currentUserId);
        var conversationsResult = new ListConversationResponse(conversations);
        return conversationsResult;
    }

    public async Task<bool> IsMember(Guid currentUserId, Guid conversationId)
    {
        return await _conversationRepository.IsMember(currentUserId, conversationId);
    }

    public async Task<List<string>> GetJoinedConversationId(Guid currentUserId)
    {
        return await _conversationRepository.GetJoinerConversationIdsAsync(currentUserId);
    }

    public async Task<List<string>> GetConversationMemberIds(Guid conversationId)
    {
        var conversation = await _conversationRepository.FindByIdAsync(conversationId);
        if (conversation == null) throw new CirMinNotFoundException(ErrorCodes.Conversation.NotFound);

        return conversation.Members.Select(x => x.UserId.ToString()).ToList();
    }
}