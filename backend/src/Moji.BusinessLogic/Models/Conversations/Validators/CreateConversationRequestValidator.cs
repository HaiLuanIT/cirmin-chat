using FluentValidation;

namespace Moji.BusinessLogic.Models.Conversations.Validators;

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên nhóm không được để trống!");
        RuleFor(x => x.UserIds).NotEmpty().WithMessage("Danh sách thành viên không được để trống!")
            .Must(members => members != null && members.Count >= 2)
            .WithMessage("Nhóm chat phải có tối thiểu 2 thành viên khác bạn!");
        RuleFor(x => x.UserIds).Must(HaveUniqueIds).WithMessage("Danh sách thành viên không được trùng nhau!");
        
    }

    private bool HaveUniqueIds(List<Guid> memberIds)
    {
        if (memberIds == null) return true;
        var set = new HashSet<Guid>();
        return memberIds.All(x => set.Add(x));
    }
}