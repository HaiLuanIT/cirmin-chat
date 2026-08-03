using Moji.DataAccess.Commons;

namespace Moji.DataAccess.Entities;

public class UserToken : BaseEntity
{
    public Guid Id { get; set; }

    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public int AuthVersion { get; set; } = 0;

    public Guid UserId { get; set; }

    //navigation
    public virtual User User { get; set; } = null!;
}