using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.Contracts.Models.Users.SearchUser;

namespace Moji.BusinessLogic.Services.Users;

public interface IUserService
{
    Task<OffsetPagingResult<SearchUserResponse>> SearchByUsername(Guid currentUserId, SearchUserRequest request);
}