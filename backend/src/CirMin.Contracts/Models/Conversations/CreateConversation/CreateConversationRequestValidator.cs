using CirMin.Contracts.Errors;
using FluentValidation;

namespace CirMin.Contracts.Models.Conversations.CreateConversation;

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.UserIds).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .Must(members => members != null && members.Count >= 2)
            .WithErrorCode(ErrorCodes.Conversation.MinMembers)
            .WithState(_ => ValidationErrorParams.Create(("min", 2)));
        RuleFor(x => x.UserIds).Must(HaveUniqueIds).WithErrorCode(ErrorCodes.Conversation.NotDuplicatedMember);
    }

    private bool HaveUniqueIds(List<Guid> memberIds)
    {
        if (memberIds == null) return true;
        var set = new HashSet<Guid>();
        return memberIds.All(x => set.Add(x));
    }
}