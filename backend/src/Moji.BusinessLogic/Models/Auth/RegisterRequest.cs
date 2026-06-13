namespace Moji.BusinessLogic.Models.Auth;

public class RegisterRequest
{
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? Email { get; set; }
}