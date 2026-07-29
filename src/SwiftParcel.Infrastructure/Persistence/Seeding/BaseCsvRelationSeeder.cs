namespace SwiftParcel.Infrastructure.Persistence.Seeding;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Helpers;

public abstract class BaseCsvRelationSeeder<TEntity, TTarget> : IEntitySeeder 
    where TEntity : class 
    where TTarget : class
{
    public abstract int Order { get; }
    protected abstract string SqlQuery { get; }

    protected abstract Task<Dictionary<int, TEntity>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken);
    protected abstract Task<List<TTarget>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken);
    protected abstract void AttachRelation(TEntity entity, TTarget target);
    protected abstract bool RelationExists(TEntity entity, TTarget target);

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var entities = await GetEntitiesAsync(dbContext, cancellationToken);

        await using var command = oldDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = SqlQuery;

        await oldDbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var hasChanges = false;

        while (await reader.ReadAsync(cancellationToken))
        {
            var entityId = reader.GetInt32(0);
            var rawCsv = reader.GetString(1);

            var tokens = StringParserHelper.ParseCsvString(rawCsv);

            if (entities.TryGetValue(entityId, out var currentEntity))
            {
                foreach (var token in tokens)
                {
                    var targets = await ResolveTargetsAsync(dbContext, token, cancellationToken);

                    foreach (var target in targets)
                    {
                        if (!RelationExists(currentEntity, target))
                        {
                            AttachRelation(currentEntity, target);
                            hasChanges = true;
                        }
                    }
                }
            }
        }

        if (hasChanges)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}