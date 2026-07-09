using FluentValidation;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Services.Friends;
using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Conversations.CreateConversation;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Conversations;

public class ConversationService : IConversationService
{
    private readonly IValidator<CreateConversationRequest> _createConversationValidator;
    private readonly IConversationRepository _conversationRepository;
    private readonly IDbTransactionManager _txManager;
    private readonly IFriendShipService _friendShipService;

    public ConversationService(IValidator<CreateConversationRequest> createConversationValidator,
        IConversationRepository conversationRepository, IDbTransactionManager txManager, IFriendShipService friendShipService)
    {
        _createConversationValidator = createConversationValidator;
        _conversationRepository = conversationRepository;
        _txManager = txManager;
        _friendShipService = friendShipService;
    }


    public async Task<CreateConversationResponse> CreateConversation(Guid currentUserId, CreateConversationRequest request)
    {
        //validation request
        var validationResult = await _createConversationValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new MojiValidationException(validationResult.Errors);
        }

        if (request.UserIds.Any(x => x == currentUserId))
        {
            throw new MojiBadRequestException("Không thể tạo nhóm với bản thân!");
        }

        foreach (var userId in request.UserIds)
        {
            if (!( await _friendShipService.IsFriend(currentUserId, userId)))
            {
                throw new MojiBadRequestException("Bạn và người dùng này không là bạn bè!");
            }
        }

        Conversation conversation;
        await using var transaction = await _txManager.BeginTransactionAsync();
        try
        {
            conversation = new Conversation()
            {
                Name = request.Name,
                IsGroup = true
            };
            foreach (var requestUserId in request.UserIds)
            {
                conversation.AddMember(requestUserId);
            }
            conversation.AddMember(currentUserId);

            _conversationRepository.Add(conversation);
            await _txManager.SaveChangesAsync();
            await _txManager.CommitAsync();
        }
        catch (Exception e)
        {
            await _txManager.RollbackAsync();
            throw;
        }

        List<Guid> members = conversation.Members.Select(x => x.UserId).ToList();
        return new CreateConversationResponse(conversation.Id, conversation.Name, conversation.IsGroup,
            conversation.CreatedAt, members);
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
        if (conversation == null) throw new MojiNotFoundException("Không tìm thấy đoạn hội thoại");

        return conversation.Members.Select(x => x.UserId.ToString()).ToList();
    }
}