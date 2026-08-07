using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelStatus;

public class GetParcelStatusQueryHandler : IRequestHandler<GetParcelStatusQuery, Result<ParcelStatusResponse>>
{
    private readonly IParcelRepository _parcelRepository;

    public GetParcelStatusQueryHandler(IParcelRepository parcelRepository)
    {
        _parcelRepository = parcelRepository;
    }

    public async Task<Result<ParcelStatusResponse>> Handle(GetParcelStatusQuery request, CancellationToken cancellationToken)
    {
        var parcel = await _parcelRepository.GetByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

        if (parcel == null)
        {
            return Result<ParcelStatusResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));
        }

        return Result<ParcelStatusResponse>.Success(new ParcelStatusResponse(parcel.Status));
    }
}