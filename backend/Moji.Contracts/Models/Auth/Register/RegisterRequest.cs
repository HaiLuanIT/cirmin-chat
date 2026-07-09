namespace Moji.Contracts.Models.Auth.Register;

public class RegisterRequest
{
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? Email { get; set; }
}