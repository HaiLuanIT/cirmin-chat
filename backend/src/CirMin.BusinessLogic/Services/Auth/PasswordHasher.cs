using static BCrypt.Net.BCrypt;

namespace CirMin.BusinessLogic.Services.Auth;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltLength = 11;
    public string HashPassword(string password)
    {
        return EnhancedHashPassword(password, SaltLength);
    }   

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        return EnhancedVerify(providedPassword, hashedPassword);
    }
}