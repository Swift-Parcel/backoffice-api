namespace SwiftParcel.Application.Helpers;

using Isopoh.Cryptography.Argon2;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password))
            return string.Empty;

        return Argon2.Hash(password);
    }
    
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        return Argon2.Verify(storedHash, password);
    }
}