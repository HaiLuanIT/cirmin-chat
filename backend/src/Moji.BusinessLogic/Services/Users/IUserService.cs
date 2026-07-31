using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.Contracts.Models.Users.SearchUser;
using Moji.Contracts.Models.Users.UpdateUserInfo;
using Moji.Contracts.Models.Users.UploadAvatar;

namespace Moji.BusinessLogic.Services.Users;

public interface IUserService
{
    Task<OffsetPagingResult<SearchUserResponse>> SearchByUsername(Guid currentUserId, SearchUserRequest request);

    Task<UploadUserAvatarResponse> UpdateUserAvatarAsync(Guid currentUserId, Stream content, string fileName,
        CancellationToken cancellationToken);

    Task<UpdateUserInfoResponse> UpdateUserInfoAsync(Guid currentUserId, UpdateUserInfoRequest request,
        CancellationToken cancellationToken);
}