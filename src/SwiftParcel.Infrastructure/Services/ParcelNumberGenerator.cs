using SwiftParcel.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class ParcelNumberGenerator : IParcelNumberGenerator
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly AppDbContext _dbcontext;

    public ParcelNumberGenerator(AppDbContext dbContext)
    {
        _dbcontext = dbContext;
    }

    public async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken = default)
    {
        string code;
        bool exists;

        do
        {
            code = GenerateRandomCode();
            exists = await _dbcontext.Parcels.AnyAsync(p => p.TrackingNumber == code, cancellationToken);
        } while (exists);

        return code;
    }

    private static string GenerateRandomCode()
    {
        var randomBytes = new byte[8];
        RandomNumberGenerator.Fill(randomBytes);

        var sb = new StringBuilder(11);
        sb.Append("SP-");

        for (int i = 0; i < 8; i++)
        {
            sb.Append(AllowedChars[randomBytes[i] % AllowedChars.Length]);
        }

        return sb.ToString();
    }
}