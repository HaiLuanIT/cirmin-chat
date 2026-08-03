using FluentValidation;

namespace Moji.Contracts.Models.Users.UpdateUserInfo;

public class UpdateUserInfoRequestValidator : AbstractValidator<UpdateUserInfoRequest>
{
    public UpdateUserInfoRequestValidator()
    {
        RuleFor(x => x.DisplayName).MaximumLength(100)
            .WithMessage("Tên hiển thị không được vượt quá 100 ký tự");
        RuleFor(x => x.Email).EmailAddress()
            .WithMessage("Email không hợp lệ!").MaximumLength(50).WithMessage("Email không được vượt quá 50 ký tự");
        RuleFor(x => x.Bio).MaximumLength(500)
            .WithMessage("Bio không vượt quá 500 kí tự.");
    }
}