using CirMin.Contracts.Errors;
using FluentValidation;

namespace CirMin.Contracts.Models.Users.SearchUser;

public class SearchUserRequestValidator : AbstractValidator<SearchUserRequest>
{
    public SearchUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(ErrorCodes.Validation.Required)
            .Matches("^[a-z0-9_]+$").WithErrorCode(ErrorCodes.Validation.InvalidFormat);
    }
}