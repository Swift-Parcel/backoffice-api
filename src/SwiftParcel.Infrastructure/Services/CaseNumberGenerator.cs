using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class CaseNumberGenerator : ICaseNumberGenerator
{
    private readonly AppDbContext _dbContext;

    public CaseNumberGenerator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken = default)
    {
        var nextValue = await _dbContext.Database
            .SqlQueryRaw<long>("SELECT nextval('case_number_seq') AS \"Value\"")
            .SingleAsync(cancellationToken);

        var year = DateTime.UtcNow.Year;

        return $"CASE-{year}-{nextValue:D7}";
    }
}