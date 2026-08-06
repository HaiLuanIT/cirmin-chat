using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.FriendShips.AddFriend;

public class AddFriendRequestValidator : AbstractValidator<AddFriendRequest>
{
    public AddFriendRequestValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.Message).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
    }
}