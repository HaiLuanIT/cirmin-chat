using CirMin.Contracts.Errors;
using FluentValidation;

namespace CirMin.Contracts.Models.FriendShips.AddFriend;

public class AddFriendRequestValidator : AbstractValidator<AddFriendRequest>
{
    public AddFriendRequestValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
        RuleFor(x => x.Message).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required);
    }
}