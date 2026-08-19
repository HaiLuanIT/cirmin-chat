using CirMin.Contracts.Models.Paginations.OffsetPagination;
using CirMin.Contracts.Models.Users.SearchUser;
using CirMin.Contracts.Models.Users.UpdateUserInfo;
using CirMin.Contracts.Models.Users.UploadAvatar;

namespace CirMin.BusinessLogic.Services.Users;

public interface IUserService
{
    Task<OffsetPagingResult<SearchUserResponse>> SearchByUsername(Guid currentUserId, SearchUserRequest request);

    Task<UploadUserAvatarResponse> UpdateUserAvatarAsync(Guid currentUserId, Stream content, string fileName,
        CancellationToken cancellationToken);

    Task<UpdateUserInfoResponse> UpdateUserInfoAsync(Guid currentUserId, UpdateUserInfoRequest request,
        CancellationToken cancellationToken);
}