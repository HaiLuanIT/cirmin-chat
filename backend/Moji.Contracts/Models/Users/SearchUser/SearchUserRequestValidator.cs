using FluentValidation;
using Moji.Contracts.Errors;

namespace Moji.Contracts.Models.Users.SearchUser;

public class SearchUserRequestValidator : AbstractValidator<SearchUserRequest>
{
    public SearchUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .Matches("^[a-z0-9_]+$").WithErrorCode(ErrorCodes.Validation.InvalidFormat);
    }
}