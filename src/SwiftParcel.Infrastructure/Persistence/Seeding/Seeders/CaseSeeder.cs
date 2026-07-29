namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class CaseSeeder : BaseCsvRelationSeeder<Case, Parcel>
{
    private Dictionary<string, Parcel> _parcelMap = new();

    public override int Order => 11;
    protected override string SqlQuery => "SELECT id, parcel_tracking_numbers FROM cases WHERE parcel_tracking_numbers IS NOT NULL AND parcel_tracking_numbers != ''";

    protected override async Task<Dictionary<int, Case>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        _parcelMap = await dbContext.Parcels.ToDictionaryAsync(p => p.TrackingNumber, p => p, cancellationToken);
        return await dbContext.Cases.Include(c => c.Parcels).ToDictionaryAsync(c => c.Id, cancellationToken);
    }

    protected override Task<List<Parcel>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken)
    {
        var result = _parcelMap.TryGetValue(token, out var parcel) ? new List<Parcel> { parcel } : new List<Parcel>();
        return Task.FromResult(result);
    }

    protected override bool RelationExists(Case entity, Parcel target) => entity.Parcels.Any(p => p.Id == target.Id);
    protected override void AttachRelation(Case entity, Parcel target) => entity.Parcels.Add(target);
}