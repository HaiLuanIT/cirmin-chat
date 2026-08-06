using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.Messages.SendMessage;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.Content).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
    }
}