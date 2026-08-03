using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Models;

namespace Moji.DataAccess.Repositories;

public interface IUserRepository
{
    void Add(User user);

    Task<User?> FindByUsernameAsync(string username);

    Task<User?> FindByIdAsync(Guid id);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<OffsetPagingResult<UserSearchProjection>> SearchUserByUsername(Guid currentUserId, string username,
        int pageNumber,
        int pageSize);

    Task<AvatarUpdateSnapshot?> GetAvatarUpdateSnapshot(Guid userId, CancellationToken cancellationToken);

    Task<(UpdatedResult, DateTimeOffset)> TryUpdateAvatar(Guid userId, uint expectedVersion, string newAvatarUrl,
        string newAvatarId, CancellationToken cancellationToken);

    Task<UpdatedResult> UpdateUserInfo(User user, string? newDisplayName, string? newBio, string? newEmail,
        CancellationToken cancellationToken);

    Task<User?> GetTrackedUser(Guid userId, CancellationToken cancellationToken);

    Task<int?> GetAuthVersion(Guid userId, CancellationToken cancellationToken);
}