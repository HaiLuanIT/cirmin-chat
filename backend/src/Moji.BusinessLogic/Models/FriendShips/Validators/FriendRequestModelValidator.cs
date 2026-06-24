using FluentValidation;

namespace Moji.BusinessLogic.Models.FriendShips.Validators;

public class FriendRequestModelValidator : AbstractValidator<FriendRequestModel>
{
    public FriendRequestModelValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Người nhận không được để trống!");
        RuleFor(x => x.Message).NotEmpty().WithMessage("Nội dung không được để trống!");
    }
}