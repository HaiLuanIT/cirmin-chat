using FluentValidation;

namespace Moji.Contracts.Models.Users.UpdateUserInfo;

public class UpdateUserInfoRequestValidator : AbstractValidator<UpdateUserInfoRequest>
{
    public UpdateUserInfoRequestValidator()
    {
        RuleFor(x => x.DisplayName).MinimumLength(1).WithMessage("Tên không được để trống.").MaximumLength(100)
            .WithMessage("Tên hiển thị không được vượt quá 100 ký tự");
        RuleFor(x => x.Email).MinimumLength(1).WithMessage("Email không được để trống").EmailAddress()
            .WithMessage("Email không hợp lệ!").MaximumLength(50).WithMessage("Email không được vượt quá 50 ký tự");
        RuleFor(x => x.Bio).MinimumLength(1).WithMessage("Bio không được để trống").MaximumLength(500)
            .WithMessage("Bio không vượt quá 500 kí tự.");
    }
}