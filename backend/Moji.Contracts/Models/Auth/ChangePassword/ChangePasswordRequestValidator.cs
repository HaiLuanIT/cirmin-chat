using FluentValidation;

namespace Moji.Contracts.Models.Auth.ChangePassword;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống.");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 kí tự.")
            .MaximumLength(64).WithMessage("Mật khẩu không được vượt quá 64 kí tự.")
            .NotEqual(x => x.OldPassword).WithMessage("Mật khẩu mới không được trùng với mật khẩu cũ.");
    }
}