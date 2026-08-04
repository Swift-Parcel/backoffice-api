using System.Security.Cryptography;
using SwiftParcel.Application.Common.Interfaces.Authentication;

namespace SwiftParcel.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32; 
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return $"{Convert.ToHexString(salt)}-{Convert.ToHexString(hash)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('-');
        if (parts.Length != 2)
        {
            return false;
        }

        try
        {
            byte[] salt = Convert.FromHexString(parts[0]);
            byte[] expectedHash = Convert.FromHexString(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }
}