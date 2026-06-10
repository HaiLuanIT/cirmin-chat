namespace Moji.DataAccess.Commons;

public class BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? DeletedAt { get; set; } = null;

    public bool IsDeleted { get; set; } = false;
}