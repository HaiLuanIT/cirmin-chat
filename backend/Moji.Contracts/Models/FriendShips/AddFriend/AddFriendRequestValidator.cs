using FluentValidation;

namespace Moji.Contracts.Models.FriendShips.AddFriend;

public class AddFriendRequestValidator : AbstractValidator<AddFriendRequest>
{
    public AddFriendRequestValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Người nhận không được để trống!");
        RuleFor(x => x.Message).NotEmpty().WithMessage("Nội dung không được để trống!");
    }
}