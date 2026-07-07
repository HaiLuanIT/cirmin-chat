using FluentValidation;

namespace Moji.BusinessLogic.Models.Validators;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty().WithMessage("Cuộc hội thoại không được để trống!");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được để trống!");
    }
}