using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelStatus;

public class GetParcelStatusQueryHandler : IRequestHandler<GetParcelStatusQuery, Result<ParcelStatusResponse>>
{
    private readonly IAppDbContext _context;

    public GetParcelStatusQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ParcelStatusResponse>> Handle(GetParcelStatusQuery request, CancellationToken cancellationToken)
    {
        var parcel = await _context.Parcels
            .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (parcel == null)
        {
            return Result<ParcelStatusResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));
        }

        return Result<ParcelStatusResponse>.Success(new ParcelStatusResponse(parcel.Status));
    }
}