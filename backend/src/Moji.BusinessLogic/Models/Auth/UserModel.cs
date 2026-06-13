namespace Moji.BusinessLogic.Models.Auth;

public class UserModel
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public string? Bio { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}