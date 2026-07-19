using FluentValidation;

namespace Moji.Contracts.Models.Users.SearchUser;

public class SearchUserRequestValidator : AbstractValidator<SearchUserRequest>
{
    public SearchUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username không được bỏ trống!")
            .Matches($"^[a-z0-9_]+$").WithMessage("Username chỉ được chứa chữ thường, số và dấu gạch nối");
    }
}